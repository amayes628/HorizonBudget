public interface ITestNavigator
{
    Task NavigateToAsync(Type pageType, object? parameter = null);
}

public class FrameTestNavigator : ITestNavigator
{
    public Task NavigateToAsync(Type pageType, object? parameter = null)
    {
        var window = Microsoft.UI.Xaml.Window.Current;
        if (window?.Content is Microsoft.UI.Xaml.Controls.Frame frame)
        {
            frame.DispatcherQueue.TryEnqueue(() => frame.Navigate(pageType, parameter));
        }
        return Task.CompletedTask;
    }
}
