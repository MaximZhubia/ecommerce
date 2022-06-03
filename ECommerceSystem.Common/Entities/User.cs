using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ECommerceSystem.Common.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public double Money { get; set; }
        public List<File> Files { get; set; }
        public List<ClientTransaction> ClientTransactions { get; set; }

        public User() { }

        public User(string username, string passwordHash)
        {
            Username = username;
            PasswordHash = passwordHash;
        }

        public User(string username, string passwordHash, double money) : this(username, passwordHash)
        {
            Money = money;
        }
    }
}
