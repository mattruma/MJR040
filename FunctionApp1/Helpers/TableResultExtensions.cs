using Microsoft.Azure.Cosmos.Table;
using System.Net;
using System.Net.Http;

namespace FunctionApp1.Helpers
{
    internal static class TableResultExtensions
    {
        internal static void EnsureSuccessStatusCode(
            this TableResult tableResult)
        {
            switch (tableResult.HttpStatusCode)
            {
                case (int)HttpStatusCode.Created:
                case (int)HttpStatusCode.OK:
                case (int)HttpStatusCode.NoContent:
                    break;
                default:
                    throw new HttpRequestException(
                        $"Something went wrong in table operation, a {tableResult.HttpStatusCode} status code was returned.");
            }
        }
    }
}
