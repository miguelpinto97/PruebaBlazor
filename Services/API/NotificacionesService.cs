using RestauranteVirtual.Dto.Alumno;
using RestauranteVirtual.Dto.Common;
using RestauranteVirtual.Dto.Notificaciones;
using System.Net.Http.Json;

namespace RestauranteVirtual.Web.Services.API
{
	public class NotificacionesService
	{
		private HttpClient _httpClient;

		public NotificacionesService(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public async Task<ApiResponse> CrearNotificacion(NotificacionAddRequest request)
		{
			var respuestaHttp = await _httpClient.PostAsJsonAsync("api/Notificacion/Registrar", request);
			var response = await respuestaHttp.Content.ReadFromJsonAsync<ApiResponse>();
			return response;
		}
		public async Task<List<NotificacionDto>> ObtenerNotificaciones(int NumeroNotificaciones)
		{
			var response = await _httpClient.GetFromJsonAsync<List<NotificacionDto>>($"api/Notificacion/Listar?NumeroNotificaciones={NumeroNotificaciones}");
			return response;
		}
	}
}
