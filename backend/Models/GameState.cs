using System;
namespace backend.Models
{
    public class GameState
    {
        public string PlayerId { get; set; }
        public string PlayerName {get; set;} = "";
        // public int Score { get; set; } = 0;
        public string Status { get; set; } = "Idle";
        public string Action { get; set; } = "None";
        public string? CurrentBoost { get; set; }
        public DateTime? BoostExpiresAt { get; set; }
        public bool IsBoostActive { get; set; } = false;
        public GameLobby Lobby {get; set;}
        // public bool MapReady { get; set; } = false;
    }
}
