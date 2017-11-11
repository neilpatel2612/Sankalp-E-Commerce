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
    public partial class SKL_ColorCodeService : ISKL_ColorCodeService
    {
        private const string colorCode_BY_ID_KEY = "Nop.colorCode.id-{0}";
        #region Fields

        private readonly IRepository<SKL_ColorCode> _colorCodeRepository;
        private readonly IWorkContext _workContext;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;
        #endregion

        #region Ctor

        public SKL_ColorCodeService(ICacheManager cacheManager,
            IRepository<SKL_ColorCode> colorCodeRepository,
            IWorkContext workContext,
            IEventPublisher eventPublisher)
        {
            this._cacheManager = cacheManager;
            this._colorCodeRepository = colorCodeRepository;
            this._workContext = workContext;
            this._eventPublisher = eventPublisher;
        }

        #endregion
        public void DeleteColorCode(SKL_ColorCode colorCode)
        {
            if (colorCode == null)
                throw new ArgumentNullException("colorCode");
            _colorCodeRepository.Delete(colorCode);

            string key = string.Format(colorCode_BY_ID_KEY, colorCode.Id);
            _cacheManager.Remove(key);
            //event notification
            _eventPublisher.EntityDeleted(colorCode);
        }
        

        public SKL_ColorCode GetColorCodeById(int Id)
        {
            if (Id == 0)
                return null;

            string key = string.Format(colorCode_BY_ID_KEY, Id);
            return _cacheManager.Get(key, () => _colorCodeRepository.GetById(Id));
        }
        

        public void InsertColorCode(SKL_ColorCode colorCode)
        {
            if (colorCode == null)
                throw new ArgumentNullException("colorCode");

            _colorCodeRepository.Insert(colorCode);

            //event notification
            _eventPublisher.EntityInserted(colorCode);
        }
        

        public void UpdateColorCode(SKL_ColorCode colorCode)
        {
            if (colorCode == null)
                throw new ArgumentNullException("colorCode");

            _colorCodeRepository.Update(colorCode);
            string key = string.Format(colorCode_BY_ID_KEY, colorCode.Id);
            _cacheManager.Remove(key);
            //event notification
            _eventPublisher.EntityInserted(colorCode);
        }
        public IPagedList<SKL_ColorCode> GetAllColorCode(string caderName = "", int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _colorCodeRepository.Table;
            //paging
            return new PagedList<SKL_ColorCode>(query.ToList(), pageIndex, pageSize);
        }
    }
}
