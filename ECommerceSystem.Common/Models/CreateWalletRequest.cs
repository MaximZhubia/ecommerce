using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerceSystem.Common.Models
{
    public class CreateWalletRequest
    {
        public int UserId { get; set; }

        public CreateWalletRequest() { }

        public CreateWalletRequest(int userId)
        {
            UserId = userId;
        }
    }
}
