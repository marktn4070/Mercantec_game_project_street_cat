
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
        public GameState? GetPlayerState(string playerId)
        {
            lock (_lock)
            {
                if (!_players.TryGetValue(playerId, out var state))
                    return null;

                // Check if invisibility has expired
                if (state.BoostExpiresAt.HasValue && DateTime.UtcNow >= state.BoostExpiresAt.Value)
                {
                    state.CurrentBoost = null;
                    state.BoostExpiresAt = null;
                    state.IsBoostActive = false;
                }

                return CopyState(state);
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
                if (action != "Jump" && action != "Crawl" && action != "None")
                    return false;

                state.Action = action;
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
                    var lobby = new GameLobby
                    {
                        Pin = pin
                    };

                    _lobbies.Add(lobby);
                }
                else if (gameMode == 2)
                {
                    //var pin = Random.Shared.Next(1000, 9999).ToString();
                    pin = "5678";
                    var lobby = new GameLobby
                    {
                        Pin = pin
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

        public GameState RestartGame(string gamePin)
        {
            lock (_lock)
            {
                var game = new GameLobby
                {
                    Pin = gamePin
                };

                var gameState = new GameState
                {
                    Status = "Running",
                    Action = "None",
                    CurrentBoost = null,
                    IsBoostActive = false,
                    Lobby = game
                };

                return CopyState(gameState);
            }
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
