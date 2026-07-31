namespace HorizonBudget.Validation;

public static class ValidationMessages
{
    // Length rules
    public const string NameLength = "Name must be between 3 and 50 characters.";
    public const string LedgerRequired = "Ledger key is required.";
    public const string OpeningBalanceInvalid = "Opening balance cannot be negative.";

    // Status / domain rules
    public const string CannotCloseBalanceNotZero = "Account cannot be closed unless Current Balance is 0.";
    public const string UnsavedChanges = "You have unsaved changes.";
    public const string CorrectHighlightedFields = "Please correct the highlighted fields.";
}
