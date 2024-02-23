using RestauranteVirtual.Dto.Common;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using System.Net;
using RestauranteVirtual.Web.Services.Javascript;
using RestauranteVirtual.Web.Components.Validacion;
using RestauranteVirtual.Web.Services.API;
using RestauranteVirtual.Web.Components.Busqueda;
using RestauranteVirtual.Dto.Alumno;
using System.Text.Json;
using RestauranteVirtual.Common.Constants;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace RestauranteVirtual.Web.Pages.Administracion
{
    public partial class Personas
	{



		/* Services Implementados */
		[Inject] AlumnoService AlumnoService { get; set; }
		[Inject] PersonaService PersonaService { get; set; }
		[Inject] JavascriptService JavascriptService { get; set; }
		[Inject] NavigationManager NavManager { get; set; }

		/* Generales */
		//public VentaSearchRequest SearchRequest = new VentaSearchRequest();
		//public VentaAnularRequest request = new VentaAnularRequest();

		//public VentaFiltrosBaseInfo BaseInfoFiltros = new VentaFiltrosBaseInfo();

		[Parameter] public string TipoVista { get { return _TipoVista; } set { _TipoVista = value; CargarDatos(); } }

		public string _TipoVista { get; set; } = "";

		private string? MensajeError = "";
		private string _columnaOrdenada = "";
		private bool _esAscendente = true;
		private bool hayData = false;
		private string _terminoBusqueda = "";

		/* Componentes */
		protected CustomFormValidator customFormValidator = new();
		protected Paginacion _paginacion = new();


		public async void CargarDatos()
		{
			switch (TipoVista)
			{
				case "Alumnos": _elementosTotales = await AlumnoService.ObtenerAlumnos(); break;
				case "Profesores": _elementosTotales = await PersonaService.ObtenerPersonasPorTipo(ParametrosConstants.RolesPersonas.PROFESOR); break;
				case "Padres": _elementosTotales = await PersonaService.ObtenerPersonasPorTipo(ParametrosConstants.RolesPersonas.PADRE_MADRE); break;
				case "Admin": _elementosTotales = await PersonaService.ObtenerPersonasPorTipo(ParametrosConstants.RolesPersonas.PERSONAL_ADMINISTRATIVO); break;
				default: _elementosTotales = new List<PersonaRowDto>(); break;
			}
			if (_elementosTotales.Count > 0)
			{
				hayData = true;
				StateHasChanged();
				_elementosFiltrados = _elementosTotales;

				_paginacion.PaginaActual = 1;
				_paginacion.ElementosPorPagina = 10;
				_paginacion.TotalElementos = _elementosTotales.Count;
				await _paginacion.CalcularPaginado();

				CambiarPagina();
				StateHasChanged();

			}
			else
			{
				hayData = false;
				_elementosFiltrados.Clear();
				_elementosPorPagina.Clear();
				StateHasChanged();
			}
		}

		protected override async Task OnInitializedAsync()
		{

			CargarDatos();
			


		}

		private async void ValorFiltroChanged(object sender, EventArgs e)
		{
			// Método que se ejecuta cuando se produce un cambio en los atributos
			// de la instancia de VentaSearchRequest.
			RecalcularTotalElementos();
		}

		private void FiltroGeneral(KeyboardEventArgs e)
		{
			if (e.Key != "Enter") return;

			AplicarFiltros();
		}
		public void AplicarFiltros()
		{
			_elementosFiltrados = _elementosTotales.Where(x => JsonSerializer.Serialize(x).ToUpper().Contains(_terminoBusqueda.Trim().ToUpper())
																	|| _terminoBusqueda.Trim().Equals("")
																	|| _terminoBusqueda == null)
														   .ToList();

			RecalcularTotalElementos();

		}
		private void RecalcularTotalElementos()
		{
			_paginacion.TotalElementos = _elementosFiltrados.Count;
			_paginacion.CalcularPaginado();
			CambiarPagina();
			StateHasChanged();
		}


		#region Metodos Generales


		#region Paginado

		public List<PersonaRowDto> _elementosTotales = new List<PersonaRowDto>();
		public List<PersonaRowDto> _elementosFiltrados = new List<PersonaRowDto>();
		public List<PersonaRowDto> _elementosPorPagina = new List<PersonaRowDto>();

		private void CambiarPagina()
		{
			_elementosPorPagina = _elementosFiltrados.Skip(_paginacion.IndiceInicio).Take(_paginacion.IndiceFin - _paginacion.IndiceInicio + 1).ToList();
		}

		#endregion


		#region Ordenamiento

		public void Ordenar(string nombreColumna, bool esAscendente)
		{
			_columnaOrdenada = nombreColumna;
			_esAscendente = esAscendente;
			AplicarOrdenamiento();
			CambiarPagina();
		}

		public void AplicarOrdenamiento()
		{
			_elementosFiltrados = _esAscendente
			  ? _elementosFiltrados.OrderBy(x => x.GetType().GetProperty(_columnaOrdenada)?.GetValue(x, null)).ToList()
			  : _elementosFiltrados.OrderByDescending(x => x.GetType().GetProperty(_columnaOrdenada)?.GetValue(x, null)).ToList();
		}

		#endregion

		#region Operaciones

		public string Accion = "";
		public int IdEdicion = 0;
		public bool esNuevo = false;
		public int ColaboradorEliminarId = 0;
		public int TipoSeleccion = 1;
		public bool esPrimeraConfirmacion = true;
		public bool SeleccionarTodos = true;

		public List<PersonaRowDto> ItemsSeleccionados { get; set; } = new List<PersonaRowDto>();


		public async void Limpiar_Cancelar()
		{
			ItemsSeleccionados.Clear();
			IdEdicion = 0;
			esNuevo = false;
			SeleccionarTodos = true;
			await JavascriptService.CheckboxAsignarValor("chkPadre", false);
		}
		//public void CheckTotal()
		//{
		//	TipoSeleccion = 2;

		//	if (SeleccionarTodos)
		//	{
		//		ItemsSeleccionados.AddRange(_elementosPorPagina.Where(x => x.EstadoId != ParametrosConstants.EstadosVenta.ANULADO));
		//	}
		//	else
		//	{
		//		ItemsSeleccionados.Clear();
		//	}
		//	SeleccionarTodos = !SeleccionarTodos;

		//}
		public void CheckearItem(PersonaRowDto dto)
		{
			TipoSeleccion = 2;

			if (ItemsSeleccionados.Contains(dto))
			{
				ItemsSeleccionados.Remove(dto);
			}
			else
			{
				ItemsSeleccionados.Add(dto);
			}
		}
		public void AbrirConfigurar(int Id = 0)
		{
			if (Id == 0)
			{
				NavManager.NavigateTo($"/Comercial/Ventas/Nuevo");
			}
			else
			{
				NavManager.NavigateTo($"/Comercial/Ventas/Configurar/{Id}");
			}
		}
		//public async void PrepararEliminar(int Id = 0)
		//{
		//	ColaboradorEliminarId = Id; //Asigna Id a Eliminar

		//	Accion = GeneralConstants.Acciones.ANULAR;

		//	if (Accion == GeneralConstants.Acciones.ANULAR)
		//	{
		//		if (ColaboradorEliminarId == 0 && ItemsSeleccionados.Count == 0)
		//		{
		//			await JavascriptService.MostrarMensajeAlerta("Debe seleccionar al menos un registro");
		//			StateHasChanged();
		//			return;
		//		}
		//	}


		//	await JavascriptService.AbrirModal("ModalConfirmacion");
		//	StateHasChanged();

		//}


		//public async void ConfirmarAccion()
		//{
		//	ApiResponse response = new ApiResponse();

		//	MensajeError = "";
		//	//customFormValidator.ClearFormErrors();

		//	switch (Accion)
		//	{
		//		case GeneralConstants.Acciones.ANULAR:
		//			{
		//				if (ColaboradorEliminarId == 0 && ItemsSeleccionados.Count == 0)
		//				{
		//					await JavascriptService.MostrarMensajeAlerta("Debe seleccionar al menos un registro");
		//					return;
		//				}
		//				response = await Eliminar();
		//			}
		//			break;
		//	}


		//	if (response.Success)
		//	{
		//		Limpiar_Cancelar();

		//		await JavascriptService.MostrarMensajeExito();

		//		/* Refresca según la BD, pero en la página actual */

		//		//DATOS SEARCH REQUEST

		//		_elementosPorPagina = await AlumnoService.ObtenerVentas(SearchRequest);
		//		BaseInfoFiltros = await AlumnoService.ObtenerBaseInfoFiltros();

		//		RecalcularTotalElementos();

		//	}
		//	else if (response.StatusCode == (int)HttpStatusCode.BadRequest)
		//	{
		//		MensajeError = response.Title;
		//		//customFormValidator.DisplayFormErrors(response.Errors);
		//	}
		//	else
		//	{
		//		MensajeError = response.Title;
		//	}

		//	StateHasChanged();
		//}


		//public async Task<ApiResponse> Eliminar()
		//{
		//	ApiResponse respuesta = new ApiResponse();


		//	if (ColaboradorEliminarId == 0)
		//	{
		//		request.Ventas = ItemsSeleccionados.Select(x => x.Id).ToList();
		//	}
		//	else
		//	{
		//		List<int> lista = new List<int>
		//		{
		//			ColaboradorEliminarId
		//		};

		//		request.Ventas = lista;
		//	}


		//	respuesta = await AlumnoService.AnularVentas(request);

		//	return respuesta;

		//}
		#endregion

		#endregion


	}
}
