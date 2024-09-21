using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmniCore.Client.ViewModels;

public class ViewModel : INotifyPropertyChanged, IDisposable
{
    public ViewModel(Page page)
    {
        page.BindingContext = this;
        page.Appearing += Page_Appearing;
        page.Disappearing += Page_Disappearing;
        this.Page = page;
    }

    private async void Page_Disappearing(object? sender, EventArgs e)
    {
        await OnDisappear();
    }

    private async void Page_Appearing(object? sender, EventArgs e)
    {
        await OnAppear();
    }

    public Page Page { get; protected set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void OnPropertyChanged()
    {
        if (this.PropertyChanged == null) return;
        this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(""));
    }

    public virtual ValueTask OnAppear() => ValueTask.CompletedTask;
    public virtual ValueTask OnDisappear() => ValueTask.CompletedTask;
    public virtual void Dispose()
    {
        this.Page.Appearing -= Page_Appearing;
        this.Page.Disappearing -= Page_Disappearing;
    }
}

public abstract class ViewModel<T> : ViewModel
{
    protected ViewModel(Page page) : base(page)
    {
    }

    public abstract Task LoadDataAsync(T data);
}
