using System.Net.Http.Headers;

namespace SistemaPDV.Web.Services
{
    public class AuthenticationHandler : DelegatingHandler
    {
        private readonly IAuthWebService _authService;

        public AuthenticationHandler(IAuthWebService authService)
        {
            _authService = authService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = _authService.GetToken();
            
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
