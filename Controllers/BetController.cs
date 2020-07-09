using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebPrefer.Tests.BL;

namespace WebPrefer.Tests.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BetController : ControllerBase
    {
        ICasinoService _casinoService;
        IWalletService _walletService;

        public BetController(ICasinoService casinoService, IWalletService walletService)
        {
            _casinoService = casinoService;
            _walletService = walletService;
        }

        [HttpPost]
        public async Task<Models.TransactionResponse> Post([FromBody] Models.BetRequest value)
        {
            var transactionId = await _casinoService.Wager(value.PlayerId, value.Game, value.TransactionId, value.RoundId, new Money(value.Currency, value.Amount));
            var balance = await _walletService.GetBalance(value.PlayerId, value.Currency);

            return await Task.FromResult(new Models.TransactionResponse
            {
                TransactionId = transactionId.ToString(),
                Balance = balance.Amount,
                ErrorCode = "NoError",
                ErrorMessage = string.Empty
            });
        }
    }
}