using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Sankalp;
using Nop.Core.Data;
using Nop.Core;
using Nop.Services.Events;
using Nop.Core.Caching;

namespace Nop.Services.Sankalp
{
    public partial class SKL_CaderService : ISKL_CaderService
    {
        private const string CADER_BY_ID_KEY = "Nop.cader.id-{0}";
        #region Fields

        private readonly IRepository<SKL_Cader> _caderRepository;
        private readonly IWorkContext _workContext;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;
        #endregion

        #region Ctor

        public SKL_CaderService(ICacheManager cacheManager,
            IRepository<SKL_Cader> caderRepository,
            IWorkContext workContext,
            IEventPublisher eventPublisher)
        {
            this._cacheManager = cacheManager;
            this._caderRepository = caderRepository;
            this._workContext = workContext;
            this._eventPublisher = eventPublisher;
        }

        #endregion

        public void DeleteCader(SKL_Cader cader)
        {
            if (cader == null)
                throw new ArgumentNullException("cader");
            _caderRepository.Delete(cader);

            string key = string.Format(CADER_BY_ID_KEY, cader.Id);
            _cacheManager.Remove(key);
            //event notification
            _eventPublisher.EntityDeleted(cader);
        }

        public SKL_Cader GetCaderById(int Id)
        {
            if (Id == 0)
                return null;

            string key = string.Format(CADER_BY_ID_KEY, Id);
            return _cacheManager.Get(key, () => _caderRepository.GetById(Id));
        }

        public void InsertCader(SKL_Cader cader)
        {
            if (cader == null)
                throw new ArgumentNullException("cader");

            _caderRepository.Insert(cader);
            
            //event notification
            _eventPublisher.EntityInserted(cader);
        }

        public void UpdateCader(SKL_Cader cader)
        {
            if (cader == null)
                throw new ArgumentNullException("cader");

            _caderRepository.Update(cader);
            string key = string.Format(CADER_BY_ID_KEY, cader.Id);
            _cacheManager.Remove(key);
            //event notification
            _eventPublisher.EntityInserted(cader);
        }

        public IPagedList<SKL_Cader> GetAllCader(string caderName = "", int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _caderRepository.Table;
            //paging
            return new PagedList<SKL_Cader>(query.ToList(), pageIndex, pageSize);
        }
    }
}
