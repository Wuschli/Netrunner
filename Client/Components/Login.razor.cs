using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Netrunner.Client.Components
{
    public partial class Login
    {
        private readonly Model _model = new();
        private bool _showErrors;
        private string _error = "";

        private async Task HandleLogin()
        {
            _showErrors = false;
            var result = await AuthService.Login(_model.UserName, _model.Password);
            if (result.Successful)
            {
                NavigationManager.NavigateTo("/");
            }
            else
            {
                _error = result.Error;
                _showErrors = true;
            }
        }

        private async Task HandleRegister()
        {
            _showErrors = false;
            var result = await AuthService.Register(_model.UserName, _model.Password);
            if (result.Successful)
            {
                NavigationManager.NavigateTo("/");
            }
            else
            {
                _error = result.Error;
                _showErrors = true;
            }
        }

        private class Model
        {
            [Required]
            public string UserName { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }
    }
}