using System;
using System.Collections.Generic;
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

            _logHostConnectionInfo();
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

            _logHostConnectionInfo();
            return true;
        }

        public static string GetLocalIPAddress() => GetLanAddressCandidates().primaryIp;

        public static (string primaryIp, List<string> allIps) GetLanAddressCandidates()
        {
            List<(string ip, int score, string adapter)> candidates = new();

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

                if (_isVirtualInterface(networkInterface))
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

                    candidates.Add((ip, _scoreLanAddress(ip, networkInterface.NetworkInterfaceType), networkInterface.Name));
                }
            }

            if (candidates.Count > 0)
            {
                candidates.Sort((a, b) => b.score.CompareTo(a.score));

                List<string> allIps = new();
                foreach ((string ip, int _, string adapter) in candidates)
                {
                    allIps.Add($"{ip} ({adapter})");
                }

                return (candidates[0].ip, allIps);
            }

            try
            {
                using Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0);
                socket.Connect("8.8.8.8", 65530);
                string fallbackIp = (socket.LocalEndPoint as IPEndPoint)?.Address.ToString() ?? "127.0.0.1";
                return (fallbackIp, new List<string> { fallbackIp });
            }
            catch
            {
                return ("127.0.0.1", new List<string> { "127.0.0.1" });
            }
        }

        private static bool _isVirtualInterface(NetworkInterface networkInterface)
        {
            string name = networkInterface.Name.ToLowerInvariant();
            string description = networkInterface.Description.ToLowerInvariant();

            return name.Contains("virtual")
                   || name.Contains("vethernet")
                   || name.Contains("wsl")
                   || name.Contains("hyper-v")
                   || name.Contains("vmware")
                   || name.Contains("virtualbox")
                   || name.Contains("vpn")
                   || name.Contains("tap")
                   || name.Contains("tun")
                   || description.Contains("virtual")
                   || description.Contains("hyper-v")
                   || description.Contains("vpn");
        }

        private static int _scoreLanAddress(string ip, NetworkInterfaceType interfaceType)
        {
            int score = 0;

            if (ip.StartsWith("192.168."))
            {
                score += 100;
            }
            else if (ip.StartsWith("10."))
            {
                score += 80;
            }
            else if (ip.StartsWith("172."))
            {
                string[] parts = ip.Split('.');
                if (parts.Length > 1 && int.TryParse(parts[1], out int secondOctet) && secondOctet >= 16 && secondOctet <= 31)
                {
                    score += 60;
                }
            }

            if (interfaceType == NetworkInterfaceType.Wireless80211)
            {
                score += 20;
            }
            else if (interfaceType == NetworkInterfaceType.Ethernet)
            {
                score += 15;
            }

            return score;
        }

        public string GetHostAddressForClients() => $"{GetLocalIPAddress()}:{_port}";

        private void _logHostConnectionInfo()
        {
            (string primaryIp, List<string> allIps) = GetLanAddressCandidates();

            Debug.Log($"[Host] Для подключения с другого устройства введите IP: {primaryIp}");
            Debug.Log($"[Host] Порт: {_port}");
            Debug.Log($"[Host] Все IP этого компьютера: {string.Join(", ", allIps)}");
        }

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
