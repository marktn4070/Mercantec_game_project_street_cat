
using backend.Models;

namespace backend.Services
{
    public class GameStateService
    {
        private readonly Dictionary<int, GameState> _players = new();
        private readonly object _lock = new();

        // Should be removed, only for testing
        // Arduino connects players with game pin.
        public GameState PostPlayersToGame(int playerId)
        {
            lock (_lock)
            {
                var state = new GameState
                {
                    Id = playerId,
                    Status = "Running",
                    Action = "None",
                    CurrentBoost = null,
                    BoostExpiresAt = null,
                    IsBoostActive = false,
                    MapReady = false
                };

                _players[playerId] = state;

                return CopyState(state);
            }
        }
        public GameState? GetPlayerState(int playerId)
        {
            lock (_lock)
            {
                if (!_players.TryGetValue(playerId, out var state))
                    return null;

                // Check if invisibility has expired
                if (state.BoostExpiresAt.HasValue && DateTime.UtcNow >= state.BoostExpiresAt.Value)
                {
                    state.CurrentBoost = null;
                    //state.BoostExpiresAt = null;
                    state.IsBoostActive = false;
                }

                return CopyState(state);
            }
        }

        public bool SetMapReady(int playerId, bool ready)
        {
            lock (_lock)
            {
                if (!_players.TryGetValue(playerId, out var state))
                    return false;

                state.MapReady = ready;
                return true;
            }
        }

        public bool PostAction(int playerId, string action)
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

        // public bool UpdateScore(int playerId, int points)
        // {
        //     lock (_lock)
        //     {
        //         if (!_players.TryGetValue(playerId, out var state))
        //             return false;

        //         if (state.Status != "Running")
        //             return false;

        //         if (points <= 0 || points > 1000)
        //             return false;

        //         state.Score += points;
        //         return true;
        //     }
        // }

        public GameState RestartGame(int playerId)
        {
            lock (_lock)
            {
                if (!_players.TryGetValue(playerId, out var state))
                    return null;

                new GameState
                {
                    Id = playerId,
                    Status = state.Status = "Idle",
                    Action = state.Action = "None",
                    CurrentBoost = null,
                    //BoostExpiresAt = null,
                    IsBoostActive = false
                };

                _players[playerId] = state;
                return CopyState(state);
            }
        }

        public bool CollectBoost(int playerId, string boostName)
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
                //state.BoostExpiresAt = null;

                return true;
            }
        }

        public bool TryUseBoost(int playerId, out string? boost)
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

        public void InitializePlayers()
        {
            lock (_lock)
            {
                if (!_players.ContainsKey(1))
                {
                    _players[1] = new GameState
                    {
                        Id = 1,
                        Status = "Idle"
                    };
                }

                if (!_players.ContainsKey(2))
                {
                    _players[2] = new GameState
                    {
                        Id = 2,
                        Status = "Idle"
                    };
                }
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
        private GameState CopyState(GameState state)
        {
            return new GameState
            {
                Id = state.Id,
                Status = state.Status,
                Action = state.Action,
                CurrentBoost = state.CurrentBoost,
                //BoostExpiresAt = state.BoostExpiresAt,
                IsBoostActive = state.IsBoostActive,
                MapReady = state.MapReady,
            };
        }
    }
}
