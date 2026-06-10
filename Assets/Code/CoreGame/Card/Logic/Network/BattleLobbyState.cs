using CoreGame.Card.Data;

namespace CoreGame.Card.Logic.Network
{
    public readonly struct BattleLobbyState
    {
        public string ActivatorId { get; }
        public bool IsOpen { get; }
        public int PlayersWaiting { get; }
        public int MaxPlayers { get; }
        public int MinPlayers { get; }
        public EBattleMode Mode { get; }
        public bool IsHost { get; }
        public bool AllowEarlyStart { get; }

        public BattleLobbyState(
            string activatorId,
            bool isOpen,
            int playersWaiting,
            int maxPlayers,
            int minPlayers,
            EBattleMode mode,
            bool isHost,
            bool allowEarlyStart)
        {
            ActivatorId = activatorId;
            IsOpen = isOpen;
            PlayersWaiting = playersWaiting;
            MaxPlayers = maxPlayers;
            MinPlayers = minPlayers;
            Mode = mode;
            IsHost = isHost;
            AllowEarlyStart = allowEarlyStart;
        }

        public bool CanStart =>
            IsHost
            && IsOpen
            && PlayersWaiting >= MinPlayers
            && (AllowEarlyStart || PlayersWaiting >= MaxPlayers);

        public string GetStatusText()
        {
            if (!IsOpen)
            {
                return string.Empty;
            }

            return $"Ожидание игроков: {PlayersWaiting}/{MaxPlayers}";
        }

        public string GetHintText()
        {
            if (!IsOpen || !IsHost)
            {
                return string.Empty;
            }

            if (PlayersWaiting >= MaxPlayers)
            {
                return "Команда собрана — можно начинать бой.";
            }

            if (!AllowEarlyStart || PlayersWaiting < MinPlayers)
            {
                return "Дождитесь напарника или подключите второго игрока к этому активатору.";
            }

            return Mode switch
            {
                EBattleMode.CoOpPvE => "Можно начать в одиночку — бой пройдёт как PvE.",
                EBattleMode.PvP or EBattleMode.Duel => "Недостаточно игроков для PvP.",
                _ => "Можно начать с текущим составом."
            };
        }
    }
}
