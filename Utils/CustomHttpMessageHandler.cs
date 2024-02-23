using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Headers;
using System.Web;
using RestauranteVirtual.Dto.Common;
using RestauranteVirtual.Web.Services.Spinner;

namespace RestauranteVirtual.Web.Utils
{
	public class CustomHttpMessageHandler : DelegatingHandler
	{
		private readonly SpinnerService _spinnerService;
		private readonly NavigationManager _navigationManager;
		private readonly ILocalStorageService _localStorage;
		private string[] _pathsSinSpinner = { 
			"api/Auth/refresh",
			"api/CuentasContables/StatsInfo"
		};
		public CustomHttpMessageHandler(SpinnerService spinnerService, NavigationManager navigationManager,
			 ILocalStorageService localStorage)
		{
			_spinnerService = spinnerService;
			_navigationManager = navigationManager;
			_localStorage = localStorage;
		}
		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			
			if (!_pathsSinSpinner.Any(x => request.RequestUri.AbsolutePath.Contains(x)))
			{
				_spinnerService.Show();
			}
			var savedToken = await _localStorage.GetItemAsync<string>("authToken");

			//if (string.IsNullOrWhiteSpace(savedToken) && !request.RequestUri.AbsolutePath.Contains("api/Auth") && !_navigationManager.Uri.Contains("/Login?"))
			//{
			//	_navigationManager.NavigateTo("/Login?returnUrl=" + HttpUtility.UrlEncode(_navigationManager.ToBaseRelativePath(_navigationManager.Uri)), forceLoad: true);
   //             return null;
			//}
			//else if (!string.IsNullOrWhiteSpace(savedToken))
			//{
			//	request.Headers.Authorization = new AuthenticationHeaderValue("bearer", savedToken);
			//}
			var response = await base.SendAsync(request, cancellationToken);
			_spinnerService.Hide();
			//if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized && request.RequestUri.AbsolutePath.EndsWith("api/Auth/Permisos"))
			//{
			//	_navigationManager.NavigateTo("/Login?returnUrl=" + HttpUtility.UrlEncode(_navigationManager.ToBaseRelativePath(_navigationManager.Uri)), forceLoad: true);
			//	return response;
			//}
			return response;
		}

	}
}
