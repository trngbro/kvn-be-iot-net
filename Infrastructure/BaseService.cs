using Common.UnitOfWork.UnitOfWorkPattern;
using DomainService.Interfaces.MQTT;
using log4net;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure
{
    public abstract class BaseService
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IMemoryCache _memoryCache;
        protected readonly ILog Log = LogManager.GetLogger(typeof(BaseService));

        public BaseService(IUnitOfWork unitOfWork, IMemoryCache memoryCache)
        {
            this._unitOfWork = unitOfWork;
            this._memoryCache = memoryCache;
        }
    }
}
