using ECommerceSystem.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerceSystem.Common.Models
{
    public class AuthenticateResponse
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }

        public AuthenticateResponse() { }

        public AuthenticateResponse(User user, string token)
        {
            Id = user.UserId;
            Username = user.Username;
            Token = token;
        }
    }
}
