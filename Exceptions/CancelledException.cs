using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gw2LogParser.Exceptions
{
    
    class CancelledException : Exception
    {
        public GridRow Row { get; }

        public CancelledException(GridRow row) : base("Operation cancelled")
        {
            Row = row;
        }

        public CancelledException(GridRow row, Exception inner) : base("Operation cancelled", inner)
        {
            Row = row;
        }
    }
}
