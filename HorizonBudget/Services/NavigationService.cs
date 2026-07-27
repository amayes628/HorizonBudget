namespace  HorizonBudget.Services;

public sealed class NavigationService(Frame frame)
{
    private readonly Frame _frame = frame;

    public void Navigate(Type pageType)
    {
        _frame.Navigate(pageType);
    }

    public void Navigate<T>() where T : Page
    {
        _frame.Navigate(typeof(T));
    }

    public void Back()
    {
        if (_frame.CanGoBack)
            _frame.GoBack();
    }
}
