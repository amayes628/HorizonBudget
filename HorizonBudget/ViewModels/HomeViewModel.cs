using System;
using System.Collections.Generic;
using System.Text;

namespace HorizonBudget.ViewModels;

public partial class HomeViewModel
{
    private readonly INavigator _navigator;

    public HomeViewModel(INavigator navigator)
    {
        _navigator = navigator ?? throw new ArgumentNullException(nameof(navigator));
    }

    // Expose the injected service so the page can inspect it
    public INavigator Navigator => _navigator;
}
