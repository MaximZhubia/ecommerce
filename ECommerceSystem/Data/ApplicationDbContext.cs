using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECommerceSystem.Common.Entities;
using ECommerceSystem.Entities;
using ECommerceSystem.Security;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ECommerceSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<ClientTransaction> ClientTransactions { get; set; }
        public DbSet<ServerTransaction> ServerTransactions { get; set; }

        public ApplicationDbContext (DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public User AddUser(string username, string passwordHash, double money)
        {
            Users.Add(new User(username, passwordHash, money));
            SaveChanges();

            return Users.SingleOrDefault(x => x.Username == username);
        }

        public File AddFile(string name, string path, double price, int ownerId)
        {
            File file = new File(name, path, price, ownerId);
            Files.Add(file);
            SaveChanges();

            return Files.SingleOrDefault(x => x.Name == name);
        }

        public Wallet CreateWallet(int userId, string pvvHash)
        {
            Wallet wallet = new Wallet(userId, pvvHash);
            Wallets.Add(wallet);
            SaveChanges();

            Wallet createdWallet = Wallets.SingleOrDefault(x => x.UserId == userId);
            ClientTransaction clientTransaction = new ClientTransaction(userId, 1800, 0);
            clientTransaction.WalletId = createdWallet.Id;
            AddClientTransaction(clientTransaction);
            AddServerTransaction(new ServerTransaction(-1, userId, -1, 1800));
            object[] walletFields = new object[] { createdWallet.Id, createdWallet.UserId, createdWallet.PvvHash, createdWallet.ClientTransactions };
            createdWallet.Hash = Hash.SHA512TripleHash(JsonConvert.SerializeObject(walletFields));
            SaveChanges();

            return createdWallet;
        }

        public void AddServerTransaction(ServerTransaction serverTransaction)
        {
            User fileOwner = Users.SingleOrDefault(x => x.Files.Contains(Files.SingleOrDefault(y => y.Id == serverTransaction.FileId)));
            //fileOwner.Money += serverTransaction.Value;

            ServerTransactions.Add(serverTransaction);
            SaveChanges();
        }

        public void AddClientTransaction(ClientTransaction clientTransaction)
        {
            if (ClientTransactions != null && ClientTransactions.Local.Count != 0)
            {
                ClientTransaction lastTransaction =
                                ClientTransactions.ToList().Where(x => x.UserId == clientTransaction.UserId).Last();

                if (lastTransaction != null)
                {
                    clientTransaction.StartValue = lastTransaction.FinishValue;
                }
            }
            else
            {
                clientTransaction.StartValue = 0;
            }

            if (clientTransaction.IncomeValue == 0 && clientTransaction.OutcomeValue != 0)
            {
                clientTransaction.FinishValue = clientTransaction.StartValue - clientTransaction.OutcomeValue;
            }
            else if(clientTransaction.IncomeValue != 0 && clientTransaction.OutcomeValue == 0)
            {
                clientTransaction.FinishValue = clientTransaction.StartValue + clientTransaction.IncomeValue;
            }

            ClientTransactions.Add(clientTransaction);
            SaveChanges();
        }
    }
}
