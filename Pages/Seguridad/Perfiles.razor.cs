using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Net;
using RestauranteVirtual.Common.Constants;
using RestauranteVirtual.Common.Utils;
using RestauranteVirtual.Dto.Seguridad.MenuOpciones;
using RestauranteVirtual.Dto.Seguridad.Perfiles;
using RestauranteVirtual.Dto.Seguridad.Permisos;
using RestauranteVirtual.Web.Components.Busqueda;
using RestauranteVirtual.Web.Components.Validacion;
using RestauranteVirtual.Web.Models.General;
using RestauranteVirtual.Web.Models.Seguridad;
using RestauranteVirtual.Web.Services.API;
using RestauranteVirtual.Web.Services.Javascript;
using RestauranteVirtual.Web.Utils.Constants;

namespace RestauranteVirtual.Web.Pages.Seguridad
{
	public partial class Perfiles 
    {
        /* Services Implementados */
        [Inject] PerfilService PerfilService { get; set; }
        [Inject] JavascriptService JavascriptService { get; set; }

		protected CustomFormValidator? customFormValidator = new();
		protected Paginacion? _paginacion = new();
		private EditContext _editContext;
		private PerfilBaseInfo _baseInfo = new();
		private string _columnaOrdenada = "";
		private bool _esAscendente = true;
		private string _terminoBusqueda = "";
		private bool _filtroEstadoLoaded = false;
		private bool _treeRenderizado = false;
		private string[] _filtroEstadosId = new string[0];
		private string _mensajeError = "";
		private string _mensajeErrorPermisos = "";
		private int _perfilIdPermisoEdicion = 0;
		private int _perfilIdEliminar = 0;
		public string _accion = "";
		private List<TreeNodeModel> _nodosPermisos = new();
		private List<PerfilDto> _todosElementos = new();
		public List<PerfilDto> _elementosPorPagina = new();
		private List<PerfilDto> _elementosFiltrados = new();
		public PerfilModel _perfilModel = new();

		protected override void OnInitialized()
        {
            _editContext = new EditContext(_perfilModel);
			_baseInfo.Estados = new Dictionary<string, string>();
			_baseInfo.MenuOpciones = new List<MenuOpcionDto>();
			_baseInfo.Permisos = new List<PermisoDto>();
		}        

        protected override async Task OnInitializedAsync()
        {
			_baseInfo = await PerfilService.ObtenerBaseInfo();
			_elementosFiltrados = _todosElementos = await PerfilService.Listar();
			_paginacion.PaginaActual = 1;
			_paginacion.ElementosPorPagina = 10;
			_paginacion.TotalElementos = _elementosFiltrados.Count;
			await _paginacion.CalcularPaginado();
			_nodosPermisos = ConvertirMenuOpcionesEnArbol();
			CambiarPagina();
			await JavascriptService.MoverModal();
			StateHasChanged();
		}

        protected override async void OnAfterRender(bool firstRender)
        {			
			if (_baseInfo.Estados.Count > 0 && !_filtroEstadoLoaded)
			{
				_filtroEstadoLoaded = true;
				await JavascriptService.InicializarMultiselect("ddlFiltroEstado", "FiltrarEstado", DotNetObjectReference.Create(this));
			}
			if(_nodosPermisos.Count > 0 && !_treeRenderizado)
			{
				_treeRenderizado = true;
				await JavascriptService.InicializarTrees("treePermisos");
			}		
		}

		private void CambiarPagina()
		{
			_elementosPorPagina = _elementosFiltrados.Skip(_paginacion.IndiceInicio).Take(_paginacion.IndiceFin - _paginacion.IndiceInicio + 1).ToList();
		}

		private async Task RecalcularTotalElementos()
		{
			_paginacion.TotalElementos = _elementosFiltrados.Count;
			await _paginacion.CalcularPaginado();
			CambiarPagina();
			StateHasChanged();
		}

		#region Filtro Multicolumna

		public async Task Filtrar()
		{
			_elementosFiltrados = _todosElementos
				.Where(x => x.Id.ToString().Buscar(_terminoBusqueda)
							|| x.Nombre.Buscar(_terminoBusqueda)
							|| _terminoBusqueda == null
							|| _terminoBusqueda == "")
				.Where(x => _filtroEstadosId.Contains(x.EstadoId) || _filtroEstadosId.Length == 0)
				.ToList();
			AplicarOrdenamiento();

			await RecalcularTotalElementos();
		}

		private async Task FiltroGeneral(KeyboardEventArgs e)
		{
			if (e.Key != "Enter") return;
			await Filtrar();
		}

