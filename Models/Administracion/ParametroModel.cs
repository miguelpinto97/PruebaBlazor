namespace RestauranteVirtual.Web.Models.Administracion
{
	public class ParametroModel
	{		

		public string? Id { get; set; }
		public string? Descripcion { get; set; }
		public string? Valor { get; set; }
		public string? ValorExterno { get; set; }
		public string? ValorPadreId { get; set; }
		public string EstadoId { get; set; }
		public int Orden { get; set; }

		public void Limpiar()
		{
			Id = null;
			Descripcion = null;
			Valor = null;
			ValorExterno = null;
			ValorPadreId = null;
			EstadoId = string.Empty;
			Orden = 0;
		}
	}
}
