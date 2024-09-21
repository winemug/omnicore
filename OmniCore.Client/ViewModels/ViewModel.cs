using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmniCore.Client.ViewModels;

public class ViewModel : INotifyPropertyChanged
{
    public ViewModel(Page page)
    {
        page.BindingContext = this;
        this.Page = page;
    }

    public Page Page { get; protected set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void OnPropertyChanged()
    {
        if (this.PropertyChanged == null) return;
        this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(""));
    }

    public virtual ValueTask OnResumed() => ValueTask.CompletedTask;
    public virtual ValueTask OnPaused() => ValueTask.CompletedTask;
    public virtual ValueTask OnNavigatingTo() => ValueTask.CompletedTask;
    public virtual ValueTask OnNavigatingAway() => ValueTask.CompletedTask;
}

public abstract class ViewModel<T> : ViewModel
{
    protected ViewModel(Page page) : base(page)
    {
    }

    public abstract Task LoadDataAsync(T data);
}
