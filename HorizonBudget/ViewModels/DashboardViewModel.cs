using System;
using System.Collections.Generic;
using System.Text;

namespace HorizonBudget.ViewModels;

public partial class DashboardViewModel(INavigator navigator)
{
    private readonly INavigator _navigator = navigator ?? throw new ArgumentNullException(nameof(navigator));
    public INavigator GetNavigator => _navigator;
}
