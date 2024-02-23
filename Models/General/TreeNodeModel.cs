namespace RestauranteVirtual.Web.Models.General
{
	public class TreeNodeModel
	{
		public string Id { get; set; }
		public string Nombre { get; set; }
		public string Tipo { get; set; }
		public bool Seleccionado { get; set; }
		public List<TreeNodeModel> Hijos { get; set; }
	}
}
