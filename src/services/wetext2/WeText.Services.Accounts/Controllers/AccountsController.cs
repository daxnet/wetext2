// ----------------------------------------------------------------------------
//       ___ ___  ___     ___
// |  | |__   |  |__  \_/  |
// |/\| |___  |  |___ / \  |
//
// Yet another WeText application for demonstration.
// MIT License
//
// Copyright (c) 2018 Sunny Chen (daxnet)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// ----------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeText.Common;
using WeText.Services.Accounts.Models;

namespace WeText.Services.Accounts.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly ICrudProvider crudProvider;
        private readonly ILogger logger;

        public AccountsController(ICrudProvider crudProvider, ILogger<AccountsController> logger)
        {
            this.crudProvider = crudProvider;
            this.logger = logger;
        }

        [HttpPost]
        [Route("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] dynamic authenticationModel)
        {
            var userName = (string)authenticationModel.userName;
            var password = (string)authenticationModel.password;
            if (string.IsNullOrEmpty(userName) ||
                string.IsNullOrEmpty(password))
            {
                return BadRequest($"Either user name or password is not specified.");
            }

            var entity = (await this.crudProvider.FindBySpecificationAsync<Account>(x =>
                x.Name.Equals(userName))).FirstOrDefault();
            if (entity == null)
            {
                return NotFound();
            }

            if (!entity.AuthenticateWith(password))
            {
                return Unauthorized();
            }

            logger.LogInformation($"'{userName}' authenticated successfully.");

            return Ok(new
            {
                id = entity.Id,
                name = entity.Name,
                token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userName}:{password}"))
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var account = await crudProvider.GetByIdAsync<Account>(id);
            if (account == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                account.Id,
                account.Name,
                account.DisplayName,
                account.Email
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] Account account)
        {
            if (account == null)
            {
                return BadRequest("Account information not specified.");
            }

            if (string.IsNullOrEmpty(account.Name))
            {
                return BadRequest("Account name must be specified.");
            }

            if (string.IsNullOrEmpty(account.DisplayName))
            {
                return BadRequest("DisplayName must be specified.");
            }

            if (string.IsNullOrEmpty(account.Password))
            {
                return BadRequest("Password must be specified");
            }

            if (string.IsNullOrEmpty(account.Email))
            {
                return BadRequest("Email must be specified");
            }

            var accountWithSameName = (await crudProvider
                .FindBySpecificationAsync<Account>(act => act.Name.Equals(account.Name))).FirstOrDefault();
            if (accountWithSameName != null)
            {
                return Conflict($"Account name '{account.Name}' already exists.");
            }

            var accountWithSameEmail = (await crudProvider
                .FindBySpecificationAsync<Account>(act => act.Email.Equals(account.Email))).FirstOrDefault();
            if (accountWithSameEmail != null)
            {
                return Conflict($"Account email '{account.Email}' already exists.");
            }

            account.Password = Utils.EncryptPassword(account.Password, account.Name);
            await crudProvider.AddAsync(account);

            return Created(Url.Action("GetByIdAsync", new { id = account.Id }), account.Id);
        }
    }
}