using Nop.Core;
using Nop.Core.Domain.Sankalp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.Sankalp
{
    public partial interface ISKL_CaderService
    {
        void DeleteCader(SKL_Cader cader);
        SKL_Cader GetCaderById(int Id);
        void InsertCader(SKL_Cader cader);
        void UpdateCader(SKL_Cader cader);
        IPagedList<SKL_Cader> GetAllCader(string caderName = "", int pageIndex = 0, int pageSize = int.MaxValue);
    }
}
