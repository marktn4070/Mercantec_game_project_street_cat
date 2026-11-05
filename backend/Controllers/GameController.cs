
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

        [HttpPost("postplayers/{id}")]
        public IActionResult PostPlayersToGame(int id)
        {
            var state = _gameState.PostPlayersToGame(id);
            return Ok(state);
        }

        [HttpGet("playerstate/{id}")]
        public IActionResult GetPlayerState(int id)
        {
            var state = _gameState.GetPlayerState(id);

            if (state == null)
                return NotFound("Player not found");

            return Ok(state);
        }

        [HttpPost("jump/{id}")]
        public IActionResult Jump(int id)
        {
            var success = _gameState.PostAction(id, "Jump");

            if (!success)
                return BadRequest("Player is not running");

            return Ok("Jump accepted");
        }

        [HttpPost("crawl/{id}")]
        public IActionResult Crawl(int id)
        {
            var success = _gameState.PostAction(id, "Crawl");

            if (!success)
                return BadRequest("Player is not running");

            return Ok("Crawl accepted");
        }

        // [HttpPost("score/{id}")]
        // public IActionResult UpdateScore(int id, [FromQuery] int points)
        // {
        //     var success = _gameState.UpdateScore(id, points);

        //     if (!success)
        //         return BadRequest("Cannot update score");

        //     return Ok(_gameState.GetPlayerState(id));
        // }

        [HttpPost("restartGame/{id}")]
        public IActionResult RestartGame(int id)
        {
            var success = _gameState.RestartGame(id);

            if (success == null)
                return NotFound("Player not found");

            return Ok(_gameState.GetPlayerState(id));
        }

        [HttpPost("boost/collect/{id}")]
        public IActionResult CollectBoost(int id, [FromQuery] string boostName)
        {
            var success = _gameState.CollectBoost(id, boostName);

            if (!success)
                return BadRequest("Cannot collect boost");

            return Ok(_gameState.GetPlayerState(id));
        }

        [HttpPost("boost/use/{id}")]
        public IActionResult UseBoost(int id)
        {
            var success = _gameState.TryUseBoost(id, out var boost);

            if (!success)
                return BadRequest("No boost available");

            return Ok(_gameState.GetPlayerState(id));
        }


        // Set map ready
        [HttpPost("mapready/{id}")]
        public IActionResult SetMapReady(int id, [FromQuery] bool ready)
        {
            var success = _gameState.SetMapReady(id, ready);

            if (!success)
                return NotFound("Player not found");

            return Ok(_gameState.GetPlayerState(id));
        }

        // Get all players
        [HttpGet("players")]
        public IActionResult GetAllPlayers()
        {
            return Ok(_gameState.GetAllPlayerStates());
        }
    }
}
