using System;
namespace backend.Models
{
    public class GameLobby
    {        
        public string Pin {get; set;} = "";
        public int MaxPlayers {get; set;}
        public List<LobbyPlayer> Players{get; set;} = new();
        public bool Started {get; set;}
    }
}
