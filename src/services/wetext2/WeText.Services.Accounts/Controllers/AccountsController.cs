using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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