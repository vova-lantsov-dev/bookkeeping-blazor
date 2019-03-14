﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace BookkeepingNasheDetstvo.Client.Components.ActionComponents
{
    public class SubmitComponentBase : BaseComponent
    {
        [Parameter]
        protected string Base { get; set; }

        protected override Task OnInitAsync()
        {
            return CheckAccessToken();
        }

        protected Task<string> SubmitAsync(object value)
        {
            return Post<string>($"/api/{Base}", value);
        }

        protected async Task DeleteAsync(string id, string navigateTo)
        {
            if (await Post($"/api/{Base}/delete", id))
                UriHelper.NavigateTo(navigateTo);
        }
    }
}
