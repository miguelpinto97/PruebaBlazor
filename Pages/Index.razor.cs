using Microsoft.AspNetCore.Components;
using RestauranteVirtual.Common.Constants;
using RestauranteVirtual.Dto.Pedidos;
using RestauranteVirtual.Web.Components.Busqueda;
using RestauranteVirtual.Web.Components.Validacion;
using RestauranteVirtual.Web.Services.API;
using RestauranteVirtual.Web.Services.Javascript;
using System.Net;

namespace RestauranteVirtual.Web.Pages
{
	public partial class Index
	{
		[Inject] PedidoService PedidoService { get; set; }
        [Inject] JavascriptService JavascriptService { get; set; }
        [Inject] NavigationManager NavManager { get; set; }

        public PedidosBaseInfo BaseInfo = new PedidosBaseInfo();
        public List<ItemDto> Items { get; set; }
        public List<ItemDto> ItemsFiltrados { get; set; }
        public IDictionary<string,string> Categorias { get; set; }

        public ItemDto ItemSeleccionado { get; set; } = new ItemDto();
        public List<ExtraProductoPedidoDto> Extras { get; set; }
        public List<CremaProductoPedidoDto> Cremas { get; set; }
        public List<ProductoPedidoDto> ProductosPedido { get; set; } = new List<ProductoPedidoDto>();

        protected override async Task OnInitializedAsync()
		{
            BaseInfo = await PedidoService.ObtenerBaseInfo(1);
            Items = BaseInfo.Items.ToList();
            ItemsFiltrados = BaseInfo.Items.ToList();
            Categorias = BaseInfo.Categorias;

            InicializarCremasExtras();

            StateHasChanged();
		}

		
		public void InicializarCremasExtras()
		{
            Cremas = Items.Where(x => x.TipoId == ParametrosConstants.Tipos_Items.CREMAS).Select(x => new CremaProductoPedidoDto()
            {
                CremaId = x.Id,
                Cantidad = 0
            }).ToList();

            Extras = Items.Where(x => x.TipoId == ParametrosConstants.Tipos_Items.EXTRAS).Select(x => new ExtraProductoPedidoDto()
            {
                ExtraId = x.Id,
                Cantidad = 0,
                Precio = x.Precio
            }).ToList();

            StateHasChanged();
        }
		public async void AgregarProducto()
		{
            ProductoPedidoDto ProductoPedido= new ProductoPedidoDto();
            ProductoPedido.ProductoId = ItemSeleccionado.Id;
            ProductoPedido.ProductoNombre = ItemSeleccionado.Nombre;
            ProductoPedido.PrecioProducto = ItemSeleccionado.Precio;
            ProductoPedido.Extras.AddRange(Extras.Where(x => x.Cantidad > 0).Select(x => x.Clone()).ToList());
            ProductoPedido.Cremas.AddRange(Cremas.Where(x => x.Cantidad > 0).Select(x => x.Clone()).ToList());
            ProductoPedido.RutaImg = ItemSeleccionado.RutaImg;
            ProductoPedido.IdTemporal = ProductosPedido.Any()? ProductosPedido.Max(x => x.IdTemporal) + 1 : 1;

            ProductoPedido.SubTotal = ProductoPedido.PrecioProducto + ProductoPedido.Extras.Sum(x => x.Cantidad * x.Precio);

            ProductosPedido.Add(ProductoPedido);

            await JavascriptService.MostrarMensajeExitoPersonalizado("Producto Agregado");

            StateHasChanged();
        }
        public void ArmarPedido(ItemDto itemSeleccionado)
        {
            ItemSeleccionado = itemSeleccionado;
            InicializarCremasExtras();
        }
        public decimal ObtenerSubtotal()
        {
            decimal subtotal = 0;

            subtotal = ItemSeleccionado.Precio + Extras.Sum(x => x.Cantidad * x.Precio);

            return subtotal;
        }
        public async void AbrirCompletarPedido()
        {

            ProductosPedido.ForEach(x => x.Cremas.RemoveAll(x => x.Cantidad < 1));
            ProductosPedido.ForEach(x => x.Extras.RemoveAll(x => x.Cantidad < 1));
            await JavascriptService.CerrarModal("modalProductoPedido"); 
              await JavascriptService.AbrirModal("modalPedido"); 
      }

        public async void CompletarPedido()
        {
            PedidoAddRequest request = new PedidoAddRequest();
            request.Productos = ProductosPedido;
            request.EntidadId = 1;
            request.UsuarioId = 1;
            request.Direccion = "";
            request.NumeroWhatsapp = "";

            var response = await PedidoService.RegistrarPedido(request);

            if (response.Success)
            {
                await JavascriptService.MostrarMensajeExito();
                await Task.Delay(500);

                int PedidoId = response.ObtenerId_Integer();

				string Mensaje = "Hola! Realicé un pedido. El detalle se encuentra aquí: ";
                string RutaPedido = $"https://salchipapaandina.netlify.app/DetallePedido/{PedidoId}";

                NavManager.NavigateTo($"https://wa.me/904166831?text="+Mensaje+RutaPedido);
            }
            else if (response.StatusCode == (int)HttpStatusCode.BadRequest)
            {
                await JavascriptService.MostrarMensajeErrorPersonalizado(response.Title);

            }
            else
            {
                await JavascriptService.MostrarMensajeErrorPersonalizado(response.Title);
            }
        }

        public async void EliminarItemPedido(ProductoPedidoDto productoPedido)
        {
            if (ProductosPedido.Count() == 1)
            {
                await JavascriptService.CerrarModal("modalPedido");
            }

            string id = "dPedido" + productoPedido.IdTemporal;

            await JavascriptService.EfectoEliminacion(id);

            await Task.Delay(500);
            StateHasChanged();

            ProductosPedido.Remove(productoPedido);

            StateHasChanged();



        }

        public async void SeleccionarCategoria(string IdHtml)
        {
            await JavascriptService.RemoverClaseMultiple(ParametrosConstants.ParametrosId.CATEGORIAS_ITEMS);

            await JavascriptService.AsignarClase(IdHtml);


            ItemsFiltrados.Clear();
            if(IdHtml == ParametrosConstants.ParametrosId.CATEGORIAS_ITEMS)
            {
                ItemsFiltrados.AddRange(BaseInfo.Items.ToList());
            }
            else
            {
                ItemsFiltrados.AddRange(BaseInfo.Items.Where(x=>x.CategoriaId == IdHtml).ToList());
            }

            StateHasChanged();
        }
    }
}
