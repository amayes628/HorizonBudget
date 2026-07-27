namespace  HorizonBudget.Services;

public interface IDataExportService
{
    Task ExportAsync();
    Task ImportAsync(StorageFile file);
}
