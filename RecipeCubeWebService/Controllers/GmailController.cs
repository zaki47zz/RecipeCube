using Microsoft.AspNetCore.Mvc;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Util.Store;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Humanizer;

namespace RecipeCubeWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GmailController : ControllerBase
    {
        private const string CredentialPath = ".credentials";

        [HttpGet("authorize")]
        public async Task<IActionResult> Authorize()
        {
            string[] scopes = { GmailService.Scope.GmailSend };
            UserCredential credential;

            using (var stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(CredentialPath, true));
            }

            // 更新的過期判斷方式
            var expirationTime = credential.Token.IssuedUtc + credential.Token.ExpiresInSeconds.Value.Seconds();
            if (System.DateTime.UtcNow >= expirationTime)
            {
                if (!await credential.RefreshTokenAsync(CancellationToken.None))
                {
                    return BadRequest("Access Token 已經過期，而且執行 Refresh Token 作業失敗！");
                }
            }

            return Ok(new { Message = "Credential file saved.", CredentialPath });
        }
    }
}