		[JSInvokable]
		public async Task FiltrarEstado(string[] estadosId)
		{
			_filtroEstadosId = estadosId;
			await Filtrar();
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

		#region CRUD
		public int IdEdicion = 0;
		public bool esNuevo = false;

		public void ActivarEdicion(int Id)
		{
			esNuevo = false;
			IdEdicion = Id;
			var dto = _elementosPorPagina
				.Where(x => x.Id == IdEdicion)
				.First();

			_perfilModel.Nombre = dto.Nombre;
			_perfilModel.EstadoId = dto.EstadoId;
			_perfilModel.Id = dto.Id;
			StateHasChanged();
		}

		public void GenerarNuevo()
		{
			IdEdicion = 0;
			esNuevo = true;
			_perfilModel.Limpiar();
			_perfilModel.EstadoId = ParametrosConstants.EstadosGenerales.ACTIVO;
		}

		public async void GuardarCambios()
		{
			_mensajeError = "";
			customFormValidator.ClearFormErrors();
			var request = new PerfilAddUpdateRequest()
			{
				PerfilId = IdEdicion,
				EstadoId = _perfilModel.EstadoId,
				Nombre = _perfilModel.Nombre
			};

			var response = await PerfilService.RegistrarActualizar(request);
			if (response.Success)
			{
				_todosElementos = await PerfilService.Listar();
				IdEdicion = 0;
				esNuevo = false;
				_mensajeError = "";
				await JavascriptService.MostrarMensajeExito();
				await Filtrar();
			}
			else if (response.StatusCode == (int)HttpStatusCode.BadRequest)
			{
				_mensajeError = response.Title;
				customFormValidator.DisplayFormErrors(response.Errors);
			}
			else
			{
				_mensajeError = response.Title;
				await JavascriptService.MostrarMensajeError();
			}
			StateHasChanged();
		}

		private void ConfirmarEliminacion(int id)
		{
			_perfilIdEliminar = id;
			_accion = GeneralConstants.Acciones.ELIMINAR;
			StateHasChanged();
		}

		public async void Eliminar()
		{
			_mensajeError = "";
			var response = await PerfilService.Eliminar(_perfilIdEliminar);
			if (response.Success)
			{
				_perfilIdEliminar = 0;
				_todosElementos = await PerfilService.Listar();
				await JavascriptService.MostrarMensajeExito();
				await Filtrar();
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
			IdEdicion = 0;
			esNuevo = false;
		}
		#endregion

		#region Permisos

		private async void CargarPermisos(int perfilId)
		{
			_mensajeErrorPermisos = string.Empty;
			_perfilIdPermisoEdicion = perfilId;
			var permisos = await PerfilService.ListarPermisosPorPerfil(perfilId);
			var ids = permisos.Select(x => x.Id).ToArray();
			await JavascriptService.MarcarNodos("treePermisos", ids);
		}

		private async void GuardarPermisos()
		{
			_mensajeErrorPermisos = "";
			customFormValidator.ClearFormErrors();
			var permisosId = await JavascriptService.ListarNodosSeleccionados("treePermisos", UserInterfaceConstants.TipoNodo.PERMISO);

			if (!permisosId.Any())
			{
				_mensajeErrorPermisos = "Seleccione al menos un permiso";
				StateHasChanged();
				return;
			}

			var request = new PerfilPermisosUpdateRequest()
			{
				PerfilId = _perfilIdPermisoEdicion,
				Permisos = permisosId.ToArray()
			};

			var response = await PerfilService.GuardarPermisos(request);
			if (response.Success)
			{
				_perfilIdPermisoEdicion = 0;
				await JavascriptService.MostrarMensajeExito();
				await JavascriptService.CerrarModal("modalPermisos");
			}
			else
			{
				_mensajeErrorPermisos = response.Title;
				await JavascriptService.MostrarMensajeError();
			}
			StateHasChanged();
		}

		private List<TreeNodeModel> ConvertirMenuOpcionesEnArbol(string idPadre = null)
		{
			List<TreeNodeModel> hijos = _baseInfo.MenuOpciones
				.Where(opcion => opcion.PadreId == idPadre)
				.Select(opcion => new TreeNodeModel
				{
					Id = opcion.Id,
					Nombre = opcion.Nombre,
					Tipo = UserInterfaceConstants.TipoNodo.OPCIONMENU,
					Hijos = ConvertirMenuOpcionesEnArbol(opcion.Id)
				}).ToList();


			hijos.AddRange(_baseInfo.Permisos
					.Where(x => x.MenuOpcionId == idPadre)
					.Select(x => new TreeNodeModel()
					{
						Id = x.Id,
						Nombre = x.Nombre,
						Tipo = UserInterfaceConstants.TipoNodo.PERMISO,
						Hijos = new List<TreeNodeModel>()
					}));



			return hijos.Where(x => TienePermisos(x)).ToList();
		}

		private bool TienePermisos(TreeNodeModel nodo)
		{
			if(nodo.Tipo == UserInterfaceConstants.TipoNodo.OPCIONMENU)
			{
				return nodo.Hijos.Any(x => TienePermisos(x));
			}
			else
			{
				return true;
			}
		}

		#endregion

	}
}
