
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using backend.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace backend.Services
{
    public class GameStateService
    {
        private readonly Dictionary<string, GameState> _players = new();

        //Store lobbies permantely
        private readonly List<GameLobby> _lobbies = new List<GameLobby>();
        private readonly object _lock = new();

        // Should be removed, only for testing
        // Arduino connects players with game pin.
        public GameState PostPlayersToGame(string playerId)
        {
            lock (_lock)
            {
                var state = new GameState
                {
                    PlayerId = playerId,
                    Status = "Running",
                    Action = "None",
                    CurrentBoost = null,
                    BoostExpiresAt = null,
                    IsBoostActive = false,
                    // MapReady = false
                };

                _players[playerId] = state;

                return CopyState(state);
            }
        }

        public void CreatePlayerState(string playerId)
        {
            lock (_lock)
            {
                var state = new GameState
                {
                    PlayerId = playerId,
                    Status = "Running",
                    Action = "None",
                    CurrentBoost = null,
                    BoostExpiresAt = null,
                    IsBoostActive = false,
                    // MapReady = false
                };
                _players[playerId] = state;
            }
        }

        public GameState? GetPlayerState(string playerId)
        {
            lock (_lock)
            {
                if (!_players.TryGetValue(playerId, out var state))
                    return null;
                    
                if (state.IsBoostActive && state.BoostExpiresAt.HasValue && DateTime.UtcNow >= state.BoostExpiresAt.Value)
                {
                    Console.WriteLine($"Boost expired for {playerId}");
                    state.IsBoostActive = false;
                    state.CurrentBoost = null;
                    state.BoostExpiresAt = null;
                }

                var copy = CopyState(state);
                if (state.Action == "Jump")
                {
                    state.Action = "None";
                }
                return copy;
            }
        }

        public bool PostAction(string playerId, string action)
        {
            lock (_lock)
            {
                if (!_players.TryGetValue(playerId, out var state))
                    return false;

                if (state.Status != "Running")
                    return false;

                if (action != "Jump" &&
                    action != "Crawl" &&
                    action != "Stand" &&
                    action != "None")
                    return false;

                if (action == "None")
                {
                    Console.WriteLine($"Ignoring None for {playerId}");
                    return true;
                }

                if (action != "None")
                {
                    state.Action = action;
                }

                Console.WriteLine($"Stored {action} for {playerId}");

                return true;
            }
        }

        public bool CollectBoost(string playerId, string boostName)
        {
            lock (_lock)
            {
                if (!_players.TryGetValue(playerId, out var state))
                    return false;

                if (state.Status != "Running")
                    return false;

                if (!string.IsNullOrEmpty(state.CurrentBoost))
                    return false;

                if (boostName != "Invisibility")
                    return false;

                state.CurrentBoost = boostName;
                state.BoostExpiresAt = null;

                return true;
            }
        }

        public bool TryUseBoost(string playerId, out string? boost)
        {
            lock (_lock)
            {
                boost = null;

                if (!_players.TryGetValue(playerId, out var state))
                    return false;

                if (state.Status != "Running")
                    return false;

                if (state.CurrentBoost != "Invisibility")
                    return false;

                if (state.IsBoostActive)
                    return false;

                boost = state.CurrentBoost;

                // Activate invisibility for 5 seconds
                state.IsBoostActive = true;
                state.BoostExpiresAt = DateTime.UtcNow.AddSeconds(5);

                return true;
            }
        }

        public List<GameState> GetAllPlayerStates()
        {
            lock (_lock)
            {
                var result = new List<GameState>();

                foreach (var state in _players.Values)
                {
                    result.Add(CopyState(state));
                }

                return result;
            }
        }

        public string PostCreateLobby(int gameMode)
        {
            lock (_lock)
            {
                var pin = "";
                if (gameMode == 1)
                {
                    //var pin = Random.Shared.Next(1000, 9999).ToString();
                    pin = "1234";
                    _lobbies.RemoveAll(l => l.Pin == pin);
                    var lobby = new GameLobby
                    {
                        Pin = pin,
                        MaxPlayers = 1
                    };

                    _lobbies.Add(lobby);
                }
                else if (gameMode == 2)
                {
                    //var pin = Random.Shared.Next(1000, 9999).ToString();
                    pin = "5678";
                    _lobbies.RemoveAll(l => l.Pin == pin);
                    var lobby = new GameLobby
                    {
                        Pin = pin,
                        MaxPlayers = 2
                    };

                    _lobbies.Add(lobby);
                }
                return pin;
            }
        }

        public GameLobby? GetGameLobby(string pin)
        {
            lock (_lock)
            {
                return _lobbies.FirstOrDefault(x => x.Pin == pin);
            }
        }

        public bool RestartGame(string gamePin)
        {
            var lobby = _lobbies.FirstOrDefault(l => l.Pin == gamePin);

            if (lobby == null)
                return false;
            Console.WriteLine($"Lobby found: {lobby.Pin}");

            foreach (var player in lobby.Players)
            {
                Console.WriteLine($"Player: {player.PlayerId}");
                if (_players.TryGetValue(player.PlayerId, out var state))
                {
                    Console.WriteLine($"Status: {state.Status}");
                    state.Status = "Running";
                    state.Action = "None";
                    state.CurrentBoost = null;
                    state.BoostExpiresAt = null;
                    state.IsBoostActive = false;
                }
            }
            return true;
        }

        public void CloseLobby(string pin)
        {
            var lobby =
                _lobbies.FirstOrDefault(
                    l => l.Pin == pin
                );

            if (lobby == null)
                return;

            foreach (var player in lobby.Players)
            {
                _players.Remove(player.PlayerId);
            }

            _lobbies.Remove(lobby);

            Console.WriteLine(
                $"[LOBBY CLOSED] {pin}"
            );
        }

        private GameState CopyState(GameState state)
        {
            return new GameState
            {
                PlayerId = state.PlayerId,
                Status = state.Status,
                Action = state.Action,
                CurrentBoost = state.CurrentBoost,
                BoostExpiresAt = state.BoostExpiresAt,
                IsBoostActive = state.IsBoostActive,
                Lobby = state.Lobby
            };
        }

      
    }
}
