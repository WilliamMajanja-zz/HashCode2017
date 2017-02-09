using System;
using GeneticSharp.Domain.Randomizations;
using System.Collections.Generic;

namespace HashCode2016
{
    public static class PizzaSlicer
    {
        private const int MinimumSliceCount = 3;

        public static IList<PizzaSlice> Slice(PizzaModel pizzaModel)
        {
            var slices = new List<PizzaSlice>();

            var rowSpan = RandomizationProvider.Current.GetInt(0, pizzaModel.RowCount);
            var columnSpan = RandomizationProvider.Current.GetInt(0, pizzaModel.ColumnCount);

            var max = Math.Max(pizzaModel.RowCount, pizzaModel.ColumnCount);
            var rowOffset = RandomizationProvider.Current.GetInt(0, max);
            var columnOffset = RandomizationProvider.Current.GetInt(0, max);

            for (int i = rowOffset; i < pizzaModel.RowCount; i += rowSpan + 1)
            {
                for (int j = columnOffset; j < pizzaModel.ColumnCount; j += columnSpan + 1)
                {
                    var slice = new PizzaSlice
                    {
                        RowStart = i,
                        RowEnd = i + rowSpan,
                        ColumnStart = j,
                        ColumnEnd = j + columnSpan
                    };

                    slices.Add(slice);
                }
            }

            // ensure we have atleast enough slices/genes for ordering and mutations
            if (slices.Count <= MinimumSliceCount)
            {
                slices.AddRange(Slice(pizzaModel));
            }

            return slices;
        }
    }
}
