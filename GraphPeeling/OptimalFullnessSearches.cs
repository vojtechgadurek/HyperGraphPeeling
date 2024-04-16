using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphPeeling
{
	public class BinarySearchForOptimalFulness
	{
		double _upperBoundOnFullness = 1;
		double _lowerBoundOnFullness = 0;
		int _numberOfSteps = 16;
		Func<double, PeelableHyperGraph> PeableHyperGraphConstructor;
		public BinarySearchForOptimalFulness(double upperBoundOnFullness, double lowerBoundOnFullness, int numberOfSteps, Func<double, PeelableHyperGraph> peableHyperGraphConstructor)
		{
			this._upperBoundOnFullness = upperBoundOnFullness;
			this._lowerBoundOnFullness = lowerBoundOnFullness;
			this._numberOfSteps = numberOfSteps;
			PeableHyperGraphConstructor = peableHyperGraphConstructor;
		}

		public double Run()

		{
			for (int step = 0; step < _numberOfSteps; step++)
			{
				double middle = (_upperBoundOnFullness + _lowerBoundOnFullness) / 2;
				PeelableHyperGraph test = PeableHyperGraphConstructor(middle);
				test.FindPure();
				var answer = test.Peel();
				if (answer == true)
				{
					_lowerBoundOnFullness = middle;
				}
				else
				{
					_upperBoundOnFullness = middle;
				}
			}
			return _lowerBoundOnFullness;
		}
	}

	public class RemovalSearchForOptimalFulness
	{
		PeelableHyperGraph _test;
		public RemovalSearchForOptimalFulness(PeelableHyperGraph peableHyperGraph)
		{
			_test = peableHyperGraph;
		}

		public double Run()
		{
			int counter = 0;
			_test.FindPure();
			while (true)
			{
				if (_test.Peel()) break;
				_test.RemoveEdge(counter);
				counter++;
				if (counter >= _test.NumberOfEdges) break;
			}
			double answer;
			answer = (_test.NumberOfEdges - counter) / _test.NumberOfVertices;
			return answer;
		}
	}

}
