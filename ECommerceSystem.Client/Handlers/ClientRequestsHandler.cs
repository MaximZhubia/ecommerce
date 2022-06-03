using ECommerceSystem.Client.Helpers;
using ECommerceSystem.Common.Entities;
using ECommerceSystem.Common.Models;
using ECommerceSystem.Security;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerceSystem.Client.Handlers
{
    public class ClientRequestsHandler
    {
        private const string WalletsDirectory = "Wallets";
        private const string FilesDirectory = "Files";

        public string Server { get; private set; }
        public int UserId { get; private set; }
        public string Username { get; private set; }
        public string AuthToken { get; private set; }
        public bool Authenticated { get; private set; }

        public ClientRequestsHandler(string server)
        {
            Server = server;
        }

        public bool Authenticate(string username, string password, out string content)
        {
            string passwordHash = Hash.SHA512TripleHash(password);
            AuthenticateRequest authRequest = new AuthenticateRequest(username, passwordHash);
            IRestResponse response = HttpRequests.Post(Server, "/Users/Authenticate", JsonConvert.SerializeObject(authRequest), out content);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var authResponse = JsonConvert.DeserializeObject<AuthenticateResponse>(response.Content);
                UserId = authResponse.Id;
                Username = authResponse.Username;
                AuthToken = authResponse.Token;
                Authenticated = true;

                return true;
            }

            return false;
        }

        public void CreateUser(string username, string password, double money = 0)
        {
            string passwordHash = Hash.SHA512TripleHash(password);
            User user = new User(username, passwordHash, money);
            IRestResponse response = HttpRequests.Post(Server, "/Users/Add", JsonConvert.SerializeObject(user), out string content);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(content);
            }
        }

        public void UploadFile(string filePath, double price)
        {
            if (!Authenticated)
            {
                throw new Exception("Forbidden");
            }

            string fileName = Path.GetFileName(filePath);
            byte[] fileContentBytes = System.IO.File.ReadAllBytes(filePath);

            UploadFileRequest uploadFileRequest = new UploadFileRequest(fileName, price, UserId, fileContentBytes);
            IRestResponse response = HttpRequests.Post(Server, "/Files/Upload", JsonConvert.SerializeObject(uploadFileRequest), out string content, AuthToken);

            if (response.StatusCode != HttpStatusCode.Created)
            {
                throw new Exception(content);
            }
        }

        public IEnumerable<ResponseFile> GetFiles()
        {
            if (!Authenticated)
            {
                throw new Exception("Forbidden");
            }

            IEnumerable<ResponseFile> result = null;
            IRestResponse response = HttpRequests.Get(Server, "/Files", out string content, AuthToken);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                result = JsonConvert.DeserializeObject<IEnumerable<ResponseFile>>(content);
            }

            return result;
        }

        public void CreateWallet()
        {
            if (!Authenticated)
            {
                throw new Exception("Forbidden");
            }

            CreateWalletRequest request = new CreateWalletRequest(UserId);
            IRestResponse response = HttpRequests.Post(Server, "/Users/CreateWallet", JsonConvert.SerializeObject(request), out string content);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                if (!Directory.Exists(WalletsDirectory))
                {
                    Directory.CreateDirectory(WalletsDirectory);
                }

                CreateWalletResponse createWalletResponse = JsonConvert.DeserializeObject<CreateWalletResponse>(content);
                string fileName = createWalletResponse.Wallet.Id + "-" + UserId + "-" + createWalletResponse.Pvv + ".wlt";
                System.IO.File.WriteAllText(Path.Combine(WalletsDirectory, fileName), JsonConvert.SerializeObject(createWalletResponse.Wallet));
            }
        }

        public bool BuyFile(int fileId, Wallet wallet, out string reason)
        {
            if (!Authenticated)
            {
                throw new Exception("Forbidden");
            }

            reason = null;
            BuyFileRequest request = new BuyFileRequest(fileId, wallet);
            IRestResponse response = HttpRequests.Post(Server, "/Files/Buy", JsonConvert.SerializeObject(request), out string content, AuthToken);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                if (!Directory.Exists(FilesDirectory))
                {
                    Directory.CreateDirectory(FilesDirectory);
                }

                BuyFileResponse buyFileResponse = JsonConvert.DeserializeObject<BuyFileResponse>(content);
                System.IO.File.WriteAllText(Path.Combine(FilesDirectory, buyFileResponse.File.Name), Encoding.ASCII.GetString(buyFileResponse.ContentBytes));

                string fromWalletFileName = string.Empty;
                string toWalletFileName = string.Empty;
                FileInfo[] walletFiles = new DirectoryInfo(WalletsDirectory).GetFiles("*.wlt");

                foreach (FileInfo fileInfo in walletFiles)
                {
                    if (fileInfo.Name.Split('-')[0] == buyFileResponse.FromWallet.Id.ToString())
                    {
                        fromWalletFileName = fileInfo.Name;
                    }

                    if (fileInfo.Name.Split('-')[0] == buyFileResponse.ToWallet.Id.ToString())
                    {
                        toWalletFileName = fileInfo.Name;
                    }
                }

                System.IO.File.WriteAllText(Path.Combine(WalletsDirectory, fromWalletFileName), JsonConvert.SerializeObject(buyFileResponse.FromWallet));
                Thread.Sleep(2000);
                System.IO.File.WriteAllText(Path.Combine(WalletsDirectory, toWalletFileName), JsonConvert.SerializeObject(buyFileResponse.ToWallet));

                reason = Path.Combine(FilesDirectory, buyFileResponse.File.Name);
                return true;
            }
            else
            {
                reason = content;
                return false;
            }
        }
    }
}
