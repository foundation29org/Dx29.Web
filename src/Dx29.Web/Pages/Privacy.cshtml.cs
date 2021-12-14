using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

using Dx29.Web.Services;

namespace Dx29.Web.Pages
{
    public class PrivacyModel : PageModel
    {
        private readonly ILogger<PrivacyModel> _logger;

        public PrivacyModel(ILogger<PrivacyModel> logger, DocumentsService documentsService)
        {
            _logger = logger;
            DocumentsService = documentsService;
        }

        public DocumentsService DocumentsService { get; }
        public string TermsAndConditions { get; set; }

        public async Task OnGet(string language = "en")
        {
            TermsAndConditions = await DocumentsService.Download("TermsAndConditions", "termsAndConditions.txt", language);
        }
    }
}
