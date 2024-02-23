namespace RestauranteVirtual.Web.Models.Configuracion
{
    public class PeriodoModel
    {
        public int Anio { get; set; }
        public string Mes { get; set; }

        public void Limpiar()
        {
            Anio = 0;
            Mes = string.Empty;
        }
    }

    
}
