using handover_api.Models;

namespace handover_api.Service
{
    public class AuthLayerService
    {
        private readonly HandoverContext _dbContext;

        public AuthLayerService(HandoverContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Authlayer> GetAllAuthlayers()
        {
            return _dbContext.Authlayers.ToList();
        }
    }
}
