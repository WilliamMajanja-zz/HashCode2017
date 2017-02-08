using GeneticSharp.Domain.Fitnesses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneticSharp.Domain.Chromosomes;

namespace HashCode2016
{
    public class PizzaSlicesFitness : IFitness
    {
        public PizzaModel PizzaModel { get; set; }

        private Ingredient[,] _ingredients;

        public double Evaluate(IChromosome chromosome)
        {
            var fitniss = 0;
            _ingredients = PizzaModel.Ingredients.Clone() as Ingredient[,];

            foreach (var gene in chromosome.GetGenes())
            {
                var slice = gene.Value as PizzaSlice;

                var ingredients = Cut(slice);
                if (ingredients.Count > PizzaModel.MaxIngredientsPerSlice)
                    continue;

                var mushroomCount = ingredients.Count(x => x == Ingredient.Mushroom);
                var tomateCount = ingredients.Count(x => x == Ingredient.Tomato);
                var noneCount = ingredients.Count(x => x == Ingredient.None);

                slice.IsValid = ingredients.Count > 0 &&
                    mushroomCount >= PizzaModel.MinimumIngredientCount &&
                    tomateCount >= PizzaModel.MinimumIngredientCount &&
                    noneCount == 0;

                if (!slice.IsValid)
                    continue;

                fitniss += ingredients.Count;
            }

            return fitniss;
        }

        private IList<Ingredient> Cut(PizzaSlice pizzaSlice)
        {
            var ingredients = new List<Ingredient>();

            if (pizzaSlice.RowStart >= PizzaModel.RowCount || pizzaSlice.RowEnd >= PizzaModel.RowCount ||
                pizzaSlice.ColumnStart >= PizzaModel.ColumnCount || pizzaSlice.ColumnEnd >= PizzaModel.ColumnCount ||
                pizzaSlice.RowStart == -1 || pizzaSlice.RowEnd == -1 || pizzaSlice.ColumnStart == -1 || pizzaSlice.ColumnEnd == -1)
            {
                return ingredients;
            }

            for (int rowIndex = pizzaSlice.RowStart; rowIndex <= pizzaSlice.RowEnd; rowIndex++)
            {
                for (int columnIndex = pizzaSlice.ColumnStart; columnIndex <= pizzaSlice.ColumnEnd; columnIndex++)
                {
                    ingredients.Add(_ingredients[rowIndex, columnIndex]);
                    _ingredients[rowIndex, columnIndex] = Ingredient.None;
                }
            }

            return ingredients;
        }
    }
}
