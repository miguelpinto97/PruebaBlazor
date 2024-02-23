using RestauranteVirtual.Dto.Entidad;
using RestauranteVirtual.Dto.Common;
using RestauranteVirtual.Dto.Seguridad.Perfiles;
using System.Net.Http.Json;

namespace RestauranteVirtual.Web.Services.API
{
	public class EntidadService
	{
		private HttpClient _httpClient;
		public EntidadService(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public async Task<List<EntidadDto>> ObtenerEntidads()
		{
			var response = await _httpClient.GetFromJsonAsync<List<EntidadDto>>($"api/Entidad/Listar");
			return response;
		}
		public async Task<ApiResponse> GuardarPermisos(PerfilPermisosUpdateRequest request)
		{
			var respuestaHttp = await _httpClient.PostAsJsonAsync($"api/Entidad/{request.PerfilId}/Permisos", request);
			var response = await respuestaHttp.Content.ReadFromJsonAsync<ApiResponse>();
			return response;
		}
	}

}
