using Microsoft.AspNetCore.Components;
using RestauranteVirtual.Dto.Pedidos;
using RestauranteVirtual.Web.Services.API;
using RestauranteVirtual.Web.Services.Javascript;

namespace RestauranteVirtual.Web.Pages
{
    public partial class DetallePedido
    {
        [Parameter] public int PedidoId { get; set; } = 0;
        [Inject] PedidoService PedidoService { get; set; }
        [Inject] JavascriptService JavascriptService { get; set; }
        [Inject] NavigationManager NavManager { get; set; }
        public List<ItemDto> Items { get; set; }


        public PedidoDto Pedido { get; set; } = new PedidoDto();

        protected override async Task OnInitializedAsync()
        {
            Pedido = await PedidoService.ObtenerDatosPedido(PedidoId);
            StateHasChanged();

            await JavascriptService.AbrirModalEstatico("modalPedido");
        }
    }
}
