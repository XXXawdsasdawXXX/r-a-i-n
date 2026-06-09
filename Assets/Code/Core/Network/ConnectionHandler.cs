using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Core.ServiceLocator;
using Cysharp.Threading.Tasks;
using FishNet;
using FishNet.Transporting;
using FishNet.Transporting.Tugboat;
using UnityEngine;

namespace Core.Network
{
    public sealed class ConnectionHandler : Essential.Mono, IService
    {
        public const string SAVE_KEY = "last_connection_ip";
        public string LastJoinedIP => _serverIP;
        public ushort Port => _port;

        [SerializeField] private string _serverIP = "192.168.1.100";
        [SerializeField] private ushort _port = 7777;

        private void OnEnable()
        {
            InstanceFinder.ClientManager.OnClientConnectionState += OnClientConnectionState;
            InstanceFinder.ServerManager.OnServerConnectionState += OnServerConnectionState;
        }

        private void OnDisable()
        {
            if (InstanceFinder.ClientManager != null)
            {
                InstanceFinder.ClientManager.OnClientConnectionState -= OnClientConnectionState;
            }

            if (InstanceFinder.ServerManager != null)
            {
                InstanceFinder.ServerManager.OnServerConnectionState -= OnServerConnectionState;
            }
        }

        public UniTask<bool> TryConnectAsClientAsync(string serverAddress) => ConnectAsClientAsync(serverAddress);

        public UniTask<bool> TryStartHostAsync() => StartHostAsync();

        public UniTask<bool> TryStartServerAsync() => StartServerAsync();

        private async UniTask<bool> ConnectAsClientAsync(string serverAddress)
        {
            ParseServerAddress(serverAddress, out string serverIP, out ushort? port);
            _serverIP = serverIP;

            if (port.HasValue)
            {
                _port = port.Value;
            }

            if (string.IsNullOrWhiteSpace(serverIP))
            {
                Debug.LogError("[Client] IP сервера не указан.");
                return false;
            }

            await EnsureStoppedAsync();
            ConfigureTransport(clientAddress: serverIP);

            if (!InstanceFinder.ClientManager.StartConnection())
            {
                Debug.LogError("[Client] Не удалось начать подключение.");
                return false;
            }

            Debug.Log($"[Client] Подключение к серверу {serverIP}:{_port}");

            if (!await WaitForClientStartedAsync())
            {
                Debug.LogError($"[Client] Не удалось подключиться к {serverIP}:{_port}. Проверьте IP, firewall на хосте и что хост уже в игре.");
                return false;
            }

            Debug.Log("[Client] Подключение установлено.");
            return true;
        }

        private async UniTask<bool> StartHostAsync()
        {
            await EnsureStoppedAsync();
            ConfigureTransport(clientAddress: "127.0.0.1", bindAllInterfaces: true);

            if (!InstanceFinder.ServerManager.StartConnection())
            {
                Debug.LogError("[Host] Не удалось начать сервер.");
                return false;
            }

            if (!await WaitForServerStartedAsync())
            {
                Debug.LogError($"[Host] Сервер не запустился. Состояние: {GetTransportState(true)}. Проверьте, свободен ли UDP-порт {_port}.");
                return false;
            }

            if (!InstanceFinder.ClientManager.StartConnection())
            {
                Debug.LogError("[Host] Не удалось запустить локальный клиент.");
                return false;
            }

            if (!await WaitForClientStartedAsync())
            {
                Debug.LogError("[Host] Локальный клиент не подключился к серверу.");
                return false;
            }

            Debug.Log($"[Host] Сервер запущен на {GetLocalIPAddress()}:{_port}");
            return true;
        }

        private async UniTask<bool> StartServerAsync()
        {
            await EnsureStoppedAsync();
            ConfigureTransport(bindAllInterfaces: true);

            if (!InstanceFinder.ServerManager.StartConnection())
            {
                Debug.LogError("[Server] Не удалось начать сервер.");
                return false;
            }

            if (!await WaitForServerStartedAsync())
            {
                Debug.LogError($"[Server] Сервер не запустился. Состояние: {GetTransportState(true)}. Проверьте, свободен ли UDP-порт {_port}.");
                return false;
            }

            Debug.Log($"[Server] Сервер запущен на {GetLocalIPAddress()}:{_port}");
            return true;
        }

