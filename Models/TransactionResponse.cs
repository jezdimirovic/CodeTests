using System;

namespace WebPrefer.Tests.Models
{
    public class TransactionResponse
    {
        /// <summary>
        /// Unique transaction identifier, can use internal representation, only needed if ErrorCode = NoError
        /// </summary>
        public string TransactionId { get; set; }

        /// <summary>
        /// Players balance
        /// </summary>
        public decimal Balance { get; set; }

        /// <summary>
        /// Can be any of NoError, InsufficentFunds, InvalidCurrency, ResponsibleGamblingLimitsMet, UnkownError
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// Can be set to a custom message in case of UnkownError
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}