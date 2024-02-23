using Microsoft.AspNetCore.Components;
using System.Collections.Specialized;
using System.Web;
using RestauranteVirtual.Dto.Seguridad.Autenticacion;
using RestauranteVirtual.Web.Models.Auth;
using RestauranteVirtual.Web.Services.Authentication;

namespace RestauranteVirtual.Web.Pages.Auth
{
	public partial class CambiarContrasena
	{
		[Inject] IAuthService AuthService { get; set; }
		[Inject] NavigationManager NavigationManager { get; set; }

		private CambiarContrasenaModel _model = new();
		private string _mensajeError = "";
		private bool _cambioRealizado = false;

		private async Task HandleCambiar()
		{
			_mensajeError = "";

			if(_model.Password != _model.RepeatPassword)
			{
				_mensajeError = "Los valores no coinciden";
				return;
			}

			string queryString = NavigationManager.ToAbsoluteUri(NavigationManager.Uri).Query;
			NameValueCollection queryParameters = HttpUtility.ParseQueryString(queryString);
			if (queryParameters["key"] != null)
			{
				var key = queryParameters["key"].ToString();
				var result = await AuthService.CambiarContrasena(new ChangeRequest() { NewPassword = _model.Password, Key = key });

				if (result.Success)
				{
					_cambioRealizado = true;
				}
				else
				{
					_mensajeError = result.Title;
				}
			}
			else
			{
				_mensajeError = "La ruta no es válida";
			}	
		}
	}
}
