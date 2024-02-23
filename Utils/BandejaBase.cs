using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using RestauranteVirtual.Web.Utils.Constants;

namespace RestauranteVirtual.Web.Utils
{
	public class BandejaBase<T> : ComponentBase
	{
		protected List<T>? _elementosCompleto;
		protected List<T>? _elementosFiltrados;
		protected List<T>? _elementosPorPagina;
		protected string _columnaOrdenada = "";
		protected bool _ordenAscendente = true;
		protected string? _terminoBusqueda = "";
		protected string? _mensajeError = "";
		protected string _accion = "";
		protected bool _seleccionarTodos = false;
		protected int _indiceInicio;
		protected int _indiceFin;
		protected int _totalElementos;
		protected EstadoFormulario _estadoFormulario;

		public virtual async Task Filtrar()
		{
			
		}

		protected async Task FiltroRapido(KeyboardEventArgs e)
		{
			if (e.Key != "Enter") return;
			await Filtrar();
		}		

		protected async Task CalcularElementosPorPagina()
		{
			if (_elementosFiltrados == null) return;
			_elementosPorPagina = _elementosFiltrados
									.Skip(_indiceInicio)
									.Take(_indiceFin - _indiceInicio + 1)
									.ToList();
		}

		protected async Task Ordenamiento(string nombreColumna, bool esAscendente)
		{
			if (_elementosFiltrados == null) return;		

			_ordenAscendente = esAscendente;
			_columnaOrdenada = nombreColumna;
			await Ordenar();
		}

		protected async Task Ordenar()
		{
			if (!string.IsNullOrEmpty(_columnaOrdenada) && _elementosFiltrados != null)
			{
				_elementosFiltrados = _ordenAscendente
			  ? _elementosFiltrados.OrderBy(x => x.GetType().GetProperty(_columnaOrdenada)?.GetValue(x, null)).ToList()
			  : _elementosFiltrados.OrderByDescending(x => x.GetType().GetProperty(_columnaOrdenada)?.GetValue(x, null)).ToList();
			}			
		}

		public void Dispose()
		{
			
		}
	}
}
