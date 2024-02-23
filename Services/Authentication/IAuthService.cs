using RestauranteVirtual.Dto.Common;
using RestauranteVirtual.Dto.Seguridad.Autenticacion;
using RestauranteVirtual.Dto.Seguridad.Permisos;
using RestauranteVirtual.Web.Models.Auth;

namespace RestauranteVirtual.Web.Services.Authentication
{
    public interface IAuthService
    {
        public Task<ApiResponse> Login(LoginModel loginModel);
        public Task Logout();
        public Task Refresh();
        public Task<List<PermisoDto>> ListarPermisos();
        public Task<ApiResponse> SolicitarRecuperarContrasena(ResetRequest request);
        public Task<ApiResponse> CambiarContrasena(ChangeRequest request);
        public Task<DateTime> ObtenerFecha();
        public Task<bool> ValidarPermisos(string[] permisos);
        public Task GuardarPermisos();

	}
}
