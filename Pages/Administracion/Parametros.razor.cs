using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Net;
using RestauranteVirtual.Common.Constants;
using RestauranteVirtual.Common.Utils;
using RestauranteVirtual.Dto.Administracion.Parametros;
using RestauranteVirtual.Web.Components.Busqueda;
using RestauranteVirtual.Web.Components.Validacion;
using RestauranteVirtual.Web.Models.Administracion;
using RestauranteVirtual.Web.Services.API;
using RestauranteVirtual.Web.Services.Javascript;

namespace RestauranteVirtual.Web.Pages.Administracion
{
	public partial class Parametros
    {
        /* Services Implementados */
        [Inject] ParametroService ParametroService { get; set; }
        [Inject] JavascriptService JavascriptService { get; set; }

		protected CustomFormValidator _customFormValidator = new();
		protected Paginacion _paginacion = new();
		protected EditContext _editContext;
		private List<ParametroValorDto> _elementosCompleto = new();
		private List<ParametroValorDto> _elementosFiltrados = new();
		private List<ParametroValorDto> _elementosPorPagina = new();
		private ParametroValorBaseInfo _baseInfo = new();
		private ParametroModel _parametroModel = new();
        private ParametroDetalleDto _parametroDetalle = new();
		private string _columnaOrdenada = "";
		private string _parametroId = "";
		private string _valorIdEliminar = "";
		public string _accion = "";
		private bool _filtroEstadoLoaded = false;
		private List<string> _filtroEstado = new List<string>();
		private string? _mensajeError = "";
		private string? _terminoBusqueda;
		private IDictionary<string, string> _valoresPadre = new Dictionary<string, string>();

		protected override void OnInitialized()
		{
			_editContext = new EditContext(_parametroModel);
			_baseInfo.Parametros = new Dictionary<string, string>();
			_baseInfo.Estados = new Dictionary<string, string>();
		}

		protected override async Task OnInitializedAsync()
        {
			var baseInfo = await ParametroService.ObtenerBaseInfo();
			_baseInfo.Parametros = baseInfo.Parametros;
			_baseInfo.Estados = baseInfo.Estados;
			_parametroId = _baseInfo.Parametros.Keys.First();
            _parametroDetalle = await ParametroService.ObtenerDetalle(_parametroId);
			_valoresPadre = string.IsNullOrEmpty(_parametroDetalle.ParametroPadreId) ?
								new Dictionary<string, string>() :
								await ParametroService.ObtenerValores(_parametroDetalle.ParametroPadreId);

			_elementosFiltrados =_elementosCompleto = await ParametroService.ListarValoresDeParametro(_parametroId);
			_paginacion.PaginaActual = 1;
			_paginacion.ElementosPorPagina = 10;
			_paginacion.TotalElementos = _elementosFiltrados.Count;
			await _paginacion.CalcularPaginado();
			CambiarPagina();
			StateHasChanged();
        }

        protected override async void OnAfterRender(bool firstRender)
        {
			if (_baseInfo.Estados.Count > 0 && !_filtroEstadoLoaded)
			{
				_filtroEstadoLoaded = true;
				await JavascriptService.InicializarMultiselect("ddlFiltroEstado", "FiltrarEstado", DotNetObjectReference.Create(this));
			}

			if (firstRender)
            {
				await JavascriptService.InicializarSelect2("ddlParametros", "FiltrarParametro", DotNetObjectReference.Create(this));
				StateHasChanged();
            }
        }

		private void CambiarPagina()
		{
			_elementosPorPagina = _elementosFiltrados
									.Skip(_paginacion.IndiceInicio)
									.Take(_paginacion.IndiceFin - _paginacion.IndiceInicio + 1)
									.ToList();
		}

		private async Task RecalcularTotalElementos()
		{
			_paginacion.TotalElementos = _elementosFiltrados.Count;
			await _paginacion.CalcularPaginado();
			CambiarPagina();
			StateHasChanged();
		}


		#region Filtros

		public async Task Filtrar()
		{
			_elementosFiltrados = _elementosCompleto
				.Where(x => x.Descripcion.Buscar(_terminoBusqueda)
							|| x.ValorPadre.Buscar(_terminoBusqueda)
							|| x.Valor.Buscar(_terminoBusqueda)
							|| x.ValorExterno.Buscar(_terminoBusqueda)
							|| x.Id.Buscar(_terminoBusqueda)
							|| _terminoBusqueda == null
							|| _terminoBusqueda == "")
				.Where(x => _filtroEstado.Contains(x.EstadoId) || _filtroEstado.Count == 0)
				.ToList();
			Cancelar();
			await RecalcularTotalElementos();
		}

