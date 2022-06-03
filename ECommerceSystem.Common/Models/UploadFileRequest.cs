using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;

namespace ECommerceSystem.Common.Models
{
    public class UploadFileRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        public int OwnerId { get; set; }
        [Required]
        public byte[] FileContentBytes { get; set; }
        
        public UploadFileRequest() { }

        public UploadFileRequest(string name, double price, int ownerId, byte[] fileContentBytes)
        {
            Name = name;
            Price = price;
            OwnerId = ownerId;
            FileContentBytes = fileContentBytes;
        }
    }
}
