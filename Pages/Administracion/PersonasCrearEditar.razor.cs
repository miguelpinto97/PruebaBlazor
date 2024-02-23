using RestauranteVirtual.Dto.Common;
using RestauranteVirtual.Web.Components.Validacion;
using RestauranteVirtual.Web.Services.Javascript;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using System.Net;
using System.Text.RegularExpressions;
using RestauranteVirtual.Web.Services.API;
using RestauranteVirtual.Dto.Alumno;
using RestauranteVirtual.Common.Constants;

namespace RestauranteVirtual.Web.Pages.Administracion
{
    public partial class PersonasCrearEditar
    {
        [Inject] JavascriptService JavascriptService { get; set; }
        [Inject] AlumnoService AlumnoService { get; set; }
		[Inject] PersonaService PersonaService { get; set; }
		[Inject] NavigationManager NavManager { get; set; }


        [Parameter] public int IdPersonaActualizar { get; set; }
		[Parameter] public string TipoVista { get; set; } = "";
		[Parameter] public string AccionVista { get; set; } = "";

		private string? MensajeError = "";

        /* Componentes */
        protected CustomFormValidator customFormValidator = new();
        protected CustomFormValidator customFormValidatorCuenta = new();
        private EditContext _editContext;
        private EditContext _editContextCuenta;

        public string AsignacionCursoId { get; set; } = "";
		public string AsignacionSalonId { get; set; } = "";

		public PersonaAddUpdateRequest AlumnoRequest { get; set; } = new PersonaAddUpdateRequest();
        public AlumnoBaseInfo BaseInfo = new AlumnoBaseInfo();

        protected override void OnInitialized()
        {
            _editContext = new EditContext(AlumnoRequest);
        }
        protected override async Task OnInitializedAsync()
        {

			BaseInfo = await AlumnoService.ObtenerAlumnoBaseInfo();

			if (AccionVista == "Actualizar")
			{
				string RolId = "";

				switch (TipoVista)
				{
					case "Alumnos":
						RolId = ParametrosConstants.RolesPersonas.ALUMNO;
						break;
					case "Profesores": RolId = ParametrosConstants.RolesPersonas.PROFESOR; break;
					case "Padres": RolId = ParametrosConstants.RolesPersonas.PADRE_MADRE; break;
					case "Admin": RolId = ParametrosConstants.RolesPersonas.PERSONAL_ADMINISTRATIVO; break;
					default: return;
				}

				AlumnoRequest = await PersonaService.ObtenerDatos(IdPersonaActualizar, RolId);
			}

			StateHasChanged();
        }


        public string Accion = "";
        public int IdEdicion = 0;
        public bool esNuevo = false;
        public int NuevoID = -1;


        public void Limpiar_Cancelar()
        {
            IdEdicion = 0;
            esNuevo = false;
            customFormValidatorCuenta.ClearFormErrors();
        }


        public async void Configurar()
        {
            MensajeError = "";
            customFormValidator.ClearFormErrors();

            ApiResponse response = new ApiResponse();

			switch (TipoVista)
			{
				case "Alumnos": {
                        AlumnoRequest.IdPadre = BaseInfo.Padres.FirstOrDefault(x => x.NumeroWhatsapp == AlumnoRequest.NumeroWhatsappPadre)?.Id;
                        response = await AlumnoService.RegistrarActualizar(AlumnoRequest); 
                    }  break;
				case "Profesores": response = await PersonaService.RegistrarActualizarProfesor(AlumnoRequest); break;
				case "Padres": response = await PersonaService.RegistrarActualizarPadre(AlumnoRequest); break;
				case "Admin": response = await PersonaService.RegistrarActualizarAdmin(AlumnoRequest); break;
				default: return;
			}


            if (response.Success)
            {

                NavManager.NavigateTo($"Administracion/{TipoVista}");
                await Task.Delay(500);
                await JavascriptService.MostrarMensajeExito();
            }
            else if (response.StatusCode == (int)HttpStatusCode.BadRequest)
            {
                MensajeError = response.Title;
                customFormValidator.DisplayFormErrors(response.Errors);
            }
            else
            {
                MensajeError = response.Title;
            }

            StateHasChanged();
        }

        public async void AgregarAsignacion()
        {
            if (!BaseInfo.Cursos.ContainsKey(AsignacionCursoId))
            {
				await JavascriptService.MostrarMensajeErrorPersonalizado("Debe seleccionar un curso.");
                return;
			}
			if (!BaseInfo.Salones.ContainsKey(AsignacionSalonId))
			{
				await JavascriptService.MostrarMensajeErrorPersonalizado("Debe seleccionar un salón.");
                return;
			}



            if (!AlumnoRequest.AsignacionesCursoSalon.Any(x=> x.SalonId == AsignacionSalonId & x.CursoId == AsignacionCursoId))
            {
				PersonaCursoSalonDto NuevaAsignacion = new PersonaCursoSalonDto();
				NuevaAsignacion.CursoId = AsignacionCursoId;
				NuevaAsignacion.SalonId = AsignacionSalonId;
                
                AlumnoRequest.AsignacionesCursoSalon.Add(NuevaAsignacion);
            }
            else
            {
                await JavascriptService.MostrarMensajeErrorPersonalizado("Asignación realizada previamente.");
            }
        }
    }
}
