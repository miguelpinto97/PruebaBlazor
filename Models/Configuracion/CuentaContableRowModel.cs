namespace RestauranteVirtual.Web.Models.Contabilidad.Configuracion
{
	public class CuentaContableRowModel
	{
		public bool Seleccionado { get; set; }
		public string Id { get; set; }
		public string Descripcion { get; set; }
		public string EstadoId { get; set; }
		public string Estado { get; set; }
		public string? PadreId { get; set; }
		public int Nivel { get; set; }
		public int Orden { get; set; }
		public bool Bloqueado { get; set; }
		public string? SubCuentaAutomaticaId { get; set; }
	}
}
