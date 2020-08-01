using System;
using System.Net;
using System.Threading.Tasks;
using Ets2SyncWebApi.Responses;
using RestSharp;

namespace Ets2SyncWebApi
{
    internal static class RestClientExtensions
    {
        public static async Task<IRestResponse<T>> ProcessRequest<T>(this RestClient client, Task<IRestResponse<T>> executeTask)
        {
            IRestResponse<T> response = await executeTask;

            if (response.ErrorException != null)
                throw response.ErrorException;

            return response;
        }
    }
}