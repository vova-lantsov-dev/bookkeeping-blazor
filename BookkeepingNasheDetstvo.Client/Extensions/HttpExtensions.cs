using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BookkeepingNasheDetstvo.Client.Extensions
{
    public static class HttpExtensions
    {
        public static async Task<T> GetAsync<T>(this HttpClient http, IUriHelper uriHelper, string uri, IReadOnlyDictionary<string, string> parameters = null, string accessToken = null)
        {
            if (uri == null)
                return default;

            if (parameters != null && parameters.Count > 0)
                uri += $"?{string.Join("&", parameters.Select(p => $"{p.Key}={p.Value}"))}";

            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            if (accessToken != null) message.Headers.TryAddWithoutValidation("Auth-Token", accessToken);

            try
            {
                var response = await http.SendAsync(message);
                await uriHelper.RedirectIfNotAuthorizedAsync(response.StatusCode);
                return await response.ReadJsonContentAsync<T>();
            }
            catch
            {
                return default;
            }
        }

        public static async Task<T> PostAsync<T>(this HttpClient http, IUriHelper uriHelper, string uri, object objectToSend, string accessToken = null)
        {
            if (uri == null || objectToSend == null)
                return default;

            System.Console.WriteLine(uri);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            var content = new StringContent(Json.Serialize(objectToSend), Encoding.UTF8, "application/json");
            if (accessToken != null) content.Headers.TryAddWithoutValidation("Auth-Token", accessToken);
            message.Content = content;

            try
            {
                var response = await http.SendAsync(message);
                await uriHelper.RedirectIfNotAuthorizedAsync(response.StatusCode);
                return await response.ReadJsonContentAsync<T>();
            }
            catch
            {
                return default;
            }
        }

        private static async Task RedirectIfNotAuthorizedAsync(this IUriHelper uriHelper, HttpStatusCode statusCode)
        {
            if (statusCode != HttpStatusCode.Unauthorized)
                return;

            await BrowserExtensions.RemoveLocalStorageAsync(Defaults.AuthTokenStorageName);
            uriHelper.NavigateTo("/authorize");
        }

        private static async Task<T> ReadJsonContentAsync<T>(this HttpResponseMessage response)
        {
            return response.StatusCode != HttpStatusCode.OK || typeof(T) == typeof(object)
                ? default
                : Json.Deserialize<T>(await response.Content.ReadAsStringAsync());
        }
    }
}
