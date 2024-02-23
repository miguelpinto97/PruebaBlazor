namespace RestauranteVirtual.Web.Models.Seguridad
{
	public class PerfilModel
	{
		public PerfilModel()
		{
			Limpiar();
		}

		public int Id { get; set; }
		public string Nombre { get; set; }
		public string EstadoId { get; set; }

		public void Limpiar()
		{
			Id = 0;
			Nombre = string.Empty;
			EstadoId = string.Empty;
		}
	}
}
