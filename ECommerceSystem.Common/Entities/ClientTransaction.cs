using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ECommerceSystem.Common.Entities
{
    public class ClientTransaction
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int WalletId { get; set; }
        public DateTime DateTime { get; set; }
        public double StartValue { get; set; }
        public double FinishValue { get; set; }
        public double IncomeValue { get; set; }
        public double OutcomeValue { get; set; }

        public ClientTransaction() { }

        public ClientTransaction(int userId, double incomeValue, double outcomeValue)
        {
            UserId = userId;
            IncomeValue = incomeValue;
            OutcomeValue = outcomeValue;
            DateTime = DateTime.Now;
        }
    }
}
