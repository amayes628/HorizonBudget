using System.Text.Json.Serialization;
using HorizonBudget.Data.Types;
using static HorizonBudget.Services.LedgerKeyLookupFactory;

namespace HorizonBudget.Serialization;

[JsonSerializable(typeof(List<RawLedgerNode>))]
[JsonSerializable(typeof(RawLedgerNode))]
public partial class LedgerKeyJsonContext : JsonSerializerContext
{
}
