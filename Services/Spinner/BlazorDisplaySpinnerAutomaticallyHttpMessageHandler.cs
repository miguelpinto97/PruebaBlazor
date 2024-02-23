using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using RestauranteVirtual.Web.Services.Authentication;
using Blazored.LocalStorage;
using System.Security.Claims;

namespace RestauranteVirtual.Web.Services.Spinner
{
    public class BlazorDisplaySpinnerAutomaticallyHttpMessageHandler : DelegatingHandler
    {
        private readonly SpinnerService _spinnerService;
		private readonly NavigationManager _navigationManager;
		private readonly ILocalStorageService _localStorage;
		public BlazorDisplaySpinnerAutomaticallyHttpMessageHandler(SpinnerService spinnerService, NavigationManager navigationManager,
			 ILocalStorageService localStorage)
        {
            _spinnerService = spinnerService;
			_navigationManager = navigationManager;
            _localStorage= localStorage;
		}
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _spinnerService.Show();
			//var savedToken = await _localStorage.GetItemAsync<string>("authToken");

			//if (string.IsNullOrWhiteSpace(savedToken) && !request.RequestUri.AbsolutePath.Contains("api/Auth"))
			//{
			//	_navigationManager.NavigateTo("/Login");
			//	throw new UnauthorizedAccessException("El usuario no está autenticado");
			//}

			var response = await base.SendAsync(request, cancellationToken);
            _spinnerService.Hide();
            return response;
        }
    }
}
