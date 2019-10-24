using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BookkeepingNasheDetstvo.Client.Extensions;
using BookkeepingNasheDetstvo.Client.Models.ForTeacher;
using Microsoft.AspNetCore.Components;

namespace BookkeepingNasheDetstvo.Client.Components
{
    public class SessionController : ComponentBase
    {
        private protected TeacherModel Current { get; set; }
        private string AccessToken { get; set; }
        private protected bool IsOk { get; set; } = true;

        [Inject]
        private HttpClient Http { get; set; }
        [Inject]
        private IUriHelper UriHelper { get; set; }

        private protected async Task CheckAccessToken()
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

        private protected async Task LoadCurrent()
        {
            Current = await GetAsync<TeacherModel>("/api/current");
            if (Current == default)
                UriHelper.NavigateTo("/authorize");
        }

        private protected void NavigateTo(string uri)
        {
            UriHelper.NavigateTo(uri);
        }

        private protected Task<T> GetAsync<T>(string uri, IReadOnlyDictionary<string, string> parameters = null)
        {
            return Http.GetAsync<T>(UriHelper, uri, parameters, AccessToken);
        }

        private protected Task<T> PostAsync<T>(string uri, object objectToSend)
        {
            return Http.PostAsync<T>(UriHelper, uri, objectToSend, AccessToken);
        }
    }
}
