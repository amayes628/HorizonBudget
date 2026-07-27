using System.Collections.Concurrent;
using System.Text.Json;

namespace  HorizonBudget.Services;

public sealed class CultureService : ICultureService
{
    private readonly ConcurrentDictionary<string, Dictionary<uint, string>> _cache = new();
    private string _currentCulture;

    // Category translation maps
    private readonly Dictionary<uint, string> _codeToKey = [];
    private readonly Dictionary<string, string> _keyToLocalized = [];

    // Domain-specific localization maps
    private readonly Dictionary<string, string> _localizedApp = [];
    private readonly Dictionary<string, string> _localizedEnums = [];
    private readonly Dictionary<string, string> _localizedMessages = [];
    private readonly Dictionary<string, string> _localizedPages = [];

    public event Action? CultureChanged;

    public CultureService(string initialCulture)
    {
        _currentCulture = Normalize(initialCulture);
        _ = LoadDefaultTranslations();
    }

    public string CurrentCultureCode => _currentCulture;

    // ----------------------------------------------------------------------
    // CATEGORY TRANSLATION
    // ----------------------------------------------------------------------

    public string TranslateCategory(uint categoryCode)
    {
        // First: find the category key from the flattened category tree
        if (_codeToKey.TryGetValue(categoryCode, out var key))
        {
            // Try localized category name
            if (_keyToLocalized.TryGetValue(key, out var localized))
                return localized;

            // Fallback: localized app domain
            if (_localizedApp.TryGetValue(key, out localized))
                return localized;

            // Fallback: raw key
            return key;
        }

        return $"Category({categoryCode:X8})";
    }

    public string TranslateCategoryPath(uint categoryCode)
    {
        // In the new architecture, categoryCode is not bit-pattern encoded.
        // The "path" is simply the localized category name.
        return TranslateCategory(categoryCode);
    }

    // ----------------------------------------------------------------------
    // CULTURE MANAGEMENT
    // ----------------------------------------------------------------------

    public void SetCulture(string cultureCode)
    {
        var normalized = Normalize(cultureCode);
        if (normalized == _currentCulture)
            return;

        _currentCulture = normalized;
        ReloadTranslations();
    }

    public void ReloadTranslations()
    {
        _ = LoadDefaultTranslations();
        CultureChanged?.Invoke();
    }

    // ----------------------------------------------------------------------
    // LOAD TRANSLATIONS
    // ----------------------------------------------------------------------

    private async Task LoadDefaultTranslations()
    {
        // Load category structure
        await LoadCategoryTreeAsync();

        // Load culture-specific domain files
        string culture = _currentCulture;

        await LoadDomainAsync(_localizedApp, "app", $"Resources/Localization/{culture}/app.json");
        await LoadDomainAsync(_localizedEnums, "enums", $"Resources/Localization/{culture}/enums.json");
        await LoadDomainAsync(_localizedMessages, "messages", $"Resources/Localization/{culture}/messages.json");
        await LoadDomainAsync(_localizedPages, "pages", $"Resources/Localization/{culture}/pages.json");

        // Build key → localized lookup
        _keyToLocalized.Clear();
        foreach (var kvp in _localizedApp)
            _keyToLocalized[kvp.Key] = kvp.Value;
        foreach (var kvp in _localizedEnums)
            _keyToLocalized[kvp.Key] = kvp.Value;
        foreach (var kvp in _localizedMessages)
            _keyToLocalized[kvp.Key] = kvp.Value;
        foreach (var kvp in _localizedPages)
            _keyToLocalized[kvp.Key] = kvp.Value;
    }

    // ----------------------------------------------------------------------
    // CATEGORY TREE LOADING
    // ----------------------------------------------------------------------

    private async Task LoadCategoryTreeAsync()
    {
        var path = Path.Combine("Resources", "raw");
        string json = await EmbeddedResourceReader.ReadAsync(path, "categories.json");
#pragma warning disable SYSLIB0020
        var rootNodes = JsonSerializer.Deserialize<List<CategoryNode>>(json) ?? [];
#pragma warning restore SYSLIB0020
        _codeToKey.Clear();

        foreach (var (code, key) in FlattenNodes(rootNodes))
            _codeToKey[code] = key;
    }

    private static IEnumerable<(uint Code, string Key)> FlattenNodes(IEnumerable<CategoryNode> nodes)
    {
        foreach (var n in nodes)
        {
            uint code = ParseCode(n.Code);
            yield return (code, n.Key);

            foreach (var child in FlattenNodes(n.Children))
                yield return child;
        }
    }

    private static uint ParseCode(string code)
    {
        if (code.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            return Convert.ToUInt32(code[2..], 16);

        return Convert.ToUInt32(code, 16);
    }

    // ----------------------------------------------------------------------
    // DOMAIN LOADING
    // ----------------------------------------------------------------------

    private static async Task LoadDomainAsync(Dictionary<string, string> dict, string domainName, string path)
    {
        dict.Clear();

        string json = await LoadJsonAsync(path,domainName);
#pragma warning disable SYSLIB0020
        var root = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(json) ?? [];
#pragma warning restore SYSLIB0020
        if (root.TryGetValue(domainName, out var domainDict))
        {
            foreach (var kvp in domainDict)
                dict[kvp.Key] = kvp.Value;
        }
    }

    // ----------------------------------------------------------------------
    // FILE LOADING
    // ----------------------------------------------------------------------

    private static async Task<string> LoadJsonAsync(string path, string fileName)
    {
        return await EmbeddedResourceReader.ReadAsync(path, fileName);
    }

    private static string Normalize(string culture)
    {
        if (string.IsNullOrWhiteSpace(culture))
            return "en";

        if (culture.Contains('-'))
            return culture.Split('-')[0];

        return culture;
    }

    // ----------------------------------------------------------------------
    // CATEGORY NODE DTO
    // ----------------------------------------------------------------------

    private sealed class CategoryNode
    {
        public string Code { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public List<CategoryNode> Children { get; set; } = [];
    }
}
