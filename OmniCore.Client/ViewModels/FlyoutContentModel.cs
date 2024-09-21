using OmniCore.Client.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OmniCore.Client.ViewModels;

public class FlyoutContentModel : ViewModel
{
    public FlyoutContentModel(FlyoutContentPage page) : base(page)
    {
    }

    public List<FlyoutItemModel> FlyoutItems { get; set; } = new List<FlyoutItemModel>();

    //public ICommand FlyoutSelectionChangedCommand => new RelayCommand(FlyoutSelectionChanged);

    private void FlyoutSelectionChanged()
    {
        Debug.WriteLine($"{DateTimeOffset.UtcNow} FlyoutSelectionChanged");
    }
}
public class FlyoutItemModel
{
    public required string Title { get; set; }
    public required string Icon { get; set; }
}