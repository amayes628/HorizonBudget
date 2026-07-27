using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.Json;
using HorizonBudget.Data;
using HorizonBudget.Data.Types;


namespace HorizonBudget.Services;

// Loads hierarchical categories.json and per-culture translation files (e.g., en.json).
// Flattening converts nested nodes into (code:uint, key:string) master entries.
public sealed partial class CategoryLookupFactory : ICategoryLookupFactory
{
    private const string DefaultCulture = "en";
    private readonly Dictionary<uint, LedgerEntry> _masterByCode = [];
    private readonly Lock _initLock = new();
    private readonly JsonSerializerOptions jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public IReadOnlyList<LedgerEntry> MasterCat { get; set; } = [];

    public async Task InitializeAsync()
    {
        lock (_initLock)
        {
            _masterByCode.Clear();
        }

        var json = await LoadJsonAsync("categories.json");
#pragma warning disable SYSLIB0020
        var raw = JsonSerializer.Deserialize<List<RawNode>>(json, jsonOptions) ?? [];
#pragma warning restore SYSLIB0020
        var master = new List<LedgerEntry>();

        foreach (var (code, key, type) in FlattenNodes(raw))
        {
            var entry = new LedgerEntry(code, key, type, "en");
            _masterByCode[code] = entry;
            master.Add(entry);
        }

        MasterCat = master;
    }

    public LedgerEntry Get(uint code) => _masterByCode.TryGetValue(code, out var m) ? m : LedgerEntry.Empty;

    public ObservableCollection<LedgerEntry> AllCategories => [..MasterCat];

    ObservableCollection<LedgerEntry> ICategoryLookupFactory.AllCategories => throw new NotImplementedException();

    IReadOnlyList<LedgerEntry> ICategoryLookupFactory.MasterCat { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    private static async Task<string> LoadJsonAsync(string fileName)
    {
        return await EmbeddedResourceReader.ReadAsync($"HorizonBudget.Core.Resources.Categories", fileName);
    }

    private static IEnumerable<(uint Code, string Key, LedgerType Type)> FlattenNodes(IEnumerable<RawNode> nodes)
    {
        foreach (var node in nodes)
        {
            var type = MapType(node.Key);

            yield return (node.CodeValue, node.Key, type);

            foreach (var child in FlattenNodes(node.Children))
                yield return child;
        }
    }
    private static LedgerType MapType(string key) =>
    key switch
    {
        "Assets" => LedgerType.Asset,
        "Liabilities" => LedgerType.Liability,
        "Income" => LedgerType.Income,
        "Expenses" => LedgerType.Expense,
        "Health-Insurance" => LedgerType.Insurance,
        "Lifestyle" => LedgerType.Lifestyle,
        _ => LedgerType.Undefined
    };

    LedgerEntry ICategoryLookupFactory.Get(uint code)
    {
        throw new NotImplementedException();
    }

    // DTOs / internal records

    // Hierarchical node DTO for categories.json (non-nullable; Children defaults to empty list)
    internal sealed partial record RawNode(
    string Code,
    string Key,
    List<RawNode> Children)
    {
        public static RawNode Empty => new("0", string.Empty, []);

        public uint CodeValue =>
            uint.TryParse(Code, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var v)
                ? v
                : 0u;
    }

    // Translation entry DTO for {culture}.json (non-nullable)
    internal sealed partial record TranslationEntry(uint Code, string Key, string Value)
    {
        public static TranslationEntry Empty => new(0u, string.Empty, string.Empty);
    }
}
