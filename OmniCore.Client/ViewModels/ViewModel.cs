using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmniCore.Client.ViewModels;

public class ViewModel : ObservableObject
{
    public ViewModel(Page page)
    {
        this.Page = page;
        this.Page.BindingContext = this;
    }

    public Page Page { get; protected set; }
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
