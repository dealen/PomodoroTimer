using System.Text.Json;

public class ConfigStorageService
{
    private readonly LocalStorageService _localStorage;
    private const string USER_CONFIGS_KEY = "pomodoro_user_configs";
    
    public ConfigStorageService(LocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task<List<PomodoroConfig>> GetUserConfigsAsync()
    {
        try
        {
            var configs = await _localStorage.GetItemAsync<List<PomodoroConfig>>(USER_CONFIGS_KEY);
            return configs ?? new List<PomodoroConfig>();
        }
        catch
        {
            return new List<PomodoroConfig>();
        }
    }

    public async Task SaveUserConfigAsync(PomodoroConfig config)
    {
        var configs = await GetUserConfigsAsync();
        
        // Remove existing config with same name (update scenario)
        configs.RemoveAll(c => c.Name == config.Name);
        
        // Add the new/updated config
        configs.Add(config);
        
        await _localStorage.SetItemAsync(USER_CONFIGS_KEY, configs);
    }

    public async Task DeleteUserConfigAsync(string configName)
    {
        var configs = await GetUserConfigsAsync();
        configs.RemoveAll(c => c.Name == configName);
        await _localStorage.SetItemAsync(USER_CONFIGS_KEY, configs);
    }

    public async Task<bool> ConfigNameExistsAsync(string name)
    {
        var configs = await GetUserConfigsAsync();
        return configs.Any(c => !string.IsNullOrEmpty(c.Name) && c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }
}
