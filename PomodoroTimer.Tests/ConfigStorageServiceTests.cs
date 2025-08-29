using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PomodoroTimer.Tests.Services
{
    public class ConfigStorageServiceTests
    {
        private readonly MockLocalStorageService _localStorage;
        private readonly ConfigStorageService _service;

        public ConfigStorageServiceTests()
        {
            _localStorage = new MockLocalStorageService();
            _service = new ConfigStorageService(_localStorage);
        }

        [Fact]
        public async Task SaveUserConfigAsync_ShouldSaveConfigToLocalStorage()
        {
            // Arrange
            var config = new PomodoroConfig
            {
                Name = "Test Config",
                Work = TimeSpan.FromMinutes(30),
                Break = TimeSpan.FromMinutes(10),
                LongBreak = TimeSpan.FromMinutes(20),
                LongBreakInterval = 3,
                Cycles = 5
            };

            // Act
            await _service.SaveUserConfigAsync(config);

            // Assert
            var savedData = _localStorage.GetStoredValue("pomodoro_user_configs");
            Assert.NotNull(savedData);
            
            var configs = JsonSerializer.Deserialize<List<PomodoroConfig>>(savedData);
            Assert.NotNull(configs);
            Assert.Single(configs);
            Assert.Equal("Test Config", configs[0].Name);
        }

        [Fact]
        public async Task GetUserConfigsAsync_ShouldReturnEmptyListWhenNoConfigs()
        {
            // Act
            var result = await _service.GetUserConfigsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task DeleteUserConfigAsync_ShouldRemoveConfigFromStorage()
        {
            // Arrange
            var config1 = new PomodoroConfig { Name = "Config 1", Work = TimeSpan.FromMinutes(25) };
            var config2 = new PomodoroConfig { Name = "Config 2", Work = TimeSpan.FromMinutes(30) };
            
            await _service.SaveUserConfigAsync(config1);
            await _service.SaveUserConfigAsync(config2);

            // Act
            await _service.DeleteUserConfigAsync("Config 1");

            // Assert
            var remaining = await _service.GetUserConfigsAsync();
            Assert.Single(remaining);
            Assert.Equal("Config 2", remaining[0].Name);
        }

        [Fact]
        public async Task ConfigNameExistsAsync_ShouldReturnTrueWhenConfigExists()
        {
            // Arrange
            var config = new PomodoroConfig { Name = "Existing Config", Work = TimeSpan.FromMinutes(25) };
            await _service.SaveUserConfigAsync(config);

            // Act
            var result = await _service.ConfigNameExistsAsync("Existing Config");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ConfigNameExistsAsync_ShouldReturnFalseWhenConfigDoesNotExist()
        {
            // Arrange
            var config = new PomodoroConfig { Name = "Existing Config", Work = TimeSpan.FromMinutes(25) };
            await _service.SaveUserConfigAsync(config);

            // Act
            var result = await _service.ConfigNameExistsAsync("Non-existing Config");

            // Assert
            Assert.False(result);
        }
    }

    // Test implementation that wraps LocalStorageService functionality
    public class MockLocalStorageService : LocalStorageService
    {
        private readonly Dictionary<string, string> _storage = new();

        public MockLocalStorageService() : base(new MockJSRuntime())
        {
        }

        public new async Task<T?> GetItemAsync<T>(string key)
        {
            await Task.CompletedTask; // Simulate async
            if (_storage.TryGetValue(key, out var value))
            {
                try
                {
                    return JsonSerializer.Deserialize<T>(value);
                }
                catch
                {
                    return default(T);
                }
            }
            return default(T);
        }

        public new async Task SetItemAsync<T>(string key, T item)
        {
            await Task.CompletedTask; // Simulate async
            var value = JsonSerializer.Serialize(item);
            _storage[key] = value;
        }

        public new async Task RemoveItemAsync(string key)
        {
            await Task.CompletedTask; // Simulate async
            _storage.Remove(key);
        }

        public string? GetStoredValue(string key)
        {
            return _storage.TryGetValue(key, out var value) ? value : null;
        }
    }

    // Simple test implementation of IJSRuntime
    public class MockJSRuntime : IJSRuntime
    {
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
        {
            return ValueTask.FromResult(default(TValue)!);
        }

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
        {
            return ValueTask.FromResult(default(TValue)!);
        }
    }
}
