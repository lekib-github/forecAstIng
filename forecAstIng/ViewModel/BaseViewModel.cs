using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace forecAstIng.ViewModel
{
    public abstract partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        bool isWorking;

        [ObservableProperty]
        string title;
    }
}
