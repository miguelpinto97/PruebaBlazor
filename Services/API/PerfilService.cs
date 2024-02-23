using System.Net.Http;
using System.Net.Http.Json;
using RestauranteVirtual.Dto.Administracion.Parametros;
using RestauranteVirtual.Dto.Common;
using RestauranteVirtual.Dto.Seguridad;
using RestauranteVirtual.Dto.Seguridad.Perfiles;
using RestauranteVirtual.Dto.Seguridad.Permisos;

namespace RestauranteVirtual.Web.Services.API
{
    public class PerfilService
    {
        private readonly HttpClient _httpClient;

        public PerfilService(HttpClient httpClient)
        {
            this._httpClient = httpClient;
        }

        public async Task<List<PerfilDto>> Listar()
        {
            var response = await _httpClient.GetFromJsonAsync<List<PerfilDto>>("api/Perfiles");
            return response;
        }

		public async Task<List<PermisoDto>> ListarPermisosPorPerfil(int perfilId)
		{
			var response = await _httpClient.GetFromJsonAsync<List<PermisoDto>>($"api/Perfiles/{perfilId}/Permisos");
			return response;
		}

		public async Task<PerfilBaseInfo> ObtenerBaseInfo()
        {
			var response = await _httpClient.GetFromJsonAsync<PerfilBaseInfo>($"api/Perfiles/BaseInfo");
			return response;
		}

		public async Task<ApiResponse> RegistrarActualizar(PerfilAddUpdateRequest perfil)
        {
            var respuestaHttp = await _httpClient.PostAsJsonAsync("api/Perfiles", perfil);
            var response = await respuestaHttp.Content.ReadFromJsonAsync<ApiResponse>();
            return response;
        }

		public async Task<ApiResponse> GuardarPermisos(PerfilPermisosUpdateRequest request)
		{
			var respuestaHttp = await _httpClient.PostAsJsonAsync($"api/Perfiles/{request.PerfilId}/Permisos", request);
			var response = await respuestaHttp.Content.ReadFromJsonAsync<ApiResponse>();
			return response;
		}

		public async Task<ApiResponse> Eliminar(int id)
        {
			var respuestaHttp = await _httpClient.DeleteAsync($"api/Perfiles/{id}");
			var response = await respuestaHttp.Content.ReadFromJsonAsync<ApiResponse>();
			return response;
        }
		public async Task<List<PermisoDto>> ListarPermisosPorEntidad(int EntidadId)
		{
			var response = await _httpClient.GetFromJsonAsync<List<PermisoDto>>($"api/Perfiles/{EntidadId}/PermisosxEntidad");
			return response;
		}
	}
}
