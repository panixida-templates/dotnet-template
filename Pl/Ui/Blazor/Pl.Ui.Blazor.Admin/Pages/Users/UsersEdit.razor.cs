using System.ComponentModel.DataAnnotations;

using Common.Enums;

using Microsoft.AspNetCore.Components;

using MudBlazor;

using Pl.Ui.Blazor.Services.Interfaces;
using Pl.Ui.Blazor.ViewModels;

namespace Pl.Ui.Blazor.Admin.Pages.Users;

public partial class UsersEdit
{
    [Inject] private IUsersService UsersService { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private ISnackbar Snackbar { get; set; } = default!;

    [Parameter] public int? Id { get; set; }

    private static readonly Type ModelType = typeof(UserViewModel);
    private static readonly EmailAddressAttribute EmailValidator = new();

    private readonly Role[] _roles = Enum.GetValues<Role>();

    private MudForm? _form;

    private bool _loading;
    private bool _saving;
    private string? _error;

    private UserViewModel _model = CreateNewModel();

    private bool IsEditMode
    {
        get
        {
            return Id.HasValue;
        }
    }

    private string Title
    {
        get
        {
            return IsEditMode ? "Редактирование пользователя" : "Создание пользователя";
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        _error = null;

        if (!IsEditMode)
        {
            _model = CreateNewModel();
            return;
        }

        _loading = true;

        try
        {
            _model = await UsersService.GetAsync(Id!.Value, cancellationToken: CancellationToken.None);
        }
        catch (Exception ex)
        {
            _error = ex.Message;
        }
        finally
        {
            _loading = false;
        }
    }

    private async Task OnSaveClicked()
    {
        if (_form is null)
        {
            return;
        }

        _error = null;

        await _form.Validate();

        if (!_form.IsValid)
        {
            return;
        }

        _saving = true;

        try
        {
            if (IsEditMode)
            {
                await UsersService.UpdateAsync(Id!.Value, _model, CancellationToken.None);
                Snackbar.Add("Пользователь сохранён.", Severity.Success);
                Navigation.NavigateTo("/users");
                return;
            }

            var createdId = await UsersService.CreateAsync(_model, CancellationToken.None);
            Snackbar.Add("Пользователь создан.", Severity.Success);
            Navigation.NavigateTo($"/users/edit/{createdId}");
        }
        catch (Exception ex)
        {
            _error = ex.Message;
            Snackbar.Add("Не удалось сохранить пользователя.", Severity.Error);
        }
        finally
        {
            _saving = false;
        }
    }

    private void OnCancelClicked()
    {
        Navigation.NavigateTo("/users");
    }

    private void OnBirthdayChanged(DateTime? date)
    {
        if (!date.HasValue)
        {
            return;
        }

        _model.Birthday = date.Value;
    }

    private IEnumerable<string> ValidateEmail(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            yield return "Введите Email.";
            yield break;
        }

        if (!EmailValidator.IsValid(value))
        {
            yield return "Некорректный Email.";
        }
    }

    private static UserViewModel CreateNewModel()
    {
        return new UserViewModel
        {
            Id = 0,
            Role = default,
            Name = string.Empty,
            Email = string.Empty,
            Phone = string.Empty,
            Age = 0,
            Birthday = DateTime.Today,
        };
    }
}
