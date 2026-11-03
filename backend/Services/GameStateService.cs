
using backend.Models;

namespace backend.Services
{
    public class GameStateService
    {
        private readonly Dictionary<string, GameState> _players = new();

        public GameState StartGame(string playerId)
        {
            var state = new GameState
            {
                PlayerId = playerId,
                Score = 0,
                Status = "Running",
                Action = "None",
                CurrentBoost = null
            };

            _players[playerId] = state;

            return state;
        }

        public GameState? GetState(string playerId)
        {
            _players.TryGetValue(playerId, out var state);
            return state;
        }

        public bool SetAction(string playerId, string action)
        {
            if (!_players.TryGetValue(playerId, out var state))
                return false;

            if (state.Status != "Running")
                return false;

            state.Action = action;
            return true;
        }

        public bool UpdateScore(string playerId, int points)
        {
            if (!_players.TryGetValue(playerId, out var state))
                return false;

            if (state.Status != "Running")
                return false;

            state.Score += points;
            return true;
        }

        public bool GameOver(string playerId)
        {
            if (!_players.TryGetValue(playerId, out var state))
                return false;

            state.Status = "GameOver";
            state.Action = "None";

            return true;
        }
    }
}

