using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dx29.Web.Services
{
    public class ManagementClient
    {
        public ManagementClient(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        public HttpClient HttpClient { get; }

        public async Task DeleteUserCasesAsync(string userId)
        {
            try
            {
                await HttpClient.DeleteAsync($"Management/user/{userId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
