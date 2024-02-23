using Microsoft.AspNetCore.Components;
using System.Collections.Specialized;
using System.Web;
using RestauranteVirtual.Web.Models.Auth;
using RestauranteVirtual.Web.Services.Authentication;

namespace RestauranteVirtual.Web.Pages
{
    public partial class Login
    {      

        [Inject] IAuthService AuthService { get; set; }
        [Inject] NavigationManager NavigationManager { get; set; }

        private LoginModel _loginModel = new();
        private string _mensajeError = "";

        private async Task HandleLogin()
        {
            _mensajeError = "";
            var result = await AuthService.Login(_loginModel);

            if (result.Success)
            {
                await AuthService.GuardarPermisos();

				string queryString = NavigationManager.ToAbsoluteUri(NavigationManager.Uri).Query;
                NameValueCollection queryParameters = HttpUtility.ParseQueryString(queryString);
                if (queryParameters["returnUrl"] != null)
                {
                    var url = queryParameters["returnUrl"];
                    NavigationManager.NavigateTo("/" + HttpUtility.UrlDecode(url), true);
                }
                else
                {
                    NavigationManager.NavigateTo("/");
                }
            }
            else
            {
                _mensajeError = result.Title;
            }
        }
		protected override async Task OnInitializedAsync()
        {
			Console.WriteLine(NavigationManager.Uri);

		}

	}
}
