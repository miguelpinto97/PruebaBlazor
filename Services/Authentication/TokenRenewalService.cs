namespace RestauranteVirtual.Web.Services.Authentication
{
	public class TokenRenewalService
	{
		private readonly CustomAuthenticationStateProvider _authStateProvider;
		private readonly IAuthService _authService;
		private readonly ILogger<TokenRenewalService> logger;
		private Timer timer;
		private const int RenewalTimeInMinutes = 5; // Renovar el token cada 5 minutos

		public TokenRenewalService(CustomAuthenticationStateProvider authStateProvider, ILogger<TokenRenewalService> logger,
			IAuthService authService)
		{
			_authStateProvider = authStateProvider;
			this.logger = logger;
			_authService= authService;

			timer = new Timer(RenewToken, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(RenewalTimeInMinutes));
		}

		private async void RenewToken(object state)
		{
			try
			{
				var authState = await _authStateProvider.GetAuthenticationStateAsync();

				if (authState.User.Identity.IsAuthenticated)
				{
					//var tokenExp = authState.User.FindFirst("exp")?.Value;
					await _authService.Refresh();					
				}
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Token renewal failed with exception.");
			}
		}
	}

}
