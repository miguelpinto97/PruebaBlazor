using RestauranteVirtual.Dto.Alumno;
using RestauranteVirtual.Dto.Common;
using System.Net.Http.Json;

namespace RestauranteVirtual.Web.Services.API
{
    public class AlumnoService
    {
        private HttpClient _httpClient;

        public AlumnoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<PersonaRowDto>> ObtenerAlumnos()
        {
            var response = await _httpClient.GetFromJsonAsync<List<PersonaRowDto>>($"api/Alumno/Listar");
            return response;
        }
        public async Task<ApiResponse> RegistrarActualizar(PersonaAddUpdateRequest request)
        {
            var respuestaHttp = await _httpClient.PostAsJsonAsync("api/Alumno/RegistrarActualizar", request);
            var response = await respuestaHttp.Content.ReadFromJsonAsync<ApiResponse>();
            return response;
        }

		public async Task<AlumnoBaseInfo> ObtenerAlumnoBaseInfo()
		{
			var response = await _httpClient.GetFromJsonAsync<AlumnoBaseInfo>($"api/Alumno/GetAlumnoBaseInfo");
			return response;
		}
       
	}
}
