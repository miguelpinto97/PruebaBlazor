namespace RestauranteVirtual.Web.Models.Administracion
{
	public class MenuOpcionModel
	{
		public string Id { get; set; }
		public string Nombre { get; set; }
		public string? Url { get; set; }
		public string? Icono { get; set; }
		public List<MenuOpcionModel> Opciones { get; set; }
		public int Orden { get; set; }
	}
}
