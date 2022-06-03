using ECommerceSystem.Common.Entities;
using ECommerceSystem.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerceSystem.Interfaces
{
    public interface IUserService
    {
        IEnumerable<User> GetAll();
        User GetById(int id);
        AuthenticateResponse Authenticate(AuthenticateRequest model);
        User Add(string username, string passwordHash, double money);
        Wallet CreateWallet(int userId, out uint pvv);
    }
}
