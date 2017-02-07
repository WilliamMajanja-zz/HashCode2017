using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashCode2016
{
    public class PizzaSlicesChromosome : ChromosomeBase
    {
        public PizzaModel PizzaModel { get; set; }

        public PizzaSlicesChromosome(PizzaModel pizzaModel) : base(10)
        {
            PizzaModel = pizzaModel;

            for (int i = 0; i < Length; i++)
            {
                ReplaceGene(i, GenerateGene(i));
            }
        }

        public override IChromosome CreateNew()
        {
            return new PizzaSlicesChromosome(PizzaModel);
        }

        public override Gene GenerateGene(int geneIndex)
        {
            // TODO: don't generate one, generate all slices until the pizza is fully sliced
            var rowStart = RandomizationProvider.Current.GetInt(0, PizzaModel.RowCount);
            var rowEnd = RandomizationProvider.Current.GetInt(rowStart, PizzaModel.RowCount);

            var columnStart = RandomizationProvider.Current.GetInt(0, PizzaModel.ColumnCount);
            var columnEnd = RandomizationProvider.Current.GetInt(columnStart, PizzaModel.ColumnCount);

            var solution = new PizzaSlicesSolution { Slices = new List<PizzaSlice>() };
            var slice = new PizzaSlice { RowStart = rowStart, RowEnd = rowEnd, ColumnStart = columnStart, ColumnEnd = columnEnd };
            solution.Slices.Add(slice);

            return new Gene(solution);
        }
    }
}
