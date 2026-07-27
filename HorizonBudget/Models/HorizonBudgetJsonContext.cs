using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using HorizonBudget.Data.Records;

namespace HorizonBudget.Models;

[JsonSerializable(typeof(Account))]
[JsonSerializable(typeof(Expense))]
[JsonSerializable(typeof(Income))]
[JsonSerializable(typeof(Recurrence))]
[JsonSerializable(typeof(Transaction))]
[JsonSerializable(typeof(List<Account>))]
[JsonSerializable(typeof(List<Expense>))]
[JsonSerializable(typeof(List<Income>))]
[JsonSerializable(typeof(List<Recurrence>))]
[JsonSerializable(typeof(List<Transaction>))]
public partial class HorizonBudgetJsonContext : JsonSerializerContext
{
}
