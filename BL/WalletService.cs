using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebPrefer.Tests.BL
{
    public interface IWalletService
    {
        Task<bool> HasWallet(int playerId, string currency);
        Task<Money> GetBalance(int playerId, string currency);
        Task<int> Debit(int playerId, Money amount);
        Task Credit(int playerId, Money amount);
    }

    public class WalletService : IWalletService
    {
        private Dictionary<int, Money> _wallets = new Dictionary<int, Money>()
        {
            { 1, new Money("SEK", 100m) },
            { 2, new Money("EUR", 250m) }
        };

        public WalletService()
        {
        }

        public Task<bool> HasWallet(int playerId, string currency)
            => Task.FromResult(_wallets.ContainsKey(playerId) && _wallets[playerId].Currency == currency);

        public Task<Money> GetBalance(int playerId, string currency)
            => Task.FromResult(_wallets[playerId]);

        public Task Credit(int playerId, Money amount)
        {
            _wallets[playerId] += amount;
            return Task.FromResult(0);
        }

        public async Task<int> Debit(int playerId, Money amount)
        {
            if (_wallets[playerId].Amount < amount.Amount)
            {
                throw new InsufficientFundsException("InsufficentFunds");
            }

            _wallets[playerId] -= amount;

            return await Task.FromResult(0);
        }
    }
}
