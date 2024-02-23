using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Web;
using RestauranteVirtual.Common.Utils;
using RestauranteVirtual.Dto.Common;
using RestauranteVirtual.Dto.Seguridad.Autenticacion;
using RestauranteVirtual.Dto.Seguridad.Permisos;
using RestauranteVirtual.Web.Models.Auth;

namespace RestauranteVirtual.Web.Services.Authentication
{
	public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly CustomAuthenticationStateProvider _authenticationStateProvider;
        private readonly ILocalStorageService _localStorage;
        private readonly NavigationManager _navigationManager;

        public AuthService(HttpClient httpClient,
                           CustomAuthenticationStateProvider authenticationStateProvider,
                           ILocalStorageService localStorage,
						   NavigationManager navigationManager)
        {
            _httpClient = httpClient;
            _authenticationStateProvider = authenticationStateProvider;
            _localStorage = localStorage;
            _navigationManager = navigationManager;
        }       

        public async Task<ApiResponse> Login(LoginModel loginModel)
        {
            var loginAsJson = JsonSerializer.Serialize(loginModel);
            var response = await _httpClient.PostAsync("api/Auth", new StringContent(loginAsJson, Encoding.UTF8, "application/json"));
            var loginResult = JsonSerializer.Deserialize<ApiResponse>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (!response.IsSuccessStatusCode)
            {
                return loginResult;
            }
            var token = loginResult.Data["token"].ToString();
            await _localStorage.SetItemAsync("authToken", token);
            _authenticationStateProvider.MarkUserAsAuthenticated(loginModel.Username);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);

            return loginResult;
        }

		public async Task Refresh()
		{
            var omitir = new string[] { "Login", "CambiarContrasena", "RecuperarContrasena" };
            if (omitir.Any(x => _navigationManager.Uri.Contains(x))) return;
			var loginResult = new ApiResponse();

			//try
			//{
			//	var savedToken = await _localStorage.GetItemAsync<string>("authToken");
			//	var handler = new JwtSecurityTokenHandler();
			//	var jsonToken = handler.ReadJwtToken(savedToken);
			//	TimeSpan diferencia = jsonToken.ValidTo - FechaUtil.FechaHoraUTC();
   //             if(diferencia < TimeSpan.Zero)
   //             {
			//		_navigationManager.NavigateTo("/Login?returnUrl=" + HttpUtility.UrlEncode(_navigationManager.ToBaseRelativePath(_navigationManager.Uri)), forceLoad: true);
			//		return;
			//	}
   //             else if (diferencia < TimeSpan.FromMinutes(10))
   //             {
			//		var respuestaHttp = await _httpClient.PostAsync("api/Auth/refresh", null);
			//		loginResult = await respuestaHttp.Content.ReadFromJsonAsync<ApiResponse>();
			//		if (loginResult.Success)
			//		{
			//			var token = loginResult.Data["token"].ToString();
			//			if (token != "")
			//			{
			//				await _localStorage.SetItemAsync("authToken", token);
			//				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
			//			}
			//		}
			//	}				
			//}
			//catch(Exception ex)
   //         {
			//	_navigationManager.NavigateTo("/Login?returnUrl=" + HttpUtility.UrlEncode(_navigationManager.ToBaseRelativePath(_navigationManager.Uri)), forceLoad: true);
			//	return;
			//}            	
		}

		public async Task Logout()
        {
            await _localStorage.RemoveItemAsync("authToken");
            _authenticationStateProvider.MarkUserAsLoggedOut();
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        public async Task<List<PermisoDto>> ListarPermisos()
        {
			var savedPermisos = await _localStorage.GetItemAsync<List<PermisoDto>>("authPermisos");
			if(savedPermisos == null)
				_navigationManager.NavigateTo("/Logout", forceLoad: true);
			return savedPermisos;
        }

		public async Task GuardarPermisos()
		{
			var response = await _httpClient.GetFromJsonAsync<List<PermisoDto>>("api/Auth/Permisos");
			await _localStorage.SetItemAsync("authPermisos", response);
		}

		public async Task<bool> ValidarPermisos(string[] permisos)
		{
			var savedPermisos = await _localStorage.GetItemAsync<List<PermisoDto>>("authPermisos");
			return permisos.Any(x => savedPermisos.Any(s => s.Id == x));
		}

		public async Task<DateTime> ObtenerFecha()
		{
			var response = await _httpClient.GetFromJsonAsync<DateTime>("api/Auth/Fecha");
			return response;
		}

		public async Task<ApiResponse> SolicitarRecuperarContrasena(ResetRequest request)
        {
			var respuestaHttp = await _httpClient.PostAsJsonAsync("api/Auth/Reestablecer", request);
			var response = await respuestaHttp.Content.ReadFromJsonAsync<ApiResponse>();
			return response;
		}

		public async Task<ApiResponse> CambiarContrasena(ChangeRequest request)
		{
			var respuestaHttp = await _httpClient.PostAsJsonAsync("api/Auth/ActualizarContrasena", request);
			var response = await respuestaHttp.Content.ReadFromJsonAsync<ApiResponse>();
			return response;
		}
	}
}
