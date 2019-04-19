using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BookkeepingNasheDetstvo.Client.Extensions;
using BookkeepingNasheDetstvo.Client.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BookkeepingNasheDetstvo.Client.Components
{
    public class BaseComponent : ComponentBase
    {
        protected TeacherModel Current { get; set; }
        protected string AccessToken { get; set; }
        protected bool IsOk { get; set; } = true;

        [Inject]
        protected HttpClient Http { get; set; }
        [Inject]
        protected IUriHelper UriHelper { get; set; }

        protected async Task CheckAccessToken()
        {
            try
            {
                AccessToken = await BrowserExtensions.ReadLocalStorageAsync(Defaults.AuthTokenStorageName);
            }
            catch
            {
                // ignored
            }

            if (string.IsNullOrEmpty(AccessToken))
                UriHelper.NavigateTo("/authorize");
        }
        protected async Task LoadCurrent()
        {
            Current = await Get<TeacherModel>("/api/current");
            if (Current == default)
                UriHelper.NavigateTo("/authorize");
        }

        protected async Task<bool> Post(string uri, object objectToSend, bool token = true)
        {
            if (objectToSend == null || uri == null)
                return false;

            if (token && AccessToken == null)
                UriHelper.NavigateTo("/authorize");

            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            var content = new StringContent(Json.Serialize(objectToSend), System.Text.Encoding.UTF8, "application/json");
            if (token) content.Headers.TryAddWithoutValidation("Auth-Token", AccessToken);
            message.Content = content;
            try
            {
                var response = await Http.SendAsync(message);
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    AccessToken = null;
                    await BrowserExtensions.RemoveLocalStorageAsync(Defaults.AuthTokenStorageName);
                    UriHelper.NavigateTo("/authorize");
                }
                return response.StatusCode == HttpStatusCode.OK;
            }
            catch
            {
                return false;
            }
        }
        protected async Task<T> Post<T>(string uri, object objectToSend, bool token = true)
        {
            if (uri == null || objectToSend == null)
                return default;

            if (token && AccessToken == null)
                UriHelper.NavigateTo("/authorize");

            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            var content = new StringContent(Json.Serialize(objectToSend), System.Text.Encoding.UTF8, "application/json");
            if (token) content.Headers.TryAddWithoutValidation("Auth-Token", AccessToken);
            message.Content = content;
            try
            {
                var response = await Http.SendAsync(message);
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    AccessToken = null;
                    await BrowserExtensions.RemoveLocalStorageAsync(Defaults.AuthTokenStorageName);
                    UriHelper.NavigateTo("/authorize");
                }
                if (response.StatusCode != HttpStatusCode.OK)
                    return default;
                
                return Json.Deserialize<T>(await response.Content.ReadAsStringAsync());
            }
            catch
            {
                return default;
            }
        }
        protected async Task<T> Get<T>(string uri, bool token = true)
        {
            if (uri == null)
                return default;

            if (token && AccessToken == null)
                UriHelper.NavigateTo("/authorize");

            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            if (token) message.Headers.TryAddWithoutValidation("Auth-Token", AccessToken);
            try
            {
                var response = await Http.SendAsync(message);
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    AccessToken = null;
                    await BrowserExtensions.RemoveLocalStorageAsync(Defaults.AuthTokenStorageName);
                    UriHelper.NavigateTo("/authorize");
                }
                if (response.StatusCode != HttpStatusCode.OK)
                    return default;

                return Json.Deserialize<T>(await response.Content.ReadAsStringAsync());
            }
            catch
            {
                return default;
            }
        }
    }
}
