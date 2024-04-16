using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace GraphPeeling
{
	public class Vertex
	{
		public int Sum = 0;
		public int NumberOfEdges = 0;

		public void AddEdge(Edge edge)
		{
			Sum += edge.Id;
			NumberOfEdges++;
		}
		public void RemoveEdge(Edge edge)
		{
			Sum -= edge.Id;
			NumberOfEdges--;
		}

		public int? TryGetPureId()
		{
			if (NumberOfEdges == 1) return Sum;
			return null;
		}
	}
	public class Edge
	{
		public readonly int Id;
		public bool IsRemoved = true;
		public Vertex[] Vertices;

		public Edge(Vertex[] vertices, int id)
		{
			Id = id;
			Vertices = vertices;
		}
		public void Remove()
		{
			if (IsRemoved == true) return;
			for (int i = 0; i < Vertices.Length; i++)
			{
				Vertices[i].RemoveEdge(this);
			}
			IsRemoved = true;
		}
		public void Add()
		{
			if (IsRemoved == false) return;
			for (int i = 0; i < Vertices.Length; i++)
			{
				Vertices[i].AddEdge(this);
			}
			IsRemoved = false;
		}
	}



	public class PeelableHyperGraph
	{
		readonly Vertex[] _vertices;
		readonly List<Edge> _edges;
		readonly List<Edge> _pures = new List<Edge>();
		public int NumberOfEdges { get { return _edges.Count(); } }
		public int NumberOfVertices { get { return _vertices.Length; } }
		int removed = 0;

		public PeelableHyperGraph(int size, int numberOfEdges, Func<Vertex[], int, Edge[]> CreateEdges)
		{
			_vertices = new Vertex[size];
			for (int i = 0; i < _vertices.Length; i++) _vertices[i] = new Vertex();
			_edges = new List<Edge>();
			int nEdges = numberOfEdges;
			_edges = new List<Edge>(CreateEdges(_vertices, nEdges));
			foreach (var edge in _edges)
			{
				edge.Add();
			}
		}
		public PeelableHyperGraph(int size, double fullness, Func<Vertex[], int, Edge[]> CreateEdges)
		{
			_vertices = new Vertex[size];
			for (int i = 0; i < _vertices.Length; i++) _vertices[i] = new Vertex();
			_edges = new List<Edge>();
			int nEdges = (int)(size * fullness);
			_edges = new List<Edge>(CreateEdges(_vertices, nEdges));
			foreach (var edge in _edges)
			{
				edge.Add();
			}
		}

		public void AddEdgesVerticesIfPure(Edge edge, List<Edge> pureEdges)
		{
			for (int i = 0; i < edge.Vertices.Length; i++)
			{
				var id = edge.Vertices[i].TryGetPureId();
				if (id == null) continue;
				pureEdges.Add(_edges[(int)id]);
			}
		}
		public void RemoveEdge(Edge edge)
		{
			edge.Remove();
			AddEdgesVerticesIfPure(edge, _pures);
			removed++;
		}

		public void RemoveEdge(int id)
		{
			Edge edge = _edges[id];
			RemoveEdge(edge);
		}

		public void FindPure()
		{
			foreach (Vertex vertex in _vertices)
			{
				var answer = vertex.TryGetPureId();
				if (answer == null) continue;
				_pures.Add(_edges[(int)answer]);
			}
		}

		public bool Peel()
		{
			while (_pures.Count > 0)
			{
				Edge lastPure = _pures[_pures.Count - 1];
				_pures.RemoveAt(_pures.Count - 1);
				RemoveEdge(lastPure);
			}
			if (removed == _edges.Count)
			{
				return true;
			}
			return false;
		}
	}
}
