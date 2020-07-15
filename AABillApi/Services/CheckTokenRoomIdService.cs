using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using AABillApi.Models;
using System.Threading;

namespace AABillApi.Services
{
    public class AccountRequirement : IAuthorizationRequirement
    {
    }
    public class CheckTokenRoomIdService : AuthorizationHandler<AccountRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CheckTokenRoomIdService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }
        async protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AccountRequirement requirement)
        {
            var httpContext = _httpContextAccessor;
            httpContext.HttpContext.Request.EnableBuffering();
            string authHeader = httpContext.HttpContext.Request.Headers["Authorization"];
            string tokenStr = authHeader.Replace("Bearer ", "");
            var handler = new JwtSecurityTokenHandler();
            var payload = handler.ReadJwtToken(tokenStr).Payload;
            var claims = payload.Claims;
            var roomId = claims.First(claim => claim.Type == "roomId").Value;
            var body = httpContext.HttpContext.Request.Body;
            var reader = new StreamReader(body);
            var bodyStr = await reader.ReadToEndAsync();
            body.Position = 0;
            string roomIdByBody = string.Empty;
            if (string.IsNullOrEmpty(bodyStr))
            {
                roomIdByBody = httpContext.HttpContext.Request.RouteValues.Values.ToList()[2].ToString();
            }
            else
            {
                roomIdByBody = JsonConvert.DeserializeObject<CreatRoomDTO>(bodyStr).RoomId.ToString();
            }
            if (roomId != roomIdByBody)
            {
                context.Fail();
                return;
            }
            context.Succeed(requirement);
            return;
        }
    }
}
