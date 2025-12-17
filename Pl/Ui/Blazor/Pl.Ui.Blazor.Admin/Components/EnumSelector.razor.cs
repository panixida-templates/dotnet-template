using Microsoft.AspNetCore.Components;

using MudBlazor;

namespace Pl.Ui.Blazor.Admin.Components;

public partial class EnumSelector<TEnum>
    where TEnum : struct, Enum
{
    [Parameter] public TEnum? Value { get; set; }
    [Parameter] public EventCallback<TEnum?> ValueChanged { get; set; }

    [Parameter] public string? Label { get; set; }
    [Parameter] public Variant Variant { get; set; } = Variant.Outlined;
    [Parameter] public bool Dense { get; set; } = true;
    [Parameter] public bool Clearable { get; set; } = true;
    [Parameter] public string? Style { get; set; }
}
