using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public static class OrientationOffset
{
	public static Vector2 North(double dist)
	{
		return new Vector2(0, (float)-dist);
	}
	public static Vector2 NorthEast(double dist)
	{
		return new Vector2((float)dist, (float)-dist);
	}
	public static Vector2 East(double dist)
	{
		return new Vector2((float)dist, 0);
	}
	public static Vector2 SouthEast(double dist)
	{
		return new Vector2((float)dist, (float)dist);
	}
	public static Vector2 South(double dist)
	{
		return new Vector2(0, (float)dist);
	}
	public static Vector2 SouthWest(double dist)
	{
		return new Vector2((float)-dist, (float)dist);
	}
	public static Vector2 West(double dist)
	{
		return new Vector2((float)-dist, 0);
	}
	public static Vector2 NorthWest(double dist)
	{
		return new Vector2((float)-dist, (float)-dist);
	}
}

public class Vertex
{
	public Vector2 Position { get; set; }
	public double Radius { get; set; }
	public double Pheromone { get; set; }
	public double Quality { get; set; }

	public Vertex(Vector2 pos, double r)
	{
		Position = pos;
		Radius = r;
	}

	public bool IntersectWith(Vertex v)
	{
		return Math.Pow(v.Position.X - Position.X, 2) + Math.Pow(v.Position.Y - Position.Y, 2) < Math.Pow(Radius, 2);
	}

	public bool IntersectWith(Vector2 v)
	{
		return Math.Pow(v.X - Position.X, 2) + Math.Pow(v.Y - Position.Y, 2) < Math.Pow(Radius, 2);
	}

	public bool IsOffscreen(float width, float height)
	{
		return Position.X < 0 || Position.X > width || Position.Y > height || Position.Y < 0;
	}

	public override string ToString()
	{
		return $"{Position} Q:{Quality} Ph:{Pheromone}";
	}
}

public partial class AntAgent : Ant
{
	private double PheromoneImportance = 2;
	private double QualityImportance = 2;
	private const double AdjacentDistance = 100;
	private double AdjacentRatio => 3 * AdjacentDistance / 6;
	private double SelfVertexRatio => AdjacentDistance / 6;
	private List<Vertex> VisitedVertices { get; set; }
	protected bool PreviousState { get; set; }
	public AntAgent() : base()
	{
		VisitedVertices = new List<Vertex>();
	}

	private Vertex _GetSelfVertex()
	{
		return new Vertex(Position, SelfVertexRatio);
	}

	public override void _Process(double delta)
	{
		PreviousState = ShouldReturn;
		_ProcessTarget();
		base._Process(delta);
		if (HasSolution())
		{
			_DropPheromones(delta);
		}
	}

	private void _ProcessTarget()
	{
		if (TargetPosition.HasValue)
		{
			var v = _GetSelfVertex();
			if (v.IntersectWith(TargetPosition.Value))
			{
				_DefineNextTargetPosition();
			}
		}
	}

	protected override void _ProcessDirection()
	{
		base._ProcessDirection();
	}

	/// <summary>
	/// Create the adjacents vertices for the current position of the ant:
	/// N, NE, E, SE, S, SW, W, NW.
	/// </summary>
	/// <returns>A list of adjacents</returns>
	private List<Vertex> _CreateAdjacents()
	{
		//Get all pheromone from map. 
		var parent = this.GetParent();
		var pheromones = parent.GetChildren().Where(node => node.Name.ToString().Contains("Pheromone"));
		//Create vertex based on current position, N, NE, E, SE, S, SW, W, NW
		List<Vertex> adjacents = new List<Vertex>
		{
			new(Position + OrientationOffset.North(AdjacentDistance), AdjacentRatio),
			new(Position + OrientationOffset.NorthEast(AdjacentDistance), AdjacentRatio),
			new(Position + OrientationOffset.East(AdjacentDistance), AdjacentRatio),
			new(Position + OrientationOffset.SouthEast(AdjacentDistance), AdjacentRatio),
			new(Position + OrientationOffset.South(AdjacentDistance), AdjacentRatio),
			new(Position + OrientationOffset.SouthWest(AdjacentDistance), AdjacentRatio),
			new(Position + OrientationOffset.West(AdjacentDistance), AdjacentRatio),
			new(Position + OrientationOffset.NorthWest(AdjacentDistance), AdjacentRatio)
		};
		//Filter adjacets to remove the ones which scape the screen
		adjacents = adjacents.Where(v => !v.IsOffscreen(GetViewportRect().Size.X, GetViewportRect().Size.Y))
							 .Where(v => !v.IntersectWith(OriginalPosition)).ToList();
		//The ant should have a default ratio.
		adjacents.ForEach(v =>
		{
			//Compute the pheromone for each vertex (N, NE ... NW)
			//TODO: Fix the formula to compute pheromone.
			v.Pheromone = pheromones.Where(ph => v.IntersectWith((ph as Pheromone).Position)).Sum(ph => (ph as Pheromone).Value);
			//Compute the quality of each vertex (N, NE, ... NW)
			v.Quality = 1.0 / OriginalPosition.DistanceTo(v.Position);
		});
		GD.Print(string.Join("\n", adjacents));
		return adjacents;
	}

