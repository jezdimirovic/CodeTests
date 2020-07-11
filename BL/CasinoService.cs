using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebPrefer.Tests.BL
{
    public interface ICasinoService
    {
        Task<long> Wager(int playerId, string externalGameId, string externalTransactionId, long externalRoundId, Money amount);
        Task<long> Win(int playerId, string externalGameId, string externalTransactionId, long externalRoundId, Money amount);
        Task EndRound(long externalRoundId);
    }

    public class CasinoService : ICasinoService
    {
        private IWalletService _walletService;

        private Dictionary<string, long> _processedTransactions = new Dictionary<string, long>();
        private Dictionary<long, (Money TotalWager, Money TotalWin, bool Ended)> _rounds = 
            new Dictionary<long, (Money TotalBet, Money TotalWin, bool Ended)>();

        private long _lastTransactionId = 0;

        public CasinoService(IWalletService walletService)
            => _walletService = walletService;

        public async Task<long> Wager(int playerId, string externalGameId, string externalTransactionId, long externalRoundId, Money amount)
        {
            if (_processedTransactions.ContainsKey(externalTransactionId))
            {
                throw new TransactionAlreadyProcessedException(_processedTransactions[externalTransactionId]);
            }

            if (!_rounds.ContainsKey(externalRoundId))
            {
                _rounds.Add(externalRoundId, (amount, new Money(amount.Currency), false));
            }
            else
            {
                var round = _rounds[externalRoundId];
                if (round.Ended)
                {
                    throw new RoundEndedException("Round ended");
                }

                round.TotalWager += amount;
            }

            var transactionId = ++_lastTransactionId;
            var playerBalance = await _walletService.GetBalance(playerId, amount.Currency);

            if (playerBalance.Amount < amount.Amount)
            {
                throw new InsufficientFundsException("InsufficentFunds");
            }

            await _walletService.Debit(playerId, amount);

            return transactionId;
        }

        public async Task<long> Win(int playerId, string externalGameId, string externalTransactionId, long externalRoundId, Money amount)
        {
            if (_processedTransactions.ContainsKey(externalTransactionId))
            {
                throw new TransactionAlreadyProcessedException(_processedTransactions[externalTransactionId]);
            }

            if (!_rounds.TryGetValue(externalRoundId, out var round))
            {
                throw new MissingRoundException();
            }

            if (round.Ended)
            {
                throw new RoundEndedException("Round ended");
            }

            if (!await _walletService.HasWallet(playerId, amount.Currency))
            {
                throw new InvalidCurrencyException("InvalidCurrency");
            }

            round.TotalWin += amount;

            var transactionId = ++_lastTransactionId;

            await _walletService.Credit(playerId, amount);

            return transactionId;
        }

        public Task EndRound(long externalRoundId)
        {
            if (!_rounds.TryGetValue(externalRoundId, out var round))
            {
                throw new MissingRoundException();
            }

            _rounds[externalRoundId] = (_rounds[externalRoundId].TotalWager, _rounds[externalRoundId].TotalWin, true);

            return Task.FromResult(0);
        }
    }

    public class TransactionAlreadyProcessedException : Exception
    {
        public long TransactionId { get; }

        public TransactionAlreadyProcessedException(long transactionId)
            => TransactionId = transactionId;
    }

    public class MissingRoundException : Exception { }

    public class RoundEndedException : Exception
    {
        public RoundEndedException(string message) : base(message)
        {

        }

        public RoundEndedException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }

    public class InsufficientFundsException : Exception
    {
        public string ErrorCode { get; }
        public InsufficientFundsException(string errorCode) : base(errorCode)
        {
            ErrorCode = errorCode;
        }
    }

    public class InvalidCurrencyException : Exception {
        public InvalidCurrencyException(string message) : base(message)
        {
        }

        public InvalidCurrencyException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}