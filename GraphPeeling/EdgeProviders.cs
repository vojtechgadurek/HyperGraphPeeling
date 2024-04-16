using GeneticSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace GraphPeeling
{
	public static class EdgeProviders
	{
		static Random random = new Random();
		public static Func<Vertex[], int, Edge[]> UniversalDistributionForEdgeSize(double[] sizeDistribution)
		{
			double[] values = sizeDistribution;
			double sum = values.Sum();
			return
				(vertices, numberOfEdges) =>
				{
					Edge[] answer = new Edge[numberOfEdges];
					for (int edgeId = 0; edgeId < numberOfEdges; edgeId++)
					{
						//Find size of edge
						double x = random.NextDouble() * sum;
						double sum2 = 0;
						int numberOfVertices = 0;
						for (int i = 0; i < values.Length; i++)
						{
							sum2 += values[i];
							if (x < sum2) break;
						}

						numberOfVertices++;

						//Build such edge
						var verticesEdge = new Vertex[numberOfVertices];
						for (int i = 0; i < numberOfVertices; i++)
						{
							verticesEdge[i] = vertices[random.Next(vertices.Length)];
						}

						answer[edgeId] = new Edge(verticesEdge, edgeId);
					}
					return answer;
				};
		}
	}

}
