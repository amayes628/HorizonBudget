using System;

namespace HorizonBudget.Models;

public class Alert
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Message { get; init; } = string.Empty;
    public DateOnly Due { get; set; }
}
