using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMG.Infrastructure.Analytics.Objects.DB.Base
{
    public static class BaseObject
    {
        public static long InvalidPrimaryKey { get { return -1; } }
        public static bool IsPrimaryKeyValid(object pk)
        {
            if (pk is long && (long)pk != InvalidPrimaryKey)
                return true;
            else if (pk == null)
                return false;
            else if (pk is Guid && (Guid)pk != Guid.Empty)
                return true;
            return false;
        }
    }
}
