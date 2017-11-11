using Nop.Core;
using Nop.Core.Domain.Sankalp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.Sankalp
{
    public partial interface ISKL_ColorCodeService
    {
        void DeleteColorCode(SKL_ColorCode colorCode);
        SKL_ColorCode GetColorCodeById(int Id);
        void InsertColorCode(SKL_ColorCode colorCode);
        void UpdateColorCode(SKL_ColorCode colorCode);
        IPagedList<SKL_ColorCode> GetAllColorCode(string colorCodeName = "", int pageIndex = 0, int pageSize = int.MaxValue);
    }
}
