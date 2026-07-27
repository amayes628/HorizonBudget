namespace HorizonBudget.Models;

public static class EmbeddedResourceReader
{
    public static async Task<string> ReadAsync(string folder, string fileName)
    {
        var assembly = typeof(EmbeddedResourceReader).Assembly;

        var resourceName =
            $"HorizonBudget.Core.Resources.{folder}.{fileName}";

        using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException(
                $"Missing embedded resource: {resourceName}");

        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }
    public static async Task<string> WriteAsync(string fileName, string content)
    {
        var folder = ApplicationData.Current.LocalFolder.Path;
        var path = Path.Combine(folder, fileName);

        using var writer = new StreamWriter(path, append: false);
        await writer.WriteAsync(content);

        return path;
    }
}
