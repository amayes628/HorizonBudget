using HorizonBudget.Forecasting;

namespace  HorizonBudget.Services;

public sealed partial class DataExportService : IDataExportService
{
    public async Task ExportAsync()
    {
        //var sample = ForecastSamples.SampleInput(); // replace with real data later
        var forecastData = new ForecastResult().ToString();
        await EmbeddedResourceReader.WriteAsync("horizon_export.json", forecastData);
    }

    public async Task ImportAsync(StorageFile file)
    {
        if (file == null)
            return;
        await EmbeddedResourceReader.ReadAsync("Templates", "horizon_export.json");

        // Deserialize into your ForecastInput model
        //var imported = JsonSerializer.Deserialize<ForecastInput>(json);

        // TODO: store imported data in your app’s database or memory
    }
}
