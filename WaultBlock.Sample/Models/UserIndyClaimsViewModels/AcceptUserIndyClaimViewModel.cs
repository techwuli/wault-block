using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;

namespace WaultBlock.Sample.Models.UserIndyClaimsViewModels
{
    public class AcceptUserIndyClaimViewModel : PageModel
    {
        private Guid _cacheKey = Guid.NewGuid();

        private IMemoryCache _cache;

        public AcceptUserIndyClaimViewModel(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        private Dictionary<string, string> _attributeValues;

        [BindProperty]
        public Dictionary<string, string> AttributeValues
        {
            get; set;
        }
    }
}
