using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Sankalp
{
    public partial class SKL_Cader : BaseEntity
    {
        public string Cader { get; set; }
        public int ColorCodeId { get; set; }
    }
}