        public static string GetLocalIPAddress()
        {
            foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (networkInterface.OperationalStatus != OperationalStatus.Up)
                {
                    continue;
                }

                if (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                {
                    continue;
                }

                foreach (UnicastIPAddressInformation address in networkInterface.GetIPProperties().UnicastAddresses)
                {
                    if (address.Address.AddressFamily != AddressFamily.InterNetwork)
                    {
                        continue;
                    }

                    string ip = address.Address.ToString();
                    if (ip.StartsWith("169.254."))
                    {
                        continue;
                    }

                    return ip;
                }
            }

            try
            {
                using Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0);
                socket.Connect("8.8.8.8", 65530);
                return (socket.LocalEndPoint as IPEndPoint)?.Address.ToString() ?? "127.0.0.1";
            }
            catch
            {
                return "127.0.0.1";
            }
        }

        public string GetHostAddressForClients() => $"{GetLocalIPAddress()}:{_port}";

        private static void ParseServerAddress(string serverAddress, out string serverIP, out ushort? port)
        {
            serverIP = serverAddress?.Trim() ?? string.Empty;
            port = null;

            int separatorIndex = serverIP.LastIndexOf(':');
            if (separatorIndex <= 0 || separatorIndex >= serverIP.Length - 1)
            {
                return;
            }

            string ipPart = serverIP[..separatorIndex];
            string portPart = serverIP[(separatorIndex + 1)..];

            if (!ushort.TryParse(portPart, out ushort parsedPort))
            {
                return;
            }

            serverIP = ipPart;
            port = parsedPort;
        }

        private void ConfigureTransport(string clientAddress = null, bool bindAllInterfaces = false)
        {
            if (InstanceFinder.NetworkManager.TransportManager.Transport is not Tugboat tugboat)
            {
                Debug.LogError("[Network] Tugboat transport не найден.");
                return;
            }

            tugboat.SetPort(_port);

            if (bindAllInterfaces)
            {
                tugboat.SetServerBindAddress("0.0.0.0", IPAddressType.IPv4);
            }

            if (!string.IsNullOrWhiteSpace(clientAddress))
            {
                tugboat.SetClientAddress(clientAddress);
            }
        }

        private async UniTask EnsureStoppedAsync()
        {
            LocalConnectionState serverState = GetTransportState(true);
            LocalConnectionState clientState = GetTransportState(false);

            if (serverState == LocalConnectionState.Stopped && clientState == LocalConnectionState.Stopped)
            {
                return;
            }

            if (serverState != LocalConnectionState.Stopped)
            {
                InstanceFinder.ServerManager.StopConnection(true);
            }

            if (clientState != LocalConnectionState.Stopped)
            {
                InstanceFinder.ClientManager.StopConnection();
            }

            await UniTask.WaitUntil(
                () => GetTransportState(true) == LocalConnectionState.Stopped
                      && GetTransportState(false) == LocalConnectionState.Stopped)
                .TimeoutWithoutException(TimeSpan.FromSeconds(3));
        }

        private bool IsServerReady()
        {
            return InstanceFinder.ServerManager.Started
                   || GetTransportState(true) == LocalConnectionState.Started;
        }

        private async UniTask<bool> WaitForServerStartedAsync()
        {
            if (IsServerReady())
            {
                return true;
            }

            bool reached = await UniTask.WaitUntil(IsServerReady)
                .TimeoutWithoutException(TimeSpan.FromSeconds(15));

            return reached || IsServerReady();
        }

        private bool IsClientReady()
        {
            return InstanceFinder.IsClientStarted;
        }

        private async UniTask<bool> WaitForClientStartedAsync()
        {
            if (IsClientReady())
            {
                return true;
            }

            bool reached = await UniTask.WaitUntil(IsClientReady)
                .TimeoutWithoutException(TimeSpan.FromSeconds(15));

            return reached || IsClientReady();
        }

        private LocalConnectionState GetTransportState(bool asServer)
        {
            return InstanceFinder.NetworkManager.TransportManager.Transport.GetConnectionState(asServer);
        }

        private void OnClientConnectionState(ClientConnectionStateArgs args)
        {
            Debug.Log($"[Client] Состояние подключения: {args.ConnectionState}");

            if (args.ConnectionState == LocalConnectionState.Stopped)
            {
                Debug.LogWarning("[Client] Подключение остановлено. Проверьте IP, порт, firewall и что хост уже запущен.");
            }
        }

        private void OnServerConnectionState(ServerConnectionStateArgs args)
        {
            Debug.Log($"[Server] Состояние сервера: {args.ConnectionState}");

            if (args.ConnectionState == LocalConnectionState.Stopped)
            {
                Debug.LogError($"[Server] Сервер остановлен. Возможно UDP-порт {_port} занят или заблокирован firewall.");
            }
        }
    }
}
