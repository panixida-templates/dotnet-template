namespace Pl.Ui.Blazor.ViewModels.Core;

public abstract record BaseViewModel<TId>
{
    public TId Id { get; set; }
}
