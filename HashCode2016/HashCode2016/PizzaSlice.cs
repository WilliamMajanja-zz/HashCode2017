using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashCode2016
{
    public class PizzaSlice
    {
        public int RowStart { get; set; }
        public int RowEnd { get; set; }
        public int ColumnStart { get; set; }
        public int ColumnEnd { get; set; }
        public bool IsValid { get; set; }

        public PizzaSlice()
        {
            RowStart = -1;
            RowEnd = -1;
            ColumnStart = -1;
            ColumnEnd = -1;
        }
    }
}
