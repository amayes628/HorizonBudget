using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.Json;
using HorizonBudget.Data;
using HorizonBudget.Data.Records;
using HorizonBudget.Data.Types;
using Microsoft.UI.Xaml; // Required for ms-appx:// URI loading
using Windows.Storage;

namespace HorizonBudget.Services;

public sealed class LedgerKeyLookupFactory : ILedgerKeyLookupFactory
{
    private readonly Dictionary<uint, LedgerEntry> _ledgerByCode = [];
    private readonly Lock _initLock = new();

    private readonly JsonSerializerOptions jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public IReadOnlyList<LedgerEntry> MasterLedgerKeys { get; private set; } = [];

    public async Task InitializeAsync()
    {
        lock (_initLock)
        {
            _ledgerByCode.Clear();
        }

        var json = await LoadJsonAsync("LedgerKey.json");

        var rawNodes = JsonSerializer.Deserialize<List<RawLedgerNode>>(json, jsonOptions) ?? [];

        var flattened = new List<LedgerEntry>();

        foreach (var (code, key, type) in FlattenLedgerNodes(rawNodes))
        {
            var entry = new LedgerEntry(code, key, type, "en");
            _ledgerByCode[code] = entry;
            flattened.Add(entry);
        }

        MasterLedgerKeys = flattened;
    }

    public LedgerEntry Get(uint code) =>
        _ledgerByCode.TryGetValue(code, out var entry)
            ? entry
            : LedgerEntry.Empty;

    public ObservableCollection<LedgerEntry> AllLedgerKeys => new(MasterLedgerKeys);

    public LedgerEntry GetLedgerEntryById(uint LedgerId) => AllLedgerKeys.FirstOrDefault(entry => entry.Code == LedgerId) ?? LedgerEntry.Empty;

    private static async Task<string> LoadJsonAsync(string fileName)
    {
        // Correct UNO cross-platform asset loading
        var uri = new Uri($"ms-appx:///Assets/Data/{fileName}");
        var file = await StorageFile.GetFileFromApplicationUriAsync(uri);
        return await FileIO.ReadTextAsync(file);
    }

    private static IEnumerable<(uint Code, string Key, LedgerType Type)> FlattenLedgerNodes(IEnumerable<RawLedgerNode> nodes)
    {
        foreach (var node in nodes)
        {
            var type = MapLedgerType(node.Name);

            yield return (node.CodeValue, node.Name, type);

            foreach (var child in FlattenLedgerNodes(node.Children))
                yield return child;
        }
    }

    private static LedgerType MapLedgerType(string key) =>
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

    public sealed partial record RawLedgerNode(
        string Code,
        string Name,
        List<RawLedgerNode> Children)
    {
        public uint CodeValue =>
            uint.TryParse(Code, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var v)
                ? v
                : 0u;

        public static RawLedgerNode Empty => new("0", string.Empty, []);
    }

    internal sealed record TranslationEntry(uint Code, string Name, string Value)
    {
        public static TranslationEntry Empty => new(0u, string.Empty, string.Empty);
    }
}
