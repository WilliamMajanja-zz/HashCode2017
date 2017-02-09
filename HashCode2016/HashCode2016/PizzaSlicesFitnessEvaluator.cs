using GeneticSharp.Domain.Fitnesses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneticSharp.Domain.Chromosomes;

namespace HashCode2016
{
    public class PizzaSlicesFitnessEvaluator : IFitness
    {
        public PizzaModel PizzaModel { get; set; }
        public List<PizzaSlice> Slices { get; set; }

        private Ingredient[,] _ingredients;

        public double Evaluate(IChromosome chromosome)
        {
            var fitniss = 0;
            _ingredients = PizzaModel.Ingredients.Clone() as Ingredient[,];
            Slices = new List<PizzaSlice>();

            foreach (var gene in chromosome.GetGenes())
            {
                var slice = gene.Value as PizzaSlice;

                var ingredients = Cut(slice);
                if (ingredients.Count > PizzaModel.MaxIngredientsPerSlice)
                    continue;

                var mushroomCount = ingredients.Count(x => x == Ingredient.Mushroom);
                var tomateCount = ingredients.Count(x => x == Ingredient.Tomato);
                var noneCount = ingredients.Count(x => x == Ingredient.None);

                bool isValid = ingredients.Count > 0 &&
                    mushroomCount >= PizzaModel.MinimumIngredientCount &&
                    tomateCount >= PizzaModel.MinimumIngredientCount &&
                    noneCount == 0;

                if (!isValid)
                    continue;

                Slices.Add(slice);
                fitniss += ingredients.Count;
            }

            return fitniss;
        }

        private IList<Ingredient> Cut(PizzaSlice pizzaSlice)
        {
            var ingredients = new List<Ingredient>();

            if (!pizzaSlice.CanBeCutOut(PizzaModel))
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
