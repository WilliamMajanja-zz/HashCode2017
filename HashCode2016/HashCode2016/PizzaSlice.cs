namespace HashCode2016
{
    public class PizzaSlice
    {
        public int RowStart { get; set; }
        public int RowEnd { get; set; }
        public int ColumnStart { get; set; }
        public int ColumnEnd { get; set; }

        public PizzaSlice()
        {
            RowStart = -1;
            RowEnd = -1;
            ColumnStart = -1;
            ColumnEnd = -1;
        }

        public bool CanBeCutOut(PizzaModel pizzaModel)
        {
            if (RowStart >= pizzaModel.RowCount || RowEnd >= pizzaModel.RowCount ||
                ColumnStart >= pizzaModel.ColumnCount || ColumnEnd >= pizzaModel.ColumnCount ||
                RowStart == -1 || RowEnd == -1 || ColumnStart == -1 || ColumnEnd == -1)
            {
                return false;
            }

            return true;
        }
    }
}
