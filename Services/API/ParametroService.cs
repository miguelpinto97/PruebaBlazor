using System.Net.Http.Json;
using RestauranteVirtual.Dto.Administracion.Parametros;
using RestauranteVirtual.Dto.Common;

namespace RestauranteVirtual.Web.Services.API
{
	public class ParametroService
    {
        private readonly HttpClient _httpClient;

        public ParametroService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ParametroValorDto>> ListarValoresDeParametro(string parametroId)
        {
            var response = await _httpClient.GetFromJsonAsync<List<ParametroValorDto>>($"api/Parametros/{parametroId}");
            return response;
        }

		public async Task<IDictionary<string,string>> ObtenerValores(string parametroId, string padreId = "")
		{
			var paramPadreId = "";
			if(!string.IsNullOrEmpty(padreId))
				paramPadreId= "?padreId=" + padreId;
			var response = await _httpClient.GetFromJsonAsync<Dictionary<string, string>>($"api/Parametros/{parametroId}/Valores{paramPadreId}");
			return response;
		}

		public async Task<ParametroValorBaseInfo> ObtenerBaseInfo()
        {
            var response = await _httpClient.GetFromJsonAsync<ParametroValorBaseInfo>($"api/Parametros/BaseInfo");
            return response;
        }

		public async Task<ParametroDetalleDto> ObtenerDetalle(string parametroId)
		{
			var response = await _httpClient.GetFromJsonAsync<ParametroDetalleDto>($"api/Parametros/{parametroId}/Detalle");
			return response;
		}

		public async Task<ApiResponse> RegistrarActualizar(ParametroValorAddUpdateRequest request)
		{
			var respuestaHttp = await _httpClient.PostAsJsonAsync("api/Parametros", request);
			var response = await respuestaHttp.Content.ReadFromJsonAsync<ApiResponse>();
            return response;
		}

		public async Task<ApiResponse> Eliminar(string parametroValorId)
		{
			var respuestaHttp = await _httpClient.DeleteAsync($"api/Parametros/{parametroValorId}");
			var response = await respuestaHttp.Content.ReadFromJsonAsync<ApiResponse>();
			return response;
		}

	}
}
