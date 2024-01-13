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

        public List<Authlayer> UpdateAuthlayers(List<Authlayer> authlayers)
        {
            var updatedAuthLayers = new List<Authlayer>();
            authlayers.ForEach(authlayer =>
            {
                var existingAuthLayer = _dbContext.Authlayers.Find(authlayer.Id);
                if (existingAuthLayer != null)
                {
                    // 使用 SetValues 來只更新不為 null 的屬性
                    _dbContext.Entry(existingAuthLayer).CurrentValues.SetValues(authlayer);
                    updatedAuthLayers.Add(existingAuthLayer);
                }
            });
            // 將變更保存到資料庫
            _dbContext.SaveChanges();
            return updatedAuthLayers;
        }
    }
}
