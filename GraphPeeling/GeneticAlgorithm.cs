using GeneticSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace GraphPeeling
{
	public class GeneticAlgorithm
	{

	}

	public class Population
	{
		public List<Chromosome> chromosomes;

		public Population(List<Chromosome> chromosomes)
		{
			this.chromosomes = chromosomes;
		}
	}

	public class Chromosome
	{
		public double[] Genes;
		public Chromosome(double[] genes)
		{
			this.Genes = genes;
		}
		public Chromosome Copy()
		{
			var newGenes = new double[Genes.Length];
			Array.Copy(Genes, newGenes, Genes.Length);
			return new Chromosome(newGenes);
		}
	}

	public class BasicGA
	{
		double _mutationRate = 0.5;
		double _mutationAlpha = 1;
		int _numberOfIterations = 1000;
		int _populationSize = 100;
		Random _random = new Random();
		Func<Chromosome, double> _fitness;

		public BasicGA(Func<Chromosome, double> fitness)
		{
			_fitness = fitness;
		}

		public Population RouletteWhealSelection(List<(Chromosome, double)> chromosomesWithFitness)
		{
			var totalFitness = chromosomesWithFitness.Sum(x => x.Item2);

			var valuesSelected = Enumerable.Range(0, chromosomesWithFitness.Count()).Select(_ => RandomizationProvider.Current.GetDouble()).Order();

			var enumerator = valuesSelected.GetEnumerator();

			var newChromosomes = new List<Chromosome>(chromosomesWithFitness.Count());


			foreach (var fitnessWithChromosome in chromosomesWithFitness)
			{
				totalFitness -= fitnessWithChromosome.Item2;
				while (true)
				{
					bool notFinished = enumerator.MoveNext();
					if (notFinished == false)
					{
						break;
					}
					if (enumerator.Current < totalFitness)
					{
						break;
					}
					newChromosomes.Add(fitnessWithChromosome.Item1);
				}
			}
			return new Population(newChromosomes);
		}

		public Chromosome DoubleMutation(Chromosome chromosome)
		{
			Chromosome c = chromosome.Copy();
			var genes = c.Genes;
			for (int i = 0; i < genes.Length; i++)
			{
				var change = 1 + ((_random.NextDouble() - 0.5) * 2) * _mutationAlpha;
				genes[i] *= change;
			}
			var sum = genes.Sum();
			for (int i = 0; i < genes.Length; i++)
			{
				genes[i] = genes[i] / sum;
			}
			return c;
		}

		public Population BasicMutation(Population p)
		{
			p.chromosomes = p.chromosomes.Select(DoubleMutation).Concat(p.chromosomes).ToList();
			return p;
		}

		public Population GetRandomPopulation(int size, int chromosomeSize)
		{
			List<Chromosome> p = new List<Chromosome>(size);
			for (int i = 0; i < size; i++)
			{
				p.Add(GetRandomChromosome(chromosomeSize));
			}
			return new Population(p);
		}

		public Population Averaging(Population x, int numberOfNewItems)
		{
			return new Population(Enumerable.Range(0, 100).Select(_ =>
			{
				var first = x.chromosomes[_random.Next(x.chromosomes.Count)];
				var second = x.chromosomes[_random.Next(x.chromosomes.Count)];
				var newGenes = new double[first.Genes.Length];
				for (int i = 0; i < newGenes.Length; i++)
				{
					newGenes[i] = first.Genes[i] + second.Genes[i];
				}
				return new Chromosome(newGenes);
			}
			).Concat(x.chromosomes).ToList());
		}

		public Chromosome GetReallyRandomChromosome(int size)
		{
			var c = new double[size];
			for (int i = 0; i < size; i++)
			{
				c[i] = _random.NextDouble();
			}
			return new Chromosome(c);
		}

		public Chromosome GetRandomChromosome(int size)
		{
			var c = new double[size];
			for (int i = 0; i < size; i++)
			{
				c[i] = 1;
			}
			return new Chromosome(c);
		}

		public void Start()
		{
			void PrintGene(double[] gene)
			{
				var sum = gene.Sum();
				var genes = gene.Select(x => x * 1000 / sum);
				foreach (var g in genes)
				{
					Console.Write((int)g);
					Console.Write(" ");
				}
				Console.WriteLine();

			}

			int chromosomeLenght = 10;

			Population population = GetRandomPopulation(_populationSize, chromosomeLenght);
			for (int i = 0; i < _numberOfIterations; i++)
			{
				foreach (var _ in Enumerable.Range(0, 20)) population.chromosomes.Add(GetReallyRandomChromosome(chromosomeLenght));
				population = BasicMutation(population);
				population = Averaging(population, 100);

				var chromosomesWithFitness = new (Chromosome, double)[population.chromosomes.Count];



				Parallel.For(0, population.chromosomes.Count, (i) =>
				{
					var c = population.chromosomes[i];
					var x = (c, Functions.Fitness(c));
					chromosomesWithFitness[i] = x;
				});

				population.chromosomes.ForEach(c =>
				{
					var sum = c.Genes.Sum();
					for (int i = 0; i < c.Genes.Length; i++)
					{
						c.Genes[i] /= sum * _random.Next(1, 10);
					}
				});

				var minFitness = chromosomesWithFitness.Min(x => x.Item2);


				var best = chromosomesWithFitness.MaxBy(x => x.Item2);
				Console.Write(_mutationAlpha);
				Console.Write("");
				Console.Write(minFitness);
				Console.Write(" ");
				Console.WriteLine(best.Item2);
				Console.WriteLine(Functions.TrueFitness(best.Item1));
				PrintGene(best.Item1.Genes);
				population = new Population(chromosomesWithFitness.OrderByDescending(x => x.Item2).Take(_populationSize).Select(x => x.Item1).ToList());

				_mutationAlpha *= 0.998;
			}
		}


	}
}
