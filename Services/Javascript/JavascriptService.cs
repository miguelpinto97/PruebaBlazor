using Microsoft.JSInterop;
using System.Security.Cryptography;
using RestauranteVirtual.Web.Models.Administracion;

namespace RestauranteVirtual.Web.Services.Javascript
{
    public class JavascriptService
    {
        private readonly IJSRuntime JS;

        public JavascriptService(IJSRuntime JS)
        {
            this.JS = JS;
        }

        public async Task<int> MostrarMensajeAlerta(string Mensaje)
        {
            await JS.InvokeVoidAsync("MostrarMensajeAlerta", Mensaje);

            return 0; 
        }
		public async Task<int> MostrarMensajeExito_Swal(string Mensaje)
		{
			await JS.InvokeVoidAsync("MostrarMensajeExito_Swal", Mensaje);

			return 0;
		}
		
		public async Task<int> MostrarMensajeError()
        {
            await JS.InvokeVoidAsync("MostrarMensajeError");

            return 0;
        }
        public async Task<int> MostrarMensajeErrorPersonalizado(string mensaje)
        {
            await JS.InvokeVoidAsync("MostrarErrorPersonalizado", mensaje);

            return 0;
        }

        public async Task<int> MostrarMensajeErrorBloqueante(string Mensaje)
		{
			await JS.InvokeVoidAsync("MostrarMensajeErrorBloqueante", Mensaje);

			return 0;
		}

		public async Task<int> MostrarMensajeExito()
        {
            await JS.InvokeVoidAsync("MostrarMensajeExito");

            return 0;
        }
		public async Task<int> MostrarMensajeExitoPersonalizado(string mensaje)
		{
			await JS.InvokeVoidAsync("MostrarMensajeExitoPersonalizado", mensaje);

			return 0;
		}
		public async Task<int> InicializarSelect2(string IdHtml, string NombreMetodoOnChange, Object dotnet )
        {
            await JS.InvokeVoidAsync("InicializarSelect2", IdHtml, NombreMetodoOnChange, dotnet);
            return 0;
        }

		public async Task<int> InicializarMultiselect(string IdHtml, string NombreMetodoOnChange, Object dotnet)
		{
			await JS.InvokeVoidAsync("InicializarMultiselect", IdHtml, NombreMetodoOnChange, dotnet);
			return 0;
		}

		public async Task<int> InicializarMultiselect(string IdHtml)
		{
			await JS.InvokeVoidAsync("InicializarMultiselectSinEvento", IdHtml);
			return 0;
		}

        public async Task MultiSelectConfig()
        {
            await JS.InvokeVoidAsync("MultiSelectConfig");
        }

        public async Task StopEventPropagation()
        {
            await JS.InvokeVoidAsync("StopPropagation");
        }

        //

        public async Task<int> CheckearMultiSelect(string IdHtml, int[] valores)
        {
            await JS.InvokeVoidAsync("CheckearMultiSelect", IdHtml, valores);
            return 0;
        }
        public async Task<int> CheckearMultiSelect_string(string IdHtml, string[] valores)
        {
            await JS.InvokeVoidAsync("CheckearMultiSelect", IdHtml, valores);
            return 0;
        }

        public async Task<int> DesCheckearMultiSelect(string IdHtml)
        {
            await JS.InvokeVoidAsync("DesCheckearMultiSelect", IdHtml);
            return 0;
        }

        public async Task<int> AbrirModal(string IdHtml)
        {
            await JS.InvokeVoidAsync("AbrirModal", IdHtml);
            return 0;
        }
        public async Task<int> AbrirModalEstatico(string IdHtml)
        {
            await JS.InvokeVoidAsync("AbrirModalEstatico", IdHtml);
            return 0;
        }
        public async Task<int> CerrarModal(string IdHtml)
		{
			await JS.InvokeVoidAsync("CerrarModal", IdHtml);
			return 0;
		}

		public async Task<int> InicializarTrees(string IdHtml)
        {
            await JS.InvokeVoidAsync("InicializarTrees", IdHtml);
            return 0;
        }

		public async Task<int> MarcarNodos(string IdHtml, string[] valores)
		{
			await JS.InvokeVoidAsync("MarcarNodos", IdHtml, valores);
			return 0;
		}

		public async Task<List<string>> ListarNodosSeleccionados(string IdHtml, string tipo)
		{
			var nodos = await JS.InvokeAsync<List<string>>("ListarNodosSeleccionados", IdHtml, tipo);
			return nodos;
		}

