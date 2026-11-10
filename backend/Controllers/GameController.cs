
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

        [HttpGet("playerstate/{id}")]
        public IActionResult GetPlayerState(string id)
        {
            var state = _gameState.GetPlayerState(id);

            if (state == null)
                return NotFound("Player not found");

            return Ok(state);
        }

        [HttpPost("boost/collect/{id}")]
        public IActionResult CollectBoost(string id, [FromQuery] string boostName)
        {
            var success = _gameState.CollectBoost(id, boostName);

            if (!success)
                return BadRequest("Cannot collect boost");

            return Ok(_gameState.GetPlayerState(id));
        }

        [HttpPost("boost/use/{id}")]
        public IActionResult UseBoost(string id)
        {
            var success = _gameState.TryUseBoost(id, out var boost);

            if (!success)
                return BadRequest("No boost available");

            return Ok(_gameState.GetPlayerState(id));
        }

        // Get all players
        [HttpGet("players")]
        public IActionResult GetAllPlayers()
        {
            return Ok(_gameState.GetAllPlayerStates());
        }

        [HttpPost("createlobby")]
        public IActionResult CreateLobby([FromQuery]int gameMode)
        {
            if(gameMode != 1 && gameMode != 2)
                    return BadRequest("Wrong gamemode");
            
            var lobby = _gameState.PostCreateLobby(gameMode);
            if(lobby == null)
                return BadRequest("Lobby could not be created");

            return Ok(lobby);
        }

        [HttpGet("lobby/{pin}")]
        public IActionResult GetLobby(string pin)
        {
            var lobby = _gameState.GetGameLobby(pin);
            if(lobby == null)
                return NotFound();

            return Ok(lobby);
        }
        
        [HttpPost("restartGame/{pin}")]
        public IActionResult RestartGame(string pin)
        {
            var success = _gameState.RestartGame(pin);

            if (!success)
                return BadRequest("Cannot restart game");

            return Ok();
        }

    }
}
