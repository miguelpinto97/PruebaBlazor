using System.Net.Http.Json;
using RestauranteVirtual.Dto.Seguridad.MenuOpciones;

namespace RestauranteVirtual.Web.Services.API
{
	public class MenuOpcionesService
    {
        private readonly HttpClient _httpClient;

        public MenuOpcionesService(HttpClient httpClient)
        {
            this._httpClient = httpClient;
        }

        public async Task<List<MenuOpcionDto>> Listar()
        {
            var response = await _httpClient.GetFromJsonAsync<List<MenuOpcionDto>>("api/MenuOpciones");
            return response;
        }
    }
}
