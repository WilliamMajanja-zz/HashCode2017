using GeneticSharp.Domain;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using System;
using System.Linq;

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
            var crossover = new OrderedCrossover();
            var mutation = new ReverseSequenceMutation();

            var fitness = new PizzaSlicesFitness { PizzaModel = model };
            var chromosome = new PizzaSlicesChromosome(model);
            var population = new Population(50, 70, chromosome);

            var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation);
            ga.Termination = new GenerationNumberTermination(100);

            Console.WriteLine("Genetic Algorithm running...");
            ga.Start();

            var best = ga.BestChromosome.GetGenes().ToList().OrderByDescending(x => (x.Value as PizzaSlicesSolution).Score).FirstOrDefault();
            Console.WriteLine("Best solution found has {0} score.", (best.Value as PizzaSlicesSolution).Score);

            Console.ReadLine();
        }

        static void DrawPizza(PizzaModel model)
        {
            for (int i = 0; i < model.RowCount; i++)
            {
                for (int j = 0; j < model.ColumnCount; j++)
                {
                    var value = model.Toppings[i, j] == Topping.Mushroom ? "M" : "T";
                    Console.Write(value + " ");
                }

                Console.WriteLine();
            }
        }
    }
}
