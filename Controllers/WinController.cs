using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebPrefer.Tests.BL;

namespace WebPrefer.Tests.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WinController : ControllerBase
    {
        ICasinoService _casinoService;
        IWalletService _walletService;

        public WinController(ICasinoService casinoService, IWalletService walletService)
        {
            _casinoService = casinoService;
            _walletService = walletService;
        }

        [HttpPost]
        public async Task<Models.TransactionResponse> Post([FromBody] Models.WinRequest value)
        {
            var transactionId = await _casinoService.Win(value.PlayerId, value.Game, value.TransactionId, value.RoundId, new Money(value.Currency, value.Amount));
            await _casinoService.EndRound(value.RoundId);
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