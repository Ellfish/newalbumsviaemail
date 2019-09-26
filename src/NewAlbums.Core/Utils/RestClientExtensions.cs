using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NewAlbums.Utils
{
    public static class RestClientExtensions
    {
        /// <summary>
        /// https://stackoverflow.com/a/44163107
        /// </summary>
        public static async Task<RestResponse> ExecuteAsync(this RestClient client, RestRequest request)
        {
            var taskCompletion = new TaskCompletionSource<IRestResponse>();
            var handle = client.ExecuteAsync(request, r => taskCompletion.SetResult(r));
            return (RestResponse) await taskCompletion.Task;
        }
    }
}
