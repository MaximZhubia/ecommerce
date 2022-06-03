using ECommerceSystem.Common.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerceSystem.Common.Models
{
    public class BuyFileResponse
    {
        public File File { get; set; }
        public byte[] ContentBytes { get; set; }
        public Wallet ToWallet { get; set; }
        public Wallet FromWallet { get; set; }

        public BuyFileResponse()
        {

        }

        public BuyFileResponse(File file, byte[] contentBytes)
        {
            File = file;
            ContentBytes = contentBytes;
        }
    }
}
