using Microsoft.JSInterop;

namespace Bookkeeping.Extensions;

public static class BrowserExtensions
{
    public static ValueTask<string> ReadLocalStorageAsync(this IJSRuntime runtime, string key)
    {
        return runtime.InvokeAsync<string>("Extensions.ReadStorage", key);
    }
    public static async ValueTask WriteLocalStorageAsync(this IJSRuntime runtime, string key, string value)
    {
        await runtime.InvokeAsync<object>("Extensions.WriteStorage", key, value);
    }
    public static async Task RemoveLocalStorageAsync(this IJSRuntime runtime, string key)
    {
        await runtime.InvokeAsync<object>("Extensions.RemoveStorage", key);
    }
}