using Nebula.Core.Services.API;

namespace Core.Services
{
    public class RegistryService
    {
        private RegistryApiClient Client { get; set; }

        public RegistryService(RegistryApiClient client)
        {
            Client = client ?? new RegistryApiClient();
        }


    }
}