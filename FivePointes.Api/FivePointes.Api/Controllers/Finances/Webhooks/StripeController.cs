using FivePointes.Api.Configuration;
using FivePointes.Common;
using FivePointes.Logic.Models.Filters;
using FivePointes.Logic.Ports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace FivePointes.Api.Controllers.Finances.Webhooks
{
    [ApiExplorerSettings(GroupName = "finances")]
    [Route("webhooks/[controller]")]
    [ApiController]
    public class StripeController : ControllerBase
    {
        private readonly ITransactionsService _transactionsService;
        private readonly ITransactionCategoriesService _transactionCategoriesService;
        private readonly IAccountsService _accountsService;
        private readonly StripeOptions _stripeOptions;
        private readonly BalanceTransactionService _balanceTransactionService;
        private readonly CustomerService _customerService;

        public StripeController(ITransactionsService transactionsService, ITransactionCategoriesService transactionCategoriesService, IAccountsService accountsService, IOptions<StripeOptions> stripeOptions)
        {
            _transactionsService = transactionsService ?? throw new ArgumentNullException(nameof(transactionsService));
            _transactionCategoriesService = transactionCategoriesService ?? throw new ArgumentNullException(nameof(transactionCategoriesService));
            _accountsService = accountsService ?? throw new ArgumentNullException(nameof(accountsService));
            _stripeOptions = stripeOptions?.Value ?? throw new ArgumentNullException(nameof(stripeOptions));

            _balanceTransactionService = new BalanceTransactionService();
            _customerService = new CustomerService();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> HandleWebook([FromQuery]string accountId)
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var secret = "";
            try
            {
                var stripeEvent = EventUtility.ParseEvent(json);
                var signatureHeader = Request.Headers["Stripe-Signature"];

                if(accountId == null)
                {
                    StripeConfiguration.ApiKey = _stripeOptions.ApiKeys.First().Value;
                    secret = "No Account: " + _stripeOptions.WebhookSecrets.First().Value;
                    stripeEvent = EventUtility.ConstructEvent(json, signatureHeader, _stripeOptions.WebhookSecrets.First().Value);
                }
                else
                {
                    StripeConfiguration.ApiKey = _stripeOptions.ApiKeys[accountId];
                    secret = _stripeOptions.WebhookSecrets[accountId];
                    stripeEvent = EventUtility.ConstructEvent(json, signatureHeader, _stripeOptions.WebhookSecrets[accountId]);
                }

                switch (stripeEvent.Type)
                {
                    case Events.ChargeSucceeded:
                        var charge = stripeEvent.Data.Object as Charge;
                        if(stripeEvent.Livemode)
                        {
                            charge.BalanceTransaction = await _balanceTransactionService.GetAsync(charge.BalanceTransactionId);

                            if(charge.CustomerId != null)
                            {
                                charge.Customer = await _customerService.GetAsync(charge.CustomerId);
                            }
                        }
                        else
                        {
                            charge.StatementDescriptor = "TEST ACCOUNT";

                            charge.BalanceTransaction = new BalanceTransaction
                            {
                                Amount = 100,
                                Fee = 5
                            };

                            charge.Customer = new Customer
                            {
                                Name = "Test Customer"
                            };
                        }

                        var allCategoriesResult = await _transactionCategoriesService.GetAsync(new TransactionCategoryFilter());
                        if (!allCategoriesResult.IsSuccessful())
                        {
                            return new ErrorActionResult(allCategoriesResult);
                        }

                        var incomeCategory = allCategoriesResult.Value.FirstOrDefault(x => x.TransactionType == Logic.Models.TransactionType.Income);
                        var processingFeeCategory = allCategoriesResult.Value.FirstOrDefault(x => x.TransactionType == Logic.Models.TransactionType.Expense && x.Name.Contains("Process"));

                        var allAccounts = await _accountsService.GetAsync(new AccountFilter());
                        if (!allAccounts.IsSuccessful())
                        {
                            return new ErrorActionResult(allAccounts);
                        }

                        var description = charge.Customer != null ? charge.Customer.Name + " - " + charge.Description : charge.Description;

                        var bizAccount = allAccounts.Value.FirstOrDefault(x => x.Name.Contains("Business"));

                        var existingChargeResult = await _transactionsService.GetAsync("StripeCharge", charge.Id);
                        if(!existingChargeResult.IsSuccessful() && existingChargeResult.Status != HttpStatusCode.NotFound)
                        {
                            return new ErrorActionResult(existingChargeResult);
                        }

                        if (!existingChargeResult.IsSuccessful())
                        {
                            await _transactionsService.CreateAsync(new Logic.Models.Transaction
                            {
                                Amount = charge.BalanceTransaction.Amount / 100.0M,
                                Date = NodaTime.LocalDate.FromDateTime(charge.Created),
                                Description = $"{charge.CalculatedStatementDescriptor} - {description}",
                                IsCleared = false,
                                Category = incomeCategory,
                                Account = bizAccount,
                                Source = "StripeCharge",
                                SourceId = charge.Id
                            });
                        }

                        var existingChargeFeeResult = await _transactionsService.GetAsync("StripeChargeFee", charge.Id);
                        if (!existingChargeFeeResult.IsSuccessful() && existingChargeFeeResult.Status != HttpStatusCode.NotFound)
                        {
                            return new ErrorActionResult(existingChargeFeeResult);
                        }

                        if (!existingChargeResult.IsSuccessful())
                        {
                            await _transactionsService.CreateAsync(new Logic.Models.Transaction
                            {
                                Amount = -(charge.BalanceTransaction.Fee / 100.0M),
                                Date = NodaTime.LocalDate.FromDateTime(charge.Created),
                                Description = $"{charge.CalculatedStatementDescriptor} - {description} - Stripe Fees",
                                IsCleared = false,
                                Category = processingFeeCategory,
                                Account = bizAccount,
                                Source = "StripeChargeFee",
                                SourceId = charge.Id
                            });
                        }
                        break;
                    case Events.ChargeRefunded:
                        var refund = stripeEvent.Data.Object as Refund;
                        // TODO
                        return new ErrorActionResult(Result.Error(HttpStatusCode.NotImplemented, "Refunds not yet implemented"));
                }

                return Ok();
            }
            catch (Exception e)
            {
                return new ErrorActionResult(Result.Error(HttpStatusCode.InternalServerError, e.Message + " " + StripeConfiguration.ApiKey + " " + secret));
            }
        }
    }
}
