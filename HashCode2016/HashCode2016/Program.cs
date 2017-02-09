using GeneticSharp.Domain;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using System;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using System.Collections.Generic;
using GeneticSharp.Domain.Randomizations;

namespace HashCode2016
{
    // https://github.com/axelgoblet/hashcode2017/blob/master/practice%20problem/pizza.pdf
    class Program
    {
        private static double HighestFitnessScore = 0;
        private static int MaxScore = int.MaxValue;

        static void Main(string[] args)
        {
            var parser = new PizzaParser();

            var model = parser.Parse("../../../../small.in");
            DrawPizza(model);

            MaxScore = model.RowCount * model.ColumnCount;
            Console.WriteLine("Maximum possible score: " + MaxScore);

            var selection = new EliteSelection();
            var crossover = new OrderedCrossOver();
            var mutation = new ReverseSequenceMutation();

            var fitness = new PizzaSlicesFitnessEvaluator { PizzaModel = model };
            var chromosome = new PizzaSlicesChromosome(model, PizzaSlicer.Slice(model));
            PizzaSlicesChromosome.MaxGenes = model.RowCount * model.ColumnCount;

            var population = new Population(50, 70, chromosome);

            var algorithm = new GeneticAlgorithm(population, fitness, selection, crossover, mutation)
            {
                Termination = new GenerationNumberTermination(1000)
            };

            algorithm.GenerationRan += AlgorithmGenerationRan;
            algorithm.TerminationReached += AlgorithmTerminationReached;
            /*
            algorithm.CrossoverProbability = 0.75f;
            algorithm.MutationProbability = 0.5f;
            */

            Console.WriteLine("Genetic Algorithm running...");
            Console.WriteLine("Current generation: ");
            algorithm.Start();

            Console.WriteLine("Best solution found has {0} score.", algorithm.BestChromosome.Fitness);

            var evaluator = new PizzaSlicesFitnessEvaluator { PizzaModel = model };
            evaluator.Evaluate(algorithm.BestChromosome);
            DrawSolution(model, evaluator.Slices);

            Console.WriteLine("Press any key to exit");

            Console.ReadLine();
        }

        private static void AlgorithmTerminationReached(object sender, EventArgs e)
        {
            Console.WriteLine();
        }

        private static void AlgorithmGenerationRan(object sender, EventArgs e)
        {
            var algorithm = sender as GeneticAlgorithm;
            if (algorithm.BestChromosome.Fitness > HighestFitnessScore)
            {
                Console.WriteLine("Generation #{0} produced a higher fitness: {1}", algorithm.GenerationsNumber,
                    algorithm.BestChromosome.Fitness);
                HighestFitnessScore = algorithm.BestChromosome.Fitness.GetValueOrDefault(HighestFitnessScore);

                if (HighestFitnessScore == MaxScore)
                {
                    algorithm.Stop();
                }
            }
        }

        private static void DrawPizza(PizzaModel model)
        {
            for (int i = 0; i < model.RowCount; i++)
            {
                for (int j = 0; j < model.ColumnCount; j++)
                {
                    var value = model.Ingredients[i, j] == Ingredient.Mushroom ? "M" : "T";
                    Console.Write(value + " ");
                }

                Console.WriteLine();
            }
        }

        private static void DrawSolution(PizzaModel model, IList<PizzaSlice> slices)
        {
            var colors = Enum.GetValues(typeof(ConsoleColor));
            var colorIndex = 1;

            var coloredPizza = new Tuple<Ingredient, ConsoleColor>[model.RowCount, model.ColumnCount];

            foreach (var pizzaSlice in slices) 
            {
                for (int rowIndex = pizzaSlice.RowStart; rowIndex <= pizzaSlice.RowEnd; rowIndex++)
                {
                    for (int columnIndex = pizzaSlice.ColumnStart; columnIndex <= pizzaSlice.ColumnEnd; columnIndex++)
                    {
                        var ingredient = model.Ingredients[rowIndex, columnIndex];
                        coloredPizza[rowIndex, columnIndex] = new Tuple<Ingredient, ConsoleColor>(ingredient, (ConsoleColor) colors.GetValue(colorIndex));
                    }
                }

                colorIndex++;

                if (colorIndex == colors.Length - 1)
                {
                    colorIndex = 1;
                }
            }

            for (int i = 0; i < model.RowCount; i++)
            {
                for (int j = 0; j < model.ColumnCount; j++)
                {
                    if (coloredPizza[i, j] == null)
                    {
                        var value = model.Ingredients[i, j] == Ingredient.Mushroom ? "M" : "T";
                        Console.ResetColor();
                        Console.Write(value + " ");
                    }
                    else
                    {
                        var value = coloredPizza[i, j].Item1 == Ingredient.Mushroom ? "M" : "T";
                        Console.BackgroundColor = coloredPizza[i, j].Item2;
                        Console.Write(value + " ");
                    }
                }

                Console.WriteLine();
            }

            Console.ResetColor();
        }

        public class OrderedCrossOver : CrossoverBase
        {
            public OrderedCrossOver()
            : base(2, 2)
            {
                IsOrdered = true;
            }

            protected override IList<IChromosome> PerformCross(IList<IChromosome> parents)
            {
                var firstParent = parents[0];
                var secondParent = parents[1];

                var rnd = RandomizationProvider.Current;
                var swapIndexesLength = rnd.GetInt(1, firstParent.Length - 1);
                var swapIndexes = rnd.GetUniqueInts(swapIndexesLength, 0, firstParent.Length);
                var firstChild = CreateChild(firstParent, secondParent, swapIndexes);
                var secondChild = CreateChild(secondParent, firstParent, swapIndexes);

                return new List<IChromosome>() { firstChild, secondChild };
            }

            protected virtual IChromosome CreateChild(IChromosome firstParent, IChromosome secondParent, int[] swapIndexes)
            {
                // ...suppose that in the second parent in the second, third 
                // and sixth positions are selected. The elements in these positions are 4, 6 and 5 respectively...
                var secondParentSwapGenes = secondParent.GetGenes()
                    .Select((g, i) => new { Gene = g, Index = i })
                    .Where((g) => swapIndexes.Contains(g.Index))
                    .ToArray();

                var firstParentGenes = firstParent.GetGenes();

                // ...in the first parent, these elements are present at the fourth, fifth and sixth positions...
                var firstParentSwapGenes = firstParentGenes
                    .Select((g, i) => new { Gene = g, Index = i })
                    .Where((g) => secondParentSwapGenes.Any(s => s.Gene == g.Gene))
                    .ToArray();

                var child = firstParent.CreateNew();
                var secondParentSwapGensIndex = 0;

                for (int i = 0; i < firstParent.Length && i < child.Length; i++)
                {
                    // Now the offspring are equal to parent 1 except in the fourth, fifth and sixth positions.
                    // We add the missing elements to the offspring in the same order in which they appear in the second parent.                
                    if (firstParentSwapGenes.Any(f => f.Index == i))
                    {
                        child.ReplaceGene(i, secondParentSwapGenes[secondParentSwapGensIndex++].Gene);
                    }
                    else
                    {
                        child.ReplaceGene(i, firstParentGenes[i]);
                    }
                }

                return child;
            }
        }
    }
}
