using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using System.Security.Claims;

namespace Blazor.Server.Pages
{
    partial class HomePage : ComponentBase
    {
        [CascadingParameter] protected Task<AuthenticationState> _authenticationStateTask { get; set; }
        [Inject] protected NavigationManager _navigationManager { get; set; }
        protected override async Task OnInitializedAsync()
        {
            var user = (await _authenticationStateTask).User;
            if (user != null && user.Identity != null)
            {
                //取得租户ID
                var TenantId_ojb = user.Claims.FirstOrDefault(x => x.Type == "TenantId");
                var TenantId = TenantId_ojb != null ? TenantId_ojb.Value.ToString() : "";

            }
        }
    }
}
