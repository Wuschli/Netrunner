using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Netrunner.Shared.Services;

namespace Netrunner.Client.Components
{
    public partial class Login
    {
        private readonly Model _model = new();
        private bool _showErrors;
        private string _error = "";

        protected override async Task OnInitializedAsync()
        {
            var pingService = await _serviceHelper.GetService<IPingService>();
            Console.WriteLine(await pingService.Ping());
            await base.OnInitializedAsync();
        }

        private async Task HandleLogin()
        {
            _showErrors = false;
            var result = await _authHelper.Login(_model.Username, _model.Password);
            if (result?.Successful == true)
            {
                //_navigationManager.NavigateTo("/");
                Console.WriteLine("Successfully logged in!");
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
            var result = await _authHelper.Register(_model.Username, _model.Password);
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
            public string? Username { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string? Password { get; set; }
        }
    }
}