
using Microsoft.AspNetCore.Mvc;
using backend.Services;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly GameStateService _gameState;

        public GameController(GameStateService gameState)
        {
            _gameState = gameState;
        }

        // Start a player's game
        [HttpPost("start/{playerId}")]
        public IActionResult StartGame(string playerId)
        {
            var state = _gameState.StartGame(playerId);
            return Ok(state);
        }

        // Get a player's game state
        [HttpGet("state/{playerId}")]
        public IActionResult GetState(string playerId)
        {
            var state = _gameState.GetState(playerId);

            if (state == null)
                return NotFound("Player not found");

            return Ok(state);
        }

        // Player jumps
        [HttpPost("jump/{playerId}")]
        public IActionResult Jump(string playerId)
        {
            var success = _gameState.SetAction(playerId, "Jump");

            if (!success)
                return BadRequest("Player is not running");

            return Ok("Jump accepted");
        }

        // Player crawls
        [HttpPost("crawl/{playerId}")]
        public IActionResult Crawl(string playerId)
        {
            var success = _gameState.SetAction(playerId, "Crawl");

            if (!success)
                return BadRequest("Player is not running");

            return Ok("Crawl accepted");
        }

        // Update score
        [HttpPost("score/{playerId}")]
        public IActionResult UpdateScore(string playerId, [FromQuery] int points)
        {
            var success = _gameState.UpdateScore(playerId, points);

            if (!success)
                return BadRequest("Cannot update score");

            return Ok(_gameState.GetState(playerId));
        }

        // End a player's game
        [HttpPost("gameover/{playerId}")]
        public IActionResult GameOver(string playerId)
        {
            var success = _gameState.GameOver(playerId);

            if (!success)
                return NotFound("Player not found");

            return Ok(_gameState.GetState(playerId));
        }

        // Collect a boost
        [HttpPost("boost/collect/{playerId}")]
        public IActionResult CollectBoost(string playerId, [FromQuery] string boostName)
        {
            var success = _gameState.CollectBoost(playerId, boostName);

            if (!success)
                return BadRequest("Cannot collect boost");

            return Ok(_gameState.GetState(playerId));
        }

        // Use a boost
        [HttpPost("boost/use/{playerId}")]
        public IActionResult UseBoost(string playerId)
        {
            var success = _gameState.TryUseBoost(playerId, out var boost);

            if (!success)
                return BadRequest("No boost available");

            return Ok(_gameState.GetState(playerId));
        }
    }
}
