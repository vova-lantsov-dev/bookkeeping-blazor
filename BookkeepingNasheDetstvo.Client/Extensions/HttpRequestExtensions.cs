using Microsoft.JSInterop;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace BookkeepingNasheDetstvo.Client.Extensions
{
    public static class HttpRequestExtensions
    {
        public static async Task<(HttpStatusCode StatusCode, T Content)> GetAsync<T>(this HttpClient http, string uri, string authToken = null)
        {
            if (uri == null)
                return default;

            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            if (authToken != null)
            {
                message.Headers.TryAddWithoutValidation("Auth-Token", authToken);
            }

            try
            {
                var response = await http.SendAsync(message);
                return (response.StatusCode, response.StatusCode == HttpStatusCode.OK ? Json.Deserialize<T>(await response.Content.ReadAsStringAsync()) : default);
            }
            catch
            {
                return (HttpStatusCode.InternalServerError, default);
            }
        }

        public static async Task<(HttpStatusCode StatusCode, T Content)> PostAsync<T>(this HttpClient http, string uri, object objectToSend, string authToken = null)
        {
            if (uri == null || objectToSend == null)
                return default;

            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            var content = new StringContent(Json.Serialize(objectToSend), System.Text.Encoding.UTF8, "application/json");
            if (authToken != null)
            {
                content.Headers.TryAddWithoutValidation("Auth-Token", authToken);
            }
            message.Content = content;

            try
            {
                var response = await http.SendAsync(message);
                return (response.StatusCode, response.StatusCode == HttpStatusCode.OK ? Json.Deserialize<T>(await response.Content.ReadAsStringAsync()) : default);
            }
            catch
            {
                return (HttpStatusCode.InternalServerError, default);
            }
        }
    }
}
