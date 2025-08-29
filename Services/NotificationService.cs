using Microsoft.JSInterop;

public class NotificationService
{
    private readonly IJSRuntime _jsRuntime;

    public NotificationService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<bool> RequestPermissionAsync()
    {
        try
        {
            var permission = await _jsRuntime.InvokeAsync<string>("Notification.requestPermission");
            return permission == "granted";
        }
        catch
        {
            return false;
        }
    }

    public async Task ShowNotificationAsync(string title, string body, string? icon = null)
    {
        try
        {
            var options = new
            {
                body,
                icon = icon ?? "data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='64' height='64' viewBox='0 0 64 64'%3E%3Crect width='64' height='64' fill='%23667eea' rx='8'/%3E%3Ccircle cx='32' cy='32' r='20' fill='none' stroke='white' stroke-width='3'/%3E%3C/svg%3E",
                badge = icon ?? "data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='64' height='64' viewBox='0 0 64 64'%3E%3Crect width='64' height='64' fill='%23667eea' rx='8'/%3E%3Ccircle cx='32' cy='32' r='20' fill='none' stroke='white' stroke-width='3'/%3E%3C/svg%3E",
                vibrate = new[] { 200, 100, 200 },
                requireInteraction = true
            };

            await _jsRuntime.InvokeVoidAsync("showNotification", title, options);
        }
        catch
        {
            // Silently fail if notifications are not supported
        }
    }

    public async Task<bool> IsPermissionGrantedAsync()
    {
        try
        {
            var permission = await _jsRuntime.InvokeAsync<string>("getNotificationPermission");
            return permission == "granted";
        }
        catch
        {
            return false;
        }
    }
}
