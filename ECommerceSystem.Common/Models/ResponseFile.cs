using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerceSystem.Common.Models
{
    public class ResponseFile
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int UserId { get; set; }
        public string OwnerUsername { get; set; }

        public ResponseFile() { }

        public ResponseFile(int id, string name, double price, int userId, string ownerUsername)
        {
            Id = id;
            Name = name;
            Price = price;
            UserId = userId;
            OwnerUsername = ownerUsername;
        }
    }
}
