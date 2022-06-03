using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ECommerceSystem.Common.Entities
{
    public class Wallet
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string PvvHash { get; set; }
        public List<ClientTransaction> ClientTransactions { get; set; }
        public string Hash { get; set; }

        public Wallet()
        {
            ClientTransactions = new List<ClientTransaction>();
        }

        public Wallet(int userId, string pvvHash)
        {
            UserId = userId;
            PvvHash = pvvHash;
            ClientTransactions = new List<ClientTransaction>();
        }
    }
}
