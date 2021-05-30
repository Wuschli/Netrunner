using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Netrunner.Client.Components
{
    public partial class Login
    {
        private readonly Model _model = new();
        private bool _showErrors;
        private string _error = "";

        protected override async Task OnInitializedAsync()
        {
            Console.WriteLine(await _pingService.Ping());
            await base.OnInitializedAsync();
        }

        private async Task HandleLogin()
        {
            _showErrors = false;
            var result = await _authService.Login(_model.UserName, _model.Password);
            if (result?.Successful == true)
            {
                _navigationManager.NavigateTo("/");
            }
            else
            {
                _error = result?.Error ?? "No answer from server";
                _showErrors = true;
            }
        }

        private async Task HandleRegister()
        {
            _showErrors = false;
            var result = await _authService.Register(_model.UserName, _model.Password);
            if (result?.Successful == true)
            {
                _navigationManager.NavigateTo("/");
            }
            else
            {
                _error = result?.Error ?? "No answer from server";
                _showErrors = true;
            }
        }

        private class Model
        {
            [Required]
            public string? UserName { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string? Password { get; set; }
        }
    }
}