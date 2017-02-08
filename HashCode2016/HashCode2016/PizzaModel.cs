using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashCode2016
{
    public class PizzaModel
    {
        public int RowCount { get; set; }

        public int ColumnCount { get; set; }

        public int MinimumIngredientCount { get; set; }

        public int MaxIngredientsPerSlice { get; set; }

        public Ingredient[,] Ingredients { get; set; }
    }
}
