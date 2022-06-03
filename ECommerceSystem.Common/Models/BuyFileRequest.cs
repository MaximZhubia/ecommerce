using ECommerceSystem.Common.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerceSystem.Common.Models
{
    public class BuyFileRequest
    {
        public int FileId { get; set; }
        public Wallet Wallet { get; set; }

        public BuyFileRequest()
        {

        }

        public BuyFileRequest(int fileId, Wallet wallet)
        {
            FileId = fileId;
            Wallet = wallet;
        }
    }
}
