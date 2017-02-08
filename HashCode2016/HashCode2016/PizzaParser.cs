using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace HashCode2016
{
    public class PizzaParser
    {
        public PizzaModel Parse(string path)
        {
            var lines = File.ReadAllLines(path);
            var parameters = lines[0].Split(' ');
            var model = new PizzaModel
            {
                RowCount = Convert.ToInt32(parameters[0]),
                ColumnCount = Convert.ToInt32(parameters[1]),
                MinimumIngredientCount = Convert.ToInt32(parameters[2]),
                MaxIngredientsPerSlice = Convert.ToInt32(parameters[3])
            };

            var toppings = new Ingredient[model.RowCount, model.ColumnCount];

            for (int rowIndex = 1; rowIndex < lines.Length; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < lines[rowIndex].Length; columnIndex++)
                {
                    var value = lines[rowIndex][columnIndex].ToString();
                    if (value.ToUpper().Equals("M"))
                    {
                        toppings[rowIndex - 1, columnIndex] = Ingredient.Mushroom;
                    }
                    else if (value.ToUpper().Equals("T"))
                    {
                        toppings[rowIndex - 1, columnIndex] = Ingredient.Tomato;
                    }
                    else
                    {
                        throw new InvalidOperationException("No topping known that is " + value);
                    }
                }
            }

            model.Ingredients = toppings;

            return model;
        }
    }
}
