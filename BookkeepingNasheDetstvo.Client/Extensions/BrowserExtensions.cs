using Microsoft.AspNetCore.Blazor;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace BookkeepingNasheDetstvo.Client.Extensions
{
    public static class BrowserExtensions
    {
        public static Task<string> ReadLocalStorageAsync(string key)
        {
            return JSRuntime.Current.InvokeAsync<string>("Extensions.ReadStorage", key);
        }
        public static Task WriteLocalStorageAsync(string key, string value)
        {
            return JSRuntime.Current.InvokeAsync<object>("Extensions.WriteStorage", key, value);
        }
        public static Task RemoveLocalStorageAsync(string key)
        {
            return JSRuntime.Current.InvokeAsync<object>("Extensions.RemoveStorage", key);
        }
        public static async Task<(string data, string extension)> GetFileData(this ElementRef elementRef)
        {
            var data = await JSRuntime.Current.InvokeAsync<string>("Extensions.ReadUploadedFileAsText", elementRef);
            var fileExtension = await JSRuntime.Current.InvokeAsync<string>("Extensions.ReadUploadedFileExtension", elementRef);
            return (data, fileExtension);
        }
    }
}
