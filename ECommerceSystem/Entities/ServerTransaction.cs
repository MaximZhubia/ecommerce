using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerceSystem.Entities
{
    public class ServerTransaction
    {
        public int Id { get; set; }
        public int FromClientId { get; set; }
        public int ToClientId { get; set; }
        public DateTime DateTime { get; set; }
        public int FileId { get; set; }
        public double Value { get; set; }

        public ServerTransaction() { }

        public ServerTransaction(int fromClientId, int toClientId, int fileId, double value)
        {
            FromClientId = fromClientId;
            ToClientId = toClientId;
            FileId = fileId;
            Value = value;
            DateTime = DateTime.Now;
        }
    }
}
