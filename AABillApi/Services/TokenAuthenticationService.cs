using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AABillApi.Models;

namespace AABillApi.Services
{
    public class TokenAuthenticationService : IAuthenticateService
    {
        private readonly ITokenManagement _tokenManagement;
        public TokenAuthenticationService(ITokenManagement tokenManagement)
        {
            _tokenManagement = tokenManagement;
        }
        public void IsAuthenticated(CreatRoomDTO request, out string token)
        {
            token = string.Empty;
            var claims = new[]
            {
                new Claim("roomId",request.RoomId.ToString())
                // new Claim("User", "User"),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenManagement.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var jwtToken = new JwtSecurityToken(_tokenManagement.Issuer, _tokenManagement.Audience, claims, expires: DateTime.Now.AddMinutes(_tokenManagement.AccessExpiration), signingCredentials: credentials);
            token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }
    }

    public interface IAuthenticateService
    {
        void IsAuthenticated(CreatRoomDTO request, out string token);
    }
}
