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

            var rowOffset = RandomizationProvider.Current.GetInt(0, 2);
            var columnOffset = RandomizationProvider.Current.GetInt(0, 2);

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

                    /*
                    if (slice.RowStart == 0 && slice.RowEnd == 2 && slice.ColumnStart == 0 && slice.ColumnEnd == 1)
                    {
                        System.Console.WriteLine("Start");
                    }

                    if (slice.RowStart == 0 && slice.RowEnd == 2 && slice.ColumnStart == 2 && slice.ColumnEnd == 2)
                    {
                        System.Console.WriteLine("Middle");
                    }

                    if (slice.RowStart == 0 && slice.RowEnd == 2 && slice.ColumnStart == 3 && slice.ColumnEnd == 4)
                    {
                        System.Console.WriteLine("End");
                    }
                    */

                    slices.Add(slice);
                }
            }

            // ensure we have atleast enough slices/genes for ordering and mutations
            if (slices.Count <= MinimumSliceCount)
            {
                while(slices.Count != MinimumSliceCount)
                {
                    slices.Add(new PizzaSlice());
                }
            }

            return slices;
        }
    }
}
