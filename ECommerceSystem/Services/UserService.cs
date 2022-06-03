using ECommerceSystem.Common.Entities;
using ECommerceSystem.Common.Models;
using ECommerceSystem.Data;
using ECommerceSystem.Helpers;
using ECommerceSystem.Interfaces;
using ECommerceSystem.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceSystem.Services
{
    public class UserService : IUserService
    {
        private readonly AppSettings _appSettings;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly DbSet<User> _users;

        public UserService(IOptions<AppSettings> appSettings, ApplicationDbContext applicationDbContext)
        {
            _appSettings = appSettings.Value;
            _applicationDbContext = applicationDbContext;
            _users = _applicationDbContext.Users;
        }

        public IEnumerable<User> GetAll()
        {
            return _users;
        }

        public User GetById(int id)
        {
            return _users.FirstOrDefault(x => x.UserId == id);
        }

        public AuthenticateResponse Authenticate(AuthenticateRequest authRequest)
        {
            User user = _users.SingleOrDefault(x => x.Username == authRequest.Username && x.PasswordHash == authRequest.PasswordHash);

            // Return null if user not found
            if (user == null)
            {
                return null;
            }

            // Authentication successful so generate jwt token
            var token = GenerateJwtToken(user);

            return new AuthenticateResponse(user, token);
        }

        public User Add(string username, string passwordHash, double money)
        {
            var user = _users.SingleOrDefault(x => x.Username == username);

            // Return null if user not found
            if (user != null)
            {
                throw new Exception("User with the specified username already exists.");
            }

            return _applicationDbContext.AddUser(username, passwordHash, money);
        }

        public Wallet CreateWallet(int userId, out uint pvv)
        {
            pvv = Hash.GeneratePvvCode();
            string pvvHash = Hash.GetPvvHash(pvv);

            return _applicationDbContext.CreateWallet(userId, pvvHash);
        }

        private string GenerateJwtToken(User user)
        {
            // Generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("Id", user.UserId.ToString()) /*, new Claim("Username", user.Username.ToString())*/ }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