	/// <summary>
	/// Defines the next target position.
	/// </summary>
	private void _DefineNextTargetPosition()
	{
		//Create adjacents vertices;
		var adjacents = _CreateAdjacents();
		//Compute the probability of moving. (If there is no pherormone every vertex has the sabe prob.)
		var probs = adjacents.Select((adj, i) => new { prob = _ProbabilityOfMoving(adj, adjacents), index = i }).OrderBy(p => p.prob);
		GD.Print("Probs, ", string.Join(".", probs));
		//Choose a vertex to set as TargetPosition.
		var randomNumber = Random.Shared.NextDouble();
		var selectedIndex = -1;
		double sumOfProbs = 0;
		foreach (var item in probs)
		{
			sumOfProbs += item.prob;
			if (sumOfProbs >= randomNumber)
			{
				GD.Print("Choosed: ", item);
				selectedIndex = item.index;
				break;
			}
		}
		GD.Print($"Prob result: {selectedIndex} => {randomNumber} | {sumOfProbs}");
		if (sumOfProbs == 0 || selectedIndex == -1)
		{
			selectedIndex = Random.Shared.Next(0, adjacents.Count());
		}
		TargetPosition = adjacents[selectedIndex].Position;
		//Put the choosed vertex in the VisitedVertices list.
		VisitedVertices.Add(adjacents[selectedIndex]);
	}

	private double _ProbabilityOfMoving(Vertex currentAdjacent, List<Vertex> adjacents)
	{
		var nonVisitedAdjacents = adjacents.Where(adj => !VisitedVertices.Any(adj.IntersectWith));
		if (!nonVisitedAdjacents.Any()) return 0;
		if (VisitedVertices.Any(u => u.IntersectWith(currentAdjacent))) return 0;

		var numerator = Math.Pow(currentAdjacent.Pheromone, PheromoneImportance) * Math.Pow(currentAdjacent.Quality, QualityImportance);

		var denominator = nonVisitedAdjacents.Sum(u =>
				Math.Pow(u.Pheromone, PheromoneImportance) * Math.Pow(u.Quality, QualityImportance));
		if (denominator == 0) return 0;
		return numerator / denominator;
	}

	/// <summary>
	/// Verifies if the VisitedVertices consist in a solution for the current problem. 
	/// </summary>
	/// <returns>True if the solution is valid</returns>
	protected virtual bool HasSolution()
	{
		var objectiveChanged = PreviousState != ShouldReturn;
		return objectiveChanged && ShouldReturn;
	}
	private void _DropPheromones(double delta)
	{
		//Copy the solution and erase from the ant. 
		// var solution = VisitedVertices;
		// VisitedVertices = new List<Vertex>();
		// //Compute solution cost
		// var cost = 0;
		// solution = 
		//Create pherormones at each position of solution


		// if (!ShouldReturn) return;
		// NextPherormoneDelay -= delta * Second;
		// if (NextPherormoneDelay <= 0)
		// {
		// 	NextPherormoneDelay = PheromoneDelay;
		// 	var parent = (Node)this.GetParent();
		// 	var pherormoneScene = (PackedScene)GD.Load("res://Pheromone/Pheromone.tscn");
		// 	var pheromone = (Area2D)pherormoneScene.Instantiate();
		// 	pheromone.Position = Position;
		// 	parent.AddChild(pheromone);
		// }
	}

	private void _DropPheromone(Vector2 position, double ph, double dur)
	{
		var parent = (Node)this.GetParent();
		var pherormoneScene = (PackedScene)GD.Load("res://Pheromone/Pheromone.tscn");
		var pheromone = (Pheromone)pherormoneScene.Instantiate();
		pheromone.RenewDuration(dur);
		pheromone.Update(ph);
		pheromone.Name = $"Pheromone {Guid.NewGuid()}";
		pheromone.Position = position;
		parent.AddChild(pheromone);
	}

}
