using Microsoft.JSInterop;
using System.Threading.Tasks;
using Mono.WebAssembly.Interop;

namespace BookkeepingNasheDetstvo.Client.Extensions
{
    public static class BrowserExtensions
    {
        private static readonly IJSRuntime JsRuntime = new MonoWebAssemblyJSRuntime();
        
        public static Task<string> ReadLocalStorageAsync(string key)
        {
            return JsRuntime.InvokeAsync<string>("Extensions.ReadStorage", key);
        }
        public static Task WriteLocalStorageAsync(string key, string value)
        {
            return JsRuntime.InvokeAsync<object>("Extensions.WriteStorage", key, value);
        }
        public static Task RemoveLocalStorageAsync(string key)
        {
            return JsRuntime.InvokeAsync<object>("Extensions.RemoveStorage", key);
        }
    }
}
