using RestauranteVirtual.Dto.Alumno;
using RestauranteVirtual.Dto.Common;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Json;

namespace RestauranteVirtual.Web.Services.API
{
	public class PersonaService
	{
		private HttpClient _httpClient;

		public PersonaService(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}
		//
		public async Task<List<PersonaRowDto>> ObtenerPersonasPorTipo(string RolId)
		{
			var response = await _httpClient.GetFromJsonAsync<List<PersonaRowDto>>($"api/Persona/Listar?RolId={RolId}");
			return response;
		}
		public async Task<PersonaAddUpdateRequest> ObtenerDatos(int PersonaId, string RolId)
		{
			var response = await _httpClient.GetFromJsonAsync<PersonaAddUpdateRequest>($"api/Persona/ObtenerDatos?PersonaId={PersonaId}&RolId={RolId}");
			return response;
		}
		public async Task<ApiResponse> RegistrarActualizarPadre(PersonaAddUpdateRequest request)
		{
			var respuestaHttp = await _httpClient.PostAsJsonAsync("api/Persona/RegistrarActualizarPadre", request);
			var response = await respuestaHttp.Content.ReadFromJsonAsync<ApiResponse>();
			return response;
		}

		public async Task<ApiResponse> RegistrarActualizarProfesor(PersonaAddUpdateRequest request)
		{
			var respuestaHttp = await _httpClient.PostAsJsonAsync("api/Persona/RegistrarActualizarProfesor", request);
			var response = await respuestaHttp.Content.ReadFromJsonAsync<ApiResponse>();
			return response;
		}

		public async Task<ApiResponse> RegistrarActualizarAdmin(PersonaAddUpdateRequest request)
		{
			var respuestaHttp = await _httpClient.PostAsJsonAsync("api/Persona/RegistrarActualizarAdmin", request);
			var response = await respuestaHttp.Content.ReadFromJsonAsync<ApiResponse>();
			return response;
		}

	}
}
