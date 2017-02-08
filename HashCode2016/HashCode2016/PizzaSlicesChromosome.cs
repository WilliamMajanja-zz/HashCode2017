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
        public static int MaxGenes = 3;

        public PizzaModel PizzaModel { get; set; }
        public IList<PizzaSlice> Slices { get; set; }

        public PizzaSlicesChromosome(PizzaModel pizzaModel, IList<PizzaSlice> slices) : base(slices.Count)
        {
            PizzaModel = pizzaModel;
            Slices = slices;
            CreateGenes();
        }

        public override IChromosome CreateNew()
        {
            var slices = PizzaSlicer.Slice(PizzaModel);
            return new PizzaSlicesChromosome(PizzaModel, slices);
        }

        public override Gene GenerateGene(int geneIndex)
        {
            return new Gene(Slices[geneIndex]);
        }
    }
}
