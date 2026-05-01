using System.Text.Json;

public static class EnvironmentSettings
{
    private const string GeminiApiKeyName = "GEMINI_API_KEY";
    private const string SettingsFileName = "appsettings.json";

    public static string GetGeminiApiKey()
    {
        var apiKey = Environment.GetEnvironmentVariable(GeminiApiKeyName);
        if (!string.IsNullOrWhiteSpace(apiKey))
        {
            return apiKey;
        }

        var settings = LoadSettings();
        if (settings.TryGetValue(GeminiApiKeyName, out var keyValue) && !string.IsNullOrWhiteSpace(keyValue))
        {
            return keyValue;
        }

        throw new InvalidOperationException($"Environment variable {GeminiApiKeyName} is required. Set it in the environment or in {SettingsFileName}.");
    }

    private static Dictionary<string, string> LoadSettings()
    {
        var path = Path.Combine(AppContext.BaseDirectory, SettingsFileName);
        if (!File.Exists(path))
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        var json = File.ReadAllText(path);
        using var doc = JsonDocument.Parse(json);

        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var property in doc.RootElement.EnumerateObject())
        {
            values[property.Name] = property.Value.GetString() ?? string.Empty;
        }

        return values;
    }
}
