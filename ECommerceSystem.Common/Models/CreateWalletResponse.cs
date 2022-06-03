using ECommerceSystem.Common.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerceSystem.Common.Models
{
    public class CreateWalletResponse
    {
        public uint Pvv { get; set; }
        public Wallet Wallet { get; set; }

        public CreateWalletResponse(uint pvv, Wallet wallet)
        {
            Pvv = pvv;
            Wallet = wallet;
        }
    }
}