		public async Task FiltroGeneral(KeyboardEventArgs e)
		{
			if (e.Key != "Enter") return;
			await Filtrar();
		}		

		[JSInvokable]
		public async Task FiltrarParametro(string parametroId)
		{
			_parametroId = parametroId;
			_parametroDetalle = await ParametroService.ObtenerDetalle(_parametroId);
			_valoresPadre = string.IsNullOrEmpty(_parametroDetalle.ParametroPadreId) ?
								new Dictionary<string, string>() :
								await ParametroService.ObtenerValores(_parametroDetalle.ParametroPadreId);
			
			_elementosFiltrados = _elementosCompleto = await ParametroService.ListarValoresDeParametro(_parametroId);
			await Filtrar();
		}

		#endregion


		#region Ordenamiento
		public async Task Ordenamiento(string nombreColumna, bool esAscendente)
		{
			_elementosFiltrados = esAscendente
			  ? _elementosFiltrados.OrderBy(x => x.GetType().GetProperty(nombreColumna)?.GetValue(x, null)).ToList()
			  : _elementosFiltrados.OrderByDescending(x => x.GetType().GetProperty(nombreColumna)?.GetValue(x, null)).ToList();

			_columnaOrdenada = nombreColumna; 
			CambiarPagina();
		}

		#endregion


		#region CRUD
		public string? IdEdicion = null;
		public bool esNuevo = false;

		public async Task ActivarEdicion(string Id)
		{
			esNuevo = false;
			IdEdicion = Id;
			var item = _elementosPorPagina
				.Where(x => x.Id == IdEdicion)
				.Select(x => new ParametroModel()
				{
					Id = x.Id,
					Descripcion = x.Descripcion,
					EstadoId = x.EstadoId,
					Orden = x.Orden,
					Valor = x.Valor,
					ValorExterno = x.ValorExterno,
					ValorPadreId = x.ValorPadreId
				})
				.First();

			_parametroModel.Valor = item.Valor;
			_parametroModel.ValorPadreId = item.ValorPadreId;
			_parametroModel.Id = item.Id;
			_parametroModel.Orden = item.Orden;
			_parametroModel.EstadoId = item.EstadoId;
			_parametroModel.Descripcion = item.Descripcion;
			_parametroModel.ValorExterno = item.ValorExterno;

			StateHasChanged();
		}

		public async Task GenerarNuevo()
		{
			IdEdicion = null;
			esNuevo = true;
			_parametroModel.Limpiar();
			_parametroModel.EstadoId = ParametrosConstants.EstadosGenerales.ACTIVO;
		}

		public async Task GuardarCambios()
		{
			_mensajeError = "";
			_customFormValidator.ClearFormErrors();
			var request = new ParametroValorAddUpdateRequest()
			{
				ParametroValorId = IdEdicion,
				Descripcion = _parametroModel.Descripcion,
				EstadoId = _parametroModel.EstadoId,
				Valor = _parametroModel.Valor,
				ValorExterno = _parametroModel.ValorExterno,
				Orden = _parametroModel.Orden,
				ValorPadreId = _parametroModel.ValorPadreId,
				ParametroId = _parametroId
			};

			var response = await ParametroService.RegistrarActualizar(request);
			if (response.Success)
			{				
				IdEdicion = null;
				esNuevo = false;
				_mensajeError = "";
				await JavascriptService.MostrarMensajeExito();
				await FiltrarParametro(_parametroId);
			}
			else if (response.StatusCode == (int)HttpStatusCode.BadRequest)
			{
				_mensajeError = response.Title;
				_customFormValidator.DisplayFormErrors(response.Errors);
			}
			else
			{
				_mensajeError = response.Title;
				await JavascriptService.MostrarMensajeError();
			}
			StateHasChanged();
		}

		private void ConfirmarEliminacion(string id)
		{
			_valorIdEliminar = id;
			_accion = GeneralConstants.Acciones.ELIMINAR;
			StateHasChanged();
		}

		public async void Eliminar()
		{
			_mensajeError = "";
			_customFormValidator.ClearFormErrors();
			var response = await ParametroService.Eliminar(_valorIdEliminar);
			if (response.Success)
			{
				await JavascriptService.MostrarMensajeExito();
				await FiltrarParametro(_parametroId);
			}
			else
			{
				_mensajeError = response.Title;
				await JavascriptService.MostrarMensajeError();
			}
			StateHasChanged();
		}

		public void Cancelar()
		{
			IdEdicion = null;
			esNuevo = false;
			_mensajeError = "";
		}
		#endregion

	}
}
