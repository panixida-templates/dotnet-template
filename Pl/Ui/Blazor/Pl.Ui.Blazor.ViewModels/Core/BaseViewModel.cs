namespace Pl.Ui.Blazor.ViewModels.Core;

public abstract record BaseViewModel<TId>
{
    public required TId Id { get; set; }
}
