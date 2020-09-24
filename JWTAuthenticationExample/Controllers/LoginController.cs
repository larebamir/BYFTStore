using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using JWTAuthenticationExample.Models;
using JWTAuthenticationExample.Repository.Implementation;
using JWTAuthenticationExample.Repository.Interface;
using JWTAuthenticationExample.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace JWTAuthenticationExample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IAdministration _loginService;

        private List<User> appUsers = new List<User>
        {
            new User {  FullName = "admin",  UserName = "admin", Password = "1234", UserRole = "Admin" },
            new User {  FullName = "Test User",  UserName = "user", Password = "1234", UserRole = "User" }
        };

        public LoginController(IConfiguration config)
        {
            _loginService = new Administration();
            _config = config;
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Login([FromBody]User login)
        {
            IActionResult response = Unauthorized();
            DataSet ds;
            ds = _loginService.ValidateUser(login.UserName, login.Password);   // EncryptionHelper.Encrypt(login["password"].ToString().Replace(" ", "+")));
            if (ds == null && ds.Tables.Count == 0)
            {
                return (ActionResult)Ok(new { message = "Login Failed !", error = true, errorCode = "ER0001" });
            }
            if (ds.Tables[0] == null && ds.Tables[0].Rows.Count == 0)
            {
                return (ActionResult)Ok(new { message = "Login Failed !", error = true, errorCode = "ER0001" });
            }
            List<User> user = DataHelper.ConvertDataTableToList<User>(ds.Tables[0]);

            //User user = AuthenticateUser(login);
            if (user != null)
            {
                var tokenString = GenerateJWTToken(user[0]);
                response = Ok(new
                {
                    token = tokenString,
                    userDetails = user,
                });
            }
            return response;
        }

        User AuthenticateUser(User loginCredentials)
        {
            User user = appUsers.SingleOrDefault(x => x.UserName == loginCredentials.UserName && x.Password == loginCredentials.Password);
            return user;
        }

        string GenerateJWTToken(User userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.UserName),
                new Claim("fullName", userInfo.FullName.ToString()),
                new Claim("role",userInfo.UserRole),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
