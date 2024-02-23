using RestauranteVirtual.Dto.Alumno;
using RestauranteVirtual.Dto.Common;
using RestauranteVirtual.Dto.Pedidos;
using System.Net.Http.Json;

namespace RestauranteVirtual.Web.Services.API
{
	public class PedidoService
	{
		private HttpClient _httpClient;

		public PedidoService(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public async Task<PedidosBaseInfo> ObtenerBaseInfo(int EntidadId)
		{
			var response = await _httpClient.GetFromJsonAsync<PedidosBaseInfo>($"api/Pedidos/BaseInfo?EntidadId={EntidadId}");
			return response;
		}
		public async Task<PedidoDto> ObtenerDatosPedido(int PedidoId)
		{
			var response = await _httpClient.GetFromJsonAsync<PedidoDto>($"api/Pedidos/ObtenerDatos?PedidoId={PedidoId}");
			return response;
		}
		public async Task<ApiResponse> RegistrarPedido(PedidoAddRequest request)
		{
			var respuestaHttp = await _httpClient.PostAsJsonAsync("api/Pedidos/Registrar", request);
			var response = await respuestaHttp.Content.ReadFromJsonAsync<ApiResponse>();
			return response;
		}
	}
}
