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

        public double Evaluate(IChromosome chromosome)
        {
            var fitniss = 0;
            foreach (var gene in chromosome.GetGenes())
            {
                var solution = gene.Value as PizzaSlicesSolution;
                foreach (var slice in solution.Slices)
                {
                    var toppings = Cut(slice);
                    if (toppings.Count > PizzaModel.MaxCellsPerSlice)
                        continue;

                    var mushroomCount = toppings.Count(x => x == Topping.Mushroom);
                    var tomateCount = toppings.Count(x => x == Topping.Tomato);

                    var isValid = toppings.Count > 0 && 
                        mushroomCount >= PizzaModel.MinimumIngredientCount && 
                        tomateCount >= PizzaModel.MinimumIngredientCount;

                    if (isValid)
                    {
                        solution.Score = toppings.Count;
                    }
                    
                    fitniss += solution.Score;
                }
            }

            return fitniss;
        }

        private IList<Topping> Cut(PizzaSlice pizzaSlice)
        {
            var toppings = new List<Topping>();

            for (int rowIndex = pizzaSlice.RowStart; rowIndex <= pizzaSlice.RowEnd; rowIndex++)
            {
                for (int columnIndex = pizzaSlice.ColumnStart; columnIndex <= pizzaSlice.ColumnEnd; columnIndex++)
                {
                    toppings.Add(PizzaModel.Toppings[rowIndex, columnIndex]);
                }
            }

            return toppings;
        }
    }
}
