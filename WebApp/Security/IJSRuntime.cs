using Microsoft.JSInterop;
using System.Text.Json;

namespace WebApp.Security
{
    public static class IJSRuntimeHelper
    {
        public static async ValueTask<object> RemoveStorage(this IJSRuntime js, string key)
        {
            var obj = await js.InvokeAsync<object>("localStorage.removeItem", key);
            return obj;
        }
        public static async ValueTask SetItem(this IJSRuntime js, string key, string value)
        {
            await js.InvokeVoidAsync("localStorage.setItem", key, value);
        }
        public static async ValueTask<T?> GetItemObject<T>(this IJSRuntime js, string key)
        {
            var item = await js.GetItem(key);
            return JsonSerializer.Deserialize<T>(item);
        }
        public static async ValueTask<string> GetItem(this IJSRuntime js, string key)
        {
            return await js.InvokeAsync<string>("localStorage.getItem", key);
        }
    }
}
