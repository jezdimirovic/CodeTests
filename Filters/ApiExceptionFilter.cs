using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebPrefer.Tests.Models;

namespace WebPrefer.Tests.Filters
{
    public class ApiExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var transactionResponse = new TransactionResponse();

            transactionResponse.ErrorCode = context.Exception.Message;

            context.Result = new ObjectResult(transactionResponse);
        }
    }
}
