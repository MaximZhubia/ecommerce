using ECommerceSystem.Client.Handlers;
using ECommerceSystem.Client.Helpers;
using ECommerceSystem.Common.Entities;
using ECommerceSystem.Common.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace ECommerceSystem.Client
{
    class Program
    {
        const string BaseUrl = "https://localhost:44381";

        static void Main(string[] args)
        {
            ClientRequestsHandler client = new ClientRequestsHandler(BaseUrl);

            while (true)
            {
                Console.WriteLine("[AUTH] Enter username: ");

                string username = Console.ReadLine();
                //string username = "test";

                if (username == "reg")
                {
                    Console.WriteLine("[REG] Enter username for new user: ");
                    string newUsername = Console.ReadLine();
                    Console.WriteLine("[REG] Enter password for new user: ");
                    string newPassword = Console.ReadLine();

                    try
                    {
                        client.CreateUser(newUsername, newPassword);
                        Console.WriteLine("[REG] User " + newUsername + " created!");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
                else
                {
                    Console.WriteLine("[AUTH] Enter password: ");
                    string password = Console.ReadLine();
                    //string password = "user";

                    if (!client.Authenticate(username, password, out string content))
                    {
                        Console.WriteLine("[AUTH ERROR]: " + content);
                        continue;
                    }

                    Console.WriteLine("[AUTH] Authentication success!");

                    while (true)
                    {
                        var filesList = client.GetFiles();
                        PrintFilesList(filesList);

                        Console.WriteLine("[INFO] Enter the file number for downloading");
                        string command = Console.ReadLine();

                        if (command == "wlt")
                        {
                            client.CreateWallet();
                            Console.WriteLine("[INFO] Wallet was created");
                        }
                        else if (command == "upload")
                        {
                            client.UploadFile("README.txt", 1000);
                            Console.WriteLine("[INFO] File was uploaded");
                        }
                        else if (int.TryParse(command, out int fileId))
                        {
                            if (((List<ResponseFile>)filesList).Exists(x => x.Id == fileId))
                            {
                                Console.WriteLine("[INFO] Enter wallet file name (with pvv): ");
                                string walletFileName = Console.ReadLine();

                                Wallet wallet = JsonConvert.DeserializeObject<Wallet>(System.IO.File.ReadAllText("Wallets\\" + walletFileName + ".wlt"));
                                if (!client.BuyFile(fileId, wallet, out string reason))
                                {
                                    Console.WriteLine("[ERROR] " + reason);
                                }
                                else
                                {
                                    Console.WriteLine("[INFO] File path is \'" + reason + "\'");
                                }
                            }
                            else
                            {
                                Console.WriteLine("[ERROR] File not found.");
                            }
                        }
                        else if (command == "exit")
                        {
                            break;
                        }
                    }
                }
            }
        }

        static void PrintFilesList(IEnumerable<ResponseFile> files)
        {
            Console.WriteLine("========= List of files =========");

            foreach (ResponseFile file in files)
            {
                Console.WriteLine(file.Id + " - " + file.Name + " - " + file.Price + " - " + file.OwnerUsername);
            }

            Console.WriteLine("=================================");
        }
    }
}
