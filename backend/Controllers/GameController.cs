
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

        [HttpPost("start/{id}")]
        public IActionResult StartGame(int id)
        {
            var state = _gameState.StartGame(id);
            return Ok(state);
        }

        [HttpGet("state/{id}")]
        public IActionResult GetState(int id)
        {
            var state = _gameState.GetState(id);

            if (state == null)
                return NotFound("Player not found");

            return Ok(state);
        }

        [HttpPost("jump/{id}")]
        public IActionResult Jump(int id)
        {
            var success = _gameState.SetAction(id, "Jump");

            if (!success)
                return BadRequest("Player is not running");

            return Ok("Jump accepted");
        }

        [HttpPost("crawl/{id}")]
        public IActionResult Crawl(int id)
        {
            var success = _gameState.SetAction(id, "Crawl");

            if (!success)
                return BadRequest("Player is not running");

            return Ok("Crawl accepted");
        }

        [HttpPost("score/{id}")]
        public IActionResult UpdateScore(int id, [FromQuery] int points)
        {
            var success = _gameState.UpdateScore(id, points);

            if (!success)
                return BadRequest("Cannot update score");

            return Ok(_gameState.GetState(id));
        }

        [HttpPost("gameover/{id}")]
        public IActionResult GameOver(int id)
        {
            var success = _gameState.GameOver(id);

            if (!success)
                return NotFound("Player not found");

            return Ok(_gameState.GetState(id));
        }

        [HttpPost("boost/collect/{id}")]
        public IActionResult CollectBoost(int id, [FromQuery] string boostName)
        {
            var success = _gameState.CollectBoost(id, boostName);

            if (!success)
                return BadRequest("Cannot collect boost");

            return Ok(_gameState.GetState(id));
        }

        [HttpPost("boost/use/{id}")]
        public IActionResult UseBoost(int id)
        {
            var success = _gameState.TryUseBoost(id, out var boost);

            if (!success)
                return BadRequest("No boost available");

            return Ok(_gameState.GetState(id));
        }

        // Get all players
        [HttpGet("players")]
        public IActionResult GetAllPlayers()
        {
            return Ok(_gameState.GetAllStates());
        }
    }
}
