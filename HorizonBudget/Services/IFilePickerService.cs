namespace  HorizonBudget.Services;

public interface IFilePickerService
{
    Task<StorageFile?> PickAsync();
}
