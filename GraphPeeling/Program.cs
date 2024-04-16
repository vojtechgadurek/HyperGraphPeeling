using GeneticSharp;
using GraphPeeling;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Dynamic;
using System.Net.Http.Headers;

var ga = new BasicGA(Functions.TrueFitness);
ga.Start();

public static class Functions
{



	public static double Fitness(Chromosome c)
	{
		var f = EdgeProviders.UniversalDistributionForEdgeSize(c.Genes);
		return Enumerable.Range(0, 5).Select(_ =>
		{
			var test = new PeelableHyperGraph(10000, 1.0, f);
			return new RemovalSearchForOptimalFulness(test).Run();
		}
		)
		.Min();
	}

	public static double TrueFitness(Chromosome c)
	{
		var f = EdgeProviders.UniversalDistributionForEdgeSize(c.Genes);
		return Enumerable.Range(0, 1).Select(_ =>
		{
			return new BinarySearchForOptimalFulness(1, 0.2, 5, (fullness) => new PeelableHyperGraph(10000, fullness, f)).Run();
		}
		)
		.Min();
	}
}



