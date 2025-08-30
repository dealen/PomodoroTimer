using Microsoft.JSInterop;

namespace PomodoroTimer.Services;

public class ThemeService
{
    private readonly IJSRuntime _jsRuntime;
    private string _currentTheme = "light";
    
    public event EventHandler<string>? ThemeChanged;
    
    public ThemeService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }
    
    public string CurrentTheme => _currentTheme;
    
    public async Task InitializeAsync()
    {
        // Try to load saved theme from localStorage
        try
        {
            var savedTheme = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "theme");
            if (!string.IsNullOrEmpty(savedTheme) && (savedTheme == "light" || savedTheme == "dark"))
            {
                _currentTheme = savedTheme;
            }
            else
            {
                // Check system preference
                var prefersDark = await _jsRuntime.InvokeAsync<bool>("window.matchMedia", "(prefers-color-scheme: dark)");
                _currentTheme = prefersDark ? "dark" : "light";
            }
        }
        catch
        {
            // Fallback to light theme if anything fails
            _currentTheme = "light";
        }
        
        await ApplyThemeAsync();
    }
    
    public async Task ToggleThemeAsync()
    {
        _currentTheme = _currentTheme == "light" ? "dark" : "light";
        await ApplyThemeAsync();
        await SaveThemeAsync();
        ThemeChanged?.Invoke(this, _currentTheme);
    }
    
    public async Task SetThemeAsync(string theme)
    {
        if (theme != "light" && theme != "dark") return;
        
        _currentTheme = theme;
        await ApplyThemeAsync();
        await SaveThemeAsync();
        ThemeChanged?.Invoke(this, _currentTheme);
    }
    
    private async Task ApplyThemeAsync()
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("document.documentElement.setAttribute", "data-theme", _currentTheme);
        }
        catch
        {
            // Ignore JS errors during theme application
        }
    }
    
    private async Task SaveThemeAsync()
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "theme", _currentTheme);
        }
        catch
        {
            // Ignore errors when saving theme
        }
    }
    
    public bool IsDarkMode => _currentTheme == "dark";
    public bool IsLightMode => _currentTheme == "light";
}
