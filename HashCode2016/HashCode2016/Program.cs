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
        static void Main(string[] args)
        {
            var parser = new PizzaParser();

            var model = parser.Parse("../../../../example.in");
            DrawPizza(model);

            var selection = new EliteSelection();
            var crossover = new OrderedCrossOver();
            var mutation = new ReverseSequenceMutation();

            var fitness = new PizzaSlicesFitness { PizzaModel = model };
            var chromosome = new PizzaSlicesChromosome(model, PizzaSlicer.Slice(model));
            PizzaSlicesChromosome.MaxGenes = model.RowCount * model.ColumnCount;

            var population = new Population(50, 70, chromosome);

            var algorithm = new GeneticAlgorithm(population, fitness, selection, crossover, mutation);
            algorithm.Termination = new GenerationNumberTermination(10000);

            algorithm.GenerationRan += AlgorithmGenerationRan;
            algorithm.TerminationReached += AlgorithmTerminationReached;

            algorithm.CrossoverProbability = 0.8f;
            algorithm.MutationProbability = 0.8f;

            Console.WriteLine("Genetic Algorithm running...");
            Console.WriteLine("Current generation: ");
            algorithm.Start();

            Console.WriteLine("Best solution found has {0} score.", algorithm.BestChromosome.Fitness);

            Console.ReadLine();
        }

        private static void AlgorithmTerminationReached(object sender, EventArgs e)
        {
            Console.WriteLine();
        }

        private static void AlgorithmGenerationRan(object sender, EventArgs e)
        {
            var algorithm = sender as GeneticAlgorithm;
            Console.WriteLine("[#{0}, F:{1}]", algorithm.GenerationsNumber, algorithm.BestChromosome.Fitness);
        }

        static void DrawPizza(PizzaModel model)
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
