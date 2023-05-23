using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using ServicesLibrary;
using System.Security.Claims;

namespace BankWebAppApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [EnableCors("AllowAll")]
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly IAccountService _accountService;

        public CustomerController(ICustomerService customerService, IAccountService accountService)
        {
            _customerService = customerService;
            _accountService = accountService;
        }

        /// <summary>
        /// Retrieves client information from the database for the currently authorized user.
        /// </summary>
        /// <returns>
        /// The client information for the current user.
        /// </returns>
        /// <remarks>
        /// This endpoint allows authorized users to retrieve their client information.
        /// The endpoint is accessed using the HTTP GET method and the URI "/client".
        /// </remarks>
        /// <response code="200">Returns the client information for the current user from the database.
        /// </response>
        [HttpGet]
        [Route("/client")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<Customer>> GetClient()
        {
            var userId = User.FindFirst(ClaimTypes.Authentication)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }
            var id = int.Parse(userId);
            var customer = await _customerService.GetCustomerAsync(id);
            if (customer == null)
            {
                return BadRequest("Client not found");
            }

            return Ok(customer);
        }

        /// <summary>
        /// Retrieves a list of transactions for the account with the specified account ID for the currently authorized user, with limits and offsets, from the database.
        /// </summary>
        /// <returns>
        /// A list of transactions for the account with the specified account ID and authorized user.
        /// </returns>
        /// <remarks>
        /// The endpoint for this API call is: GET /client-account-transactions/{accountId}/{limit}/{offset}
        /// </remarks>
        /// <response code="200">
        /// Returns a list of transactions for the account with the specified account ID and authorized user.
        /// </response>
        [HttpGet]
        [Route("/client-account-transactions/{accountId:int}/{limit:int}/{offset:int}")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<List<Transaction>>> GetClientTransactions(int accountId, int limit, int offset)
        {
            var userId = User.FindFirst(ClaimTypes.Authentication)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }
            var customerId = int.Parse(userId);
            var transactions = await _accountService.GetTransactionsAsync(customerId, accountId, limit, offset);
            if (transactions.Count == 0)
            {
                return BadRequest("Account not found");
            }
            return Ok(transactions);
        }

        /// <summary>
        /// Retrieves information for a client with the specified ID from the database. This endpoint is only accessible to admins.
        /// </summary>
        /// <returns>
        /// The client information from the database.
        /// </returns>
        /// <remarks>
        /// This endpoint can be accessed with the following URL: GET /client/{id}.
        /// </remarks>
        /// <response code="200">The client information was successfully retrieved from the database.
        /// </response>
        [HttpGet]
        [Route("/client/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Customer>> GetClient(int id)
        {

            var customer = await _customerService.GetCustomerAsync(id);
            if (customer == null)
            {
                return BadRequest("Client not found");
            }
            return Ok(customer);
        }

        /// <summary>
        /// Retrieves a list of transactions for the specified client account from the database, with a specified limit and offset. This endpoint is only accessible to admins.
        /// </summary>
        /// <returns>
        /// A list of transactions for the specified client account.
        /// </returns>
        /// <remarks>
        /// To retrieve the transactions for a specific client account, make a GET request to the following endpoint:
        /// /client-transactions/{customerId}/{accountId}/{limit}/{offset}
        /// The {customerId} parameter should be replaced with the ID of the customer whose account transactions you want to retrieve.
        /// The {accountId} parameter should be replaced with the ID of the account whose transactions you want to retrieve.
        /// The {limit} parameter should be replaced with the maximum number of transactions to retrieve.
        /// The {offset} parameter should be replaced with the number of transactions to skip before starting to retrieve results.
        /// If successful, the method returns a list of transactions for the specified client account. 
        /// </remarks>
        /// <response code="200">
        /// The list of transactions for the specified client account was successfully retrieved.
        /// </response>
        [HttpGet]
        [Route("/client-account-transactions/{customerId:int}/{accountId:int}/{limit:int}/{offset:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<Transaction>>> GetClientTransactions(int customerId,int accountId, int limit, int offset)
        {
            var transactions = await _accountService.GetTransactionsAsync(customerId, accountId, limit, offset);
            if (transactions == null)
            {
                return BadRequest("Client transactions not found");
            }
            return Ok(transactions);
        }

    }
}