		public async Task<int> InicializarCalendario(string IdHtml)
		{
			await JS.InvokeVoidAsync("InicializarCalendario", IdHtml);
			return 0;
		}

        public async Task<string[]> ObtenerSeleccionadosMultiSelect(string IdHtml)
        {
                
            return await JS.InvokeAsync<string[]>("ObtenerSeleccionadosMultiSelect", IdHtml);
        }
        public async Task<int> CheckboxAsignarValor(string IdHtml, bool check)
        {
            await JS.InvokeVoidAsync("CheckboxAsignarValor", IdHtml, check);
            return 0;
        }

		public async Task<int> InicializarTag(string IdHtml)
		{
			await JS.InvokeVoidAsync("InicializarTag", IdHtml);
			return 0;
		}

		public async Task<string> ObtenerValoresTag(string IdHtml)
		{
			return await JS.InvokeAsync<string>("ObtenerValoresTag", IdHtml);
		}

		public async Task<int> InicializarSelect2(string IdHtml)
		{
			await JS.InvokeVoidAsync("InicializarSelect2", IdHtml);
			return 0;
		}
		public async Task<string[]> ObtenerValoresSelect2(string IdHtml)
		{			
			return await JS.InvokeAsync<string[]>("ObtenerValoresSelect2", IdHtml); 
		}
		public async Task<string> ObtenerValorSelect2(string IdHtml)
		{
			return await JS.InvokeAsync<string>("ObtenerValorSelect2", IdHtml);
		}
		public async Task<int> ObtenerValorSelect2_Int(string IdHtml)
		{
			return int.Parse(await JS.InvokeAsync<string>("ObtenerValorSelect2", IdHtml));
		}
		public async Task DescargarArchivo(string fileName, string mimeType, byte[] fileContent)
		{
			await JS.InvokeVoidAsync("DownloadFile", fileName, mimeType, fileContent);
		}

		//public async Task<int> InicializarCalendario2(string IdHtml, List<CalendarioEventoModel> evento, string NombreMetodoOnClick, Object dotnet)
		//{
		//	await JS.InvokeVoidAsync("InicializarCalendario2", IdHtml, evento, NombreMetodoOnClick, dotnet);
		//	return 0;
		//}

		public async Task<int> EventoAgregado(string IdHtml,string fecha, string titulo, string color)
		{
			await JS.InvokeVoidAsync("EventoAgregado",IdHtml, fecha, titulo, color);
			return 0;
		}

		public async Task<int> SeleccionarSelect2(string IdHtml, string[] valores)
		{
			await JS.InvokeVoidAsync("SeleccionarSelect2", IdHtml, valores);
			return 0;
		}
		public async Task<int> SetearSelect2(string IdHtml, string valor)
		{
			await JS.InvokeVoidAsync("SetearSelect2", IdHtml, valor);
			return 0;
		}

		public async Task Imprimir(string IdHtml)
		{
			await JS.InvokeVoidAsync("Imprimir", IdHtml);
		}

        public async Task Imprimir_PrintThis(string IdHtml)
        {
            await JS.InvokeVoidAsync("Imprimir_PrintThis", IdHtml);
        }

        public async Task Imprimir_PrintArea(string IdHtml)
        {
            await JS.InvokeVoidAsync("Imprimir_PrintArea", IdHtml);
        }
        public async Task IniciarTooltip()
		{
			await JS.InvokeVoidAsync("IniciarTooltip");
		}
		public async Task MoverModal()
		{
			await JS.InvokeVoidAsync("MoverModal");

		}
		public async Task GenerarClickEnElemento(string IdHtml)
		{
			await JS.InvokeVoidAsync("generarClickEnElemento", IdHtml);
		}
		public async Task DispararMouseOver(string IdHtml)
		{
			Console.WriteLine("Disparado");
			await JS.InvokeVoidAsync("dispararMouseOver", IdHtml);
		}
        public async Task EfectoEliminacion(string IdHtml)
        {
            await JS.InvokeVoidAsync("EfectoEliminacion", IdHtml);
        }
        public async Task AsignarClase(string IdHtml)
        {
            await JS.InvokeVoidAsync("AsignarClase", IdHtml);
        }
        public async Task RemoverClase(string IdHtml)
        {
            await JS.InvokeVoidAsync("RemoverClase", IdHtml);
        }
        public async Task RemoverClaseMultiple(string IdHtmlParcial)
        {
            await JS.InvokeVoidAsync("RemoverClaseMultiple", IdHtmlParcial);
        }
    }
}
