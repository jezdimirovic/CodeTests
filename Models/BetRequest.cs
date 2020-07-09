using System;

namespace WebPrefer.Tests.Models
{
    public class BetRequest
    {
        public int PlayerId { get; set; }

        /// <summary>
        /// Game identifier
        /// </summary>
        public string Game { get; set; }

        /// <summary>
        /// Unique transaction identifier
        /// </summary>
        public string TransactionId { get; set; }

        /// <summary>
        /// ISO currency code
        /// </summary>
        public string Currency { get; set; }

        public decimal Amount { get; set; }
        public long RoundId { get; set; }
    }
}