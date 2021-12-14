using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

using Dx29.Web.Services;

namespace Dx29.Web.Pages
{
    public class AgreementModel : PageModel
    {
        private readonly ILogger<PrivacyModel> _logger;

        public AgreementModel(ILogger<PrivacyModel> logger, DocumentsService documentsService)
        {
            _logger = logger;
            DocumentsService = documentsService;
        }

        public DocumentsService DocumentsService { get; }
        public string DataAgreement { get; set; }

        public async Task OnGet(string language = "en")
        {
            DataAgreement = await DocumentsService.Download("TermsAndConditions", "dataProcessingAgreement.txt", language);
        }
    }
}
