using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerceSystem.Data;
using ECommerceSystem.Interfaces;
using Microsoft.AspNetCore.Authorization;
using ECommerceSystem.Common.Models;
using ECommerceSystem.Common.Entities;
using System.Net;

namespace ECommerceSystem.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("Authenticate")]
        public IActionResult Authenticate(AuthenticateRequest model)
        {
            var response = _userService.Authenticate(model);

            if (response == null)
            {
                return Unauthorized("Username or password is incorrect.");
            }

            return Ok(response);
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            return Ok(users);
        }

        //[Authorize]
        [HttpPost("Add")]
        public IActionResult PostUser(User user)
        {
            User createdUser;

            try
            {
                createdUser = _userService.Add(user.Username, user.PasswordHash, user.Money);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }

            return Ok(createdUser);
        }

        [HttpPost("CreateWallet")]
        public IActionResult CreateUserWallet(CreateWalletRequest request)
        {
            CreateWalletResponse createWalletResponse;
            try
            {
                Wallet createdWallet = _userService.CreateWallet(request.UserId, out uint pvv);
                createWalletResponse = new CreateWalletResponse(pvv, createdWallet);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }

            return Ok(createWalletResponse);
        }
    }
}
