using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using AABillApi.Models;

namespace AABillApi.Services
{
    public class AccountRequirement : IAuthorizationRequirement
    {
        public readonly string RoleName;
        public AccountRequirement(string roleName)
        {
            RoleName = roleName;
        }
    }
    public class CheckTokenRoomIdService : AuthorizationHandler<AccountRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CheckTokenRoomIdService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }
        protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AccountRequirement requirement)
        {
            var httpContext = _httpContextAccessor;
            string authHeader = httpContext.HttpContext.Request.Headers["Authorization"];
            string tokenStr = authHeader.Replace("Bearer ", "");
            var handler = new JwtSecurityTokenHandler();
            var payload = handler.ReadJwtToken(tokenStr).Payload;
            var claims = payload.Claims;
            var roomId = claims.First(claim => claim.Type == "roomId").Value;
            var body = httpContext.HttpContext.Request.Body;
            using (var reader = new StreamReader(body))
            {
                //var bodyStr = await reader.ReadToEndAsync();
                //var roomIdByBody = JsonConvert.DeserializeObject<CreatRoomDTO>(bodyStr).RoomId.ToString();
                //if (roomId == roomIdByBody)
                //{
                    //context.Succeed(requirement);
                //}
            }

            return Task.CompletedTask;
        }
    }
}
