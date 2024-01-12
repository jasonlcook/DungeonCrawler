using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;

using BlazorDungeonCrawler.Server.Data;
using BlazorDungeonCrawler.Shared.Models;
using BlazorDungeonCrawler.Shared.Responses;

namespace BlazorDungeonCrawler.Server.Controllers {

    [ApiController]
    [Route("api/accesstoken")]
    public class AccessTokenConroller : Controller {
        private ILogger<AccessTokenConroller> _logger;
        private AccessTokenManager _tokenManager;

        public AccessTokenConroller(AccessTokenManager tokenManager, ILogger<AccessTokenConroller> logger) {
            this._tokenManager = tokenManager;
            this._logger = logger;
        }

        [HttpGet]
        [Route("new/{name}")]
        [EnableCors("AllowAnyOriginMethodHeader")]
        public async Task<ActionResult<TokenResponse>> Generate(string name) {
            try {
                AccessToken accessToken = await _tokenManager.Generate(name);

                return Ok(new TokenResponse() {
                    Success = true,
                    Name = accessToken.Name
                });
            } catch (Exception ex) {
                _logger.LogError($"Token check Error: {ex.Message}");

                return StatusCode(500, new TokenResponse() {
                    Success = false,
                    ErrorMessages = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("{token}")]        
        [EnableCors("AllowAnyOriginMethodHeader")]
        public async Task<ActionResult<TokenResponse>> Retrieve(string token) {
            try {
                AccessToken accessToken = await _tokenManager.RetrieveAccessToken(token);

                return Ok(new TokenResponse() {
                    Success = true,
                    Name = accessToken.Name
                });
            } catch (Exception ex) {
                _logger.LogError($"Token check Error: {ex.Message}");

                return StatusCode(500, new TokenResponse() {
                    Success = false,
                    ErrorMessages = ex.Message
                });
            }
        }
    }
}
