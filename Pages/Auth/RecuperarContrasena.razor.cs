using Microsoft.AspNetCore.Components;
using System.Collections.Specialized;
using System.Web;
using RestauranteVirtual.Dto.Seguridad.Autenticacion;
using RestauranteVirtual.Web.Models.Auth;
using RestauranteVirtual.Web.Services.Authentication;

namespace RestauranteVirtual.Web.Pages.Auth
{
	public partial class RecuperarContrasena
	{
		[Inject] IAuthService AuthService { get; set; }
		[Inject] NavigationManager NavigationManager { get; set; }

		private RecuperarModel _model = new();
		private string _mensajeError = "";
		private bool _solicitudEnviada = false;

		private async Task HandleRecuperar()
		{
			_mensajeError = "";
			var result = await AuthService.SolicitarRecuperarContrasena(new ResetRequest() { Username = _model.UserName });

			if (result.Success)
			{
				_solicitudEnviada = true;
			}
			else
			{
				_mensajeError = result.Title;
			}
		}
	}
}
