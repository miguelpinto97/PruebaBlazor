using Microsoft.AspNetCore.Components;
//using RestauranteVirtual.Common.Constants;
//using RestauranteVirtual.Common.Utils;
using RestauranteVirtual.Web.Services.API;
using RestauranteVirtual.Common.Constants;
using RestauranteVirtual.Common.Utils;

namespace RestauranteVirtual.Web.Components.Busqueda
{
	public partial class Paginacion : ComponentBase
	{

		[Inject] ParametroService ParametroService { get; set; }

		[Parameter]
		public EventCallback PaginaSeleccionada { get; set; }
		public int ElementosPorPagina { get; set; } = 10;
		public int PaginaActual { get; set; } = 1;
		public int TotalElementos { get; set; }
		public int IndiceInicio { get; private set; }
		public int IndiceFin { get; private set; }
		public int CantidadDePaginas { get; private set; }
		public int PaginadoInicial { get; set; } = 1;
		public int PaginadoFinal { get; set; } = 1;
		private List<int> ListadoPaginas = new();


		protected override async Task OnInitializedAsync()
		{
			
		}

		public async Task CalcularPaginado()
		{
			if (ListadoPaginas.Count == 0)
			{
				var valoresPaginacion = await ParametroService.ObtenerValores(ParametrosConstants.ParametrosId.PAGINACION);
				ListadoPaginas = valoresPaginacion.Select(x => x.Value.ToInt()).ToList();
				ElementosPorPagina = ListadoPaginas.First();
				PaginaActual = 1;
			}

			CantidadDePaginas = (int)Math.Ceiling((double)TotalElementos / ElementosPorPagina);


			if (CantidadDePaginas > 0)
			{
				if (PaginaActual > CantidadDePaginas)
				{
					PaginaActual = CantidadDePaginas;
				}
				IndiceInicio = (PaginaActual - 1) * ElementosPorPagina;
				IndiceFin = IndiceInicio + ElementosPorPagina - 1;
				if (IndiceFin >= TotalElementos)
				{
					IndiceFin = TotalElementos - 1;
				}
			}

			if(PaginaActual == 1)
			{
				PaginadoInicial = 1;
			}

			PaginadoFinal = PaginadoInicial + 4;

			if (PaginadoFinal > CantidadDePaginas) PaginadoFinal = CantidadDePaginas;

			StateHasChanged();
		}

		private async Task AsignarCantidadPorPagina(int cantidad)
		{
			ElementosPorPagina = cantidad;
			PaginaActual = 1;
			await CalcularPaginado();
			await PaginaSeleccionada.InvokeAsync();
		}

		private async Task CambiarPagina(int nroPagina, int tipo = 0)
		{
			if (nroPagina == PaginaActual) return;

			if (nroPagina < 1) nroPagina = 1;
			if (nroPagina > CantidadDePaginas) nroPagina = CantidadDePaginas;


			PaginaActual = nroPagina;

			int residuo = nroPagina % 5;

			if (tipo == 1 || tipo == 4)
			{

				if (residuo == 0)
				{
					PaginadoFinal = nroPagina;
					PaginadoInicial = PaginadoFinal - 5 + 1;
				}
				else
				{
					PaginadoInicial = nroPagina - residuo + 1;
					PaginadoFinal = PaginadoInicial + 5;
				}

			}
			else if (tipo == 2)
			{
				if (residuo == 0)
				{
					PaginadoFinal = nroPagina;
					PaginadoInicial = PaginadoFinal - 5 + 1;
				}
			}
			else if (tipo == 3)
			{
				if (residuo != 0)
				{
					PaginadoInicial = nroPagina - residuo + 1;
					PaginadoFinal = PaginadoInicial + 5;
				}
			}
			await CalcularPaginado();
			await PaginaSeleccionada.InvokeAsync();
		}
	}
}
