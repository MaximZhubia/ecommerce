using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;

namespace ECommerceSystem.Common.Entities
{
    public class File
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public double Price { get; set; }
        public int UserId { get; set; }

        [JsonIgnore]
        public User User { get; set; }

        public File() { }

        public File(string name, string path, double price, int userId)
        {
            Name = name;
            Path = path;
            Price = price;
            UserId = userId;
        }
    }
}
