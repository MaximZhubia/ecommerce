using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerceSystem.Common.Entities;
using ECommerceSystem.Data;
using ECommerceSystem.Interfaces;
using ECommerceSystem.Common.Models;
using Newtonsoft.Json;
using ECommerceSystem.Security;
using ECommerceSystem.Entities;

namespace ECommerceSystem.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class FilesController : ControllerBase
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IFileService _fileService;

        public FilesController(ApplicationDbContext applicationDbContext, IFileService fileService)
        {
            _applicationDbContext = applicationDbContext;
            _fileService = fileService;
        }

        // GET: Files
        [HttpGet]
        public IEnumerable<ResponseFile> GetFiles()
        {
            return _fileService.GetFiles();
        }

        // GET: Files/Name
        [HttpGet("{id}")]
        public ActionResult<File> GetFile(int id)
        {
            var file = _fileService.GetFile(id);

            if (file == null)
            {
                return NotFound();
            }

            return file;
        }

        // POST: Files/Upload
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost("Upload")]
        public ActionResult<File> PostFile(UploadFileRequest uploadFileRequest)
        {
            File uploadedFile = null;

            try
            {
                uploadedFile = _fileService.Upload(uploadFileRequest.Name, uploadFileRequest.Price,
                uploadFileRequest.OwnerId, uploadFileRequest.FileContentBytes);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }

            return Created(uploadedFile.Path, uploadedFile);
        }

        [HttpPost("Buy")]
        public ActionResult<BuyFileResponse> BuyFile(BuyFileRequest request)
        {
            if (!ValidateWallet(request.Wallet))
            {
                return Unauthorized("Wallet is incorrect");
            }

            double filePrice = _applicationDbContext.Files.SingleOrDefault(x => x.Id == request.FileId).Price;

            if (!CheckWalletValue(request.Wallet, filePrice))
            {
                return Unauthorized("Not enough money");
            }

            BuyFileResponse buyFileResponse;

            try
            {
                File file = _fileService.GetFile(request.FileId);
                buyFileResponse = new BuyFileResponse(file, System.IO.File.ReadAllBytes(file.Path));
                CreateServerTransaction(request.Wallet.UserId, file.UserId, file.Id, file.Price);
                CreateClientTransactions(request.Wallet.UserId, file.UserId, file.Price);

                Wallet fromWallet = _applicationDbContext.Wallets.SingleOrDefault(x => x.Id == request.Wallet.Id);
                object[] fromWalletFields = new object[] { fromWallet.Id, fromWallet.UserId, fromWallet.PvvHash, fromWallet.ClientTransactions };
                fromWallet.Hash = Hash.SHA512TripleHash(JsonConvert.SerializeObject(fromWalletFields));
                buyFileResponse.FromWallet = fromWallet;
                
                Wallet toWallet = _applicationDbContext.Wallets.SingleOrDefault(x => x.UserId == file.UserId);
                object[] toWalletFields = new object[] { toWallet.Id, toWallet.UserId, toWallet.PvvHash, toWallet.ClientTransactions };
                toWallet.Hash = Hash.SHA512TripleHash(JsonConvert.SerializeObject(toWalletFields));
                buyFileResponse.ToWallet = toWallet;

                _applicationDbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }

            return Ok(buyFileResponse);
        }

        private bool ValidateWallet(Wallet wallet)
        {
            Wallet originalWaller = _applicationDbContext.Wallets.SingleOrDefault(x => x.Id == wallet.Id);
            originalWaller.ClientTransactions = _applicationDbContext.ClientTransactions.Where(x => x.WalletId == originalWaller.Id).ToList();
            object[] originalWalletFields = new object[] { wallet.Id, wallet.UserId, wallet.PvvHash, wallet.ClientTransactions };
            string originalWalletHash = Hash.SHA512TripleHash(JsonConvert.SerializeObject(originalWalletFields));

            if (originalWaller != null && originalWalletHash == wallet.Hash)
            {
                return true;
            }

            return false;
        }

        private bool CheckWalletValue(Wallet wallet, double value)
        {
            if (_applicationDbContext.ClientTransactions == null || _applicationDbContext.ClientTransactions.ToList().Count == 0)
            {
                return false;
            }

            ClientTransaction lastTransaction =
                _applicationDbContext.ClientTransactions.ToList().Where(x => x.UserId == wallet.UserId).Last();

            if (lastTransaction == null)
            {
                return false;
            }

            if (lastTransaction.FinishValue >= value)
            {
                return true;
            }

            return false;
        }

        private void CreateServerTransaction(int fromClientId, int toClientId, int fileId, double value)
        {
            ServerTransaction serverTransaction = new ServerTransaction(fromClientId, toClientId, fileId, value);
            _applicationDbContext.AddServerTransaction(serverTransaction);
        }

        private void CreateClientTransactions(int fromClientId, int toClientId, double value)
        {
            ClientTransaction fromClientTransaction =
                new ClientTransaction(fromClientId, 0, value);
            fromClientTransaction.WalletId = _applicationDbContext.Wallets.SingleOrDefault(x => x.UserId == fromClientId).Id;
            ClientTransaction toClientTransaction =
                new ClientTransaction(toClientId, value, 0);
            toClientTransaction.WalletId = _applicationDbContext.Wallets.SingleOrDefault(x => x.UserId == toClientId).Id;

            _applicationDbContext.AddClientTransaction(fromClientTransaction);
            _applicationDbContext.AddClientTransaction(toClientTransaction);
        }

        //private bool FileExists(int id)
        //{
        //    return _context.Files.Any(e => e.Id == id);
        //}
    }
}
