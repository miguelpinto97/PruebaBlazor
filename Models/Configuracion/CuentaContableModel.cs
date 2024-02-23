using System.ComponentModel.DataAnnotations;

namespace RestauranteVirtual.Web.Models.Contabilidad.Configuracion
{
	public class CuentaContableModel
	{
		public string Id { get; set; }
		public string Descripcion { get; set; }
		public string EstadoId { get; set; }
		public string? PadreId { get; set; }
		public string NaturalezaId { get; set; }

		public string? SubCuentaAutomaticaId { get; set; }
		public int LongitudPrimerCorrelativo { get; set; }
		public int LongitudSegundoCorrelativo { get; set; }

		public bool ReferenciaContrato { get; set; }
		public bool ReferenciaFecha { get; set; }
		public bool ReferenciaMedioPago { get; set; }
		public bool ReferenciaTipoDocumento { get; set; }
		public bool ReferenciaNroDocumento { get; set; }
		public bool ReferenciaGrupo { get; set; }
		public int UsuarioAuditoriaId { get; set; }
		public int? GrupoId { get; set; }
		public string? ProgramaId { get; set; }
		public string? Programa { get; set; }
		public string? Grupo { get; set; }

		public void Clear()
		{
			Id = string.Empty;
			Descripcion = string.Empty;
			EstadoId = string.Empty;
			PadreId = null;
			NaturalezaId = string.Empty;
			SubCuentaAutomaticaId = null;
			LongitudPrimerCorrelativo = 0;
			LongitudSegundoCorrelativo= 0;
			ReferenciaContrato = false;
			ReferenciaFecha = false;
			ReferenciaMedioPago = false;
			ReferenciaTipoDocumento = false;
			ReferenciaGrupo = false;
			ReferenciaNroDocumento = false;
			GrupoId = null;
			ProgramaId = null;

		}
	}
}
