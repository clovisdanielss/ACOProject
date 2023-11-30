using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public partial class AntAgent : Ant
{
	public List<Edge> Graph { get; set; }
	public double Alpha = 1;
	public double Beta = 1;
	private double SelfVertexRadius => 25;
	private List<Vertex> VisitedVertices { get; set; }
	private Vertex _currentVertex;
	public Vertex CurrentVertex
	{
		get
		{
			return _currentVertex;
		}
		set
		{
			_currentVertex = value;
			VisitedVertices.Add(CurrentVertex);
		}
	}
	protected bool PreviousState { get; set; }
	public AntAgent() : base()
	{
		VisitedVertices = new List<Vertex>();
	}

	public override void _Process(double delta)
	{
		if (Graph == null) return;
		PreviousState = HasFood;
		_ProcessTarget();
		base._Process(delta);
		if (_HasSolution())
		{
			if (!HasFood) this.EditText("FCounter", v => ++v);
			QueueRedraw();
			_DropPheromones(delta);
		}
	}

	private void _ProcessTarget()
	{
		if (TargetPosition.HasValue)
		{
			if (Position.IntersectWith(TargetPosition.Value, SelfVertexRadius))
			{
				_DefineNextTargetPosition();
			}
		}
	}

	protected override void _ProcessDirection()
	{
		base._ProcessDirection();
	}

	protected virtual List<Vertex> _GetAdjacents()
	{
		return CurrentVertex.Edges
			.Select(uv => uv.GetOtherVertex(CurrentVertex))
			.Where(v => v != null).ToList();
	}

	/// <summary>
	/// Defines the next target position.
	/// </summary>
	private void _DefineNextTargetPosition()
	{
		//Get adjacents vertices;
		var adjacents = _GetAdjacents();
		//Compute the probability of moving. (If there is no pherormone every vertex has the sabe prob.)
		var probs = adjacents.Select((adja, i) => new { prob = _ProbabilityOfMoving(adja, adjacents), index = i }).OrderBy(p => p.prob);
		//Choose a vertex to set as TargetPosition.
		var randomNumber = Random.Shared.NextDouble();
		var selectedIndex = -1;
		double sumOfProbs = 0;
		foreach (var item in probs)
		{
			sumOfProbs += item.prob;
			if (sumOfProbs >= randomNumber)
			{

				selectedIndex = item.index;
				break;
			}
		}
		if (sumOfProbs == 0 || selectedIndex == -1)
		{
			Reset();
			return;
		}
		CurrentVertex = adjacents[selectedIndex];
		TargetPosition = CurrentVertex.Position;
		//Put the choosed vertex in the VisitedVertices list.
		VisitedVertices.Add(adjacents[selectedIndex]);
	}

	private void Reset()
	{
		VisitedVertices = new List<Vertex>();
		if (HasFood)
		{
			Position = FoodPosition.Value;
			TargetPosition = FoodPosition.Value;
			CurrentVertex = Graph.First(e => e.GetVertexAt(FoodPosition.Value) != null).GetVertexAt(FoodPosition.Value);
		}
		else
		{
			Position = ColonyPosition;
			TargetPosition = ColonyPosition;
			CurrentVertex = Graph.First(e => e.GetVertexAt(ColonyPosition) != null).GetVertexAt(ColonyPosition);
		}		
	}


	public override void _Draw()
	{
		DrawCircle(ToLocal(Position), (float)SelfVertexRadius, new Color
		{
			R = 0,
			G = HasFood ? 1 : 0,
			B = HasFood ? 0 : 1,
			A = .5f,
		});
		base._Draw();
	}
	private double _ProbabilityOfMoving(Vertex u, List<Vertex> adjacents)
	{
		var nonVisitedAdjacents = adjacents.Where(u => !VisitedVertices.Any(u.Equals));
		if (!nonVisitedAdjacents.Any()) return 0;
		if (VisitedVertices.Any(v => v == u)) return 0;

		var edge = CurrentVertex.GetEdge(u);
		var numerator = Math.Pow(edge.Pheromone, Alpha) * Math.Pow(edge.Quality, Beta);

		var denominator = nonVisitedAdjacents.Sum(u =>
		{
			var edge = CurrentVertex.GetEdge(u);
			return Math.Pow(edge.Pheromone, Alpha) * Math.Pow(edge.Quality, Beta);
		});

		if (denominator == 0) return 0;
		return numerator / denominator;
	}

	/// <summary>
	/// Verifies if the VisitedVertices consist in a solution for the current problem. 
	/// </summary>
	/// <returns>True if the solution is valid</returns>
	protected virtual bool _HasSolution()
	{
		var objectiveChanged = PreviousState != HasFood;
		return objectiveChanged && VisitedVertices.Count > 1;
	}
	private void _DropPheromones(double delta)
	{
		//Copy the solution. 
		var solution = VisitedVertices;
		//Compute solution cost
		var path = solution.Zip(solution.Skip(1), (first, second) => new { u = first, v = second });
		var cost = .0;
		foreach (var edge in path)
		{
			cost += edge.u.Position.DistanceTo(edge.v.Position);
		}
		if (cost == 0)
		{
			GD.Print(cost, " Caminho: ", string.Join("\n", VisitedVertices), " Comida: ", HasFood ? "Tem comida" : "Não tem", " TPosition: ", TargetPosition, " Position: ", Position, " CPosition:", ColonyPosition);
		}
		foreach (var edge in Graph)
		{
			if (path.Any(e => edge.IsThisEdge(e.u, e.v)))
			{
				edge.Update(1.0 / cost);
			}else{
				edge.Update(0);
			}
		}
		//Reset visited vertices.
		Reset();
	}


}
