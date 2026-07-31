using System.Collections.Concurrent;
using System.Globalization;
using System.Text.Json;
using HorizonBudget.Services;
using static HorizonBudget.Services.LedgerKeyLookupFactory;

public sealed class CultureService : ICultureService
{
    private const string DefaultCulture = "en";

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly ConcurrentDictionary<uint, string> _translations = new();

    public event Action? CultureChanged;

    public string CurrentCulture { get; private set; } = DefaultCulture;

    public async Task InitializeAsync(string? culture = null)
    {
        CurrentCulture = string.IsNullOrWhiteSpace(culture)
            ? DefaultCulture
            : culture;

        await LoadTranslationsAsync(CurrentCulture);
        CultureChanged?.Invoke();
    }

    public async void SetCulture(string culture)
    {
        if (string.Equals(CurrentCulture, culture, StringComparison.OrdinalIgnoreCase))
            return;

        CurrentCulture = culture;
        await LoadTranslationsAsync(CurrentCulture);
        CultureChanged?.Invoke();
    }

    public string TranslateLedgerKey(uint code)
    {
        if (_translations.TryGetValue(code, out var value))
            return value;

        // Fallback: show hex code if no translation exists
        return code.ToString("X8", CultureInfo.InvariantCulture);
    }

    public string TranslateLedgerKeyPath(uint code)
    {
        // For now, path == single translation; later you can expand to full hierarchy
        return TranslateLedgerKey(code);
    }

    private async Task LoadTranslationsAsync(string culture)
    {
        _translations.Clear();

        var json = await LoadJsonAsync($"Translations/{culture}.json");

        var entries = JsonSerializer.Deserialize<List<RawLedgerNode>>(json, _jsonOptions) ?? [];

        foreach (var entry in entries)
        {
            _translations[entry.CodeValue] = entry.Code;
        }
    }

    private static async Task<string> LoadJsonAsync(string relativePath)
    {
        // UNO cross-platform asset loading
        var uri = new Uri($"ms-appx:///Assets/Data/{relativePath}");
        var file = await StorageFile.GetFileFromApplicationUriAsync(uri);
        return await FileIO.ReadTextAsync(file);
    }

    internal sealed partial record TranslationEntry(string Code, string Name, string Value)
    {
        public uint CodeValue =>
            uint.TryParse(Code, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var v)
                ? v
                : 0u;
    }
}
