using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public partial class AntAgent : Ant
{
	public double PheromoneImportance = 1;
	public double QualityImportance = 1;
	private double AdjacentDistance => 80 * (Scale.X + Scale.Y)/2;
	private double AdjacentRatio => AdjacentDistance / 2;
	private double SelfVertexRatio => 2 * AdjacentRatio / 5;
	private List<Vertex> VisitedVertices { get; set; }
	protected bool PreviousState { get; set; }
	public AntAgent() : base()
	{
		VisitedVertices = new List<Vertex>();
	}

	private Vertex _GetSelfVertex()
	{
		return new Vertex(Position, SelfVertexRatio, -1);
	}

	public override void _Process(double delta)
	{
		PreviousState = HasFood;
		_ProcessTarget();
		base._Process(delta);
		if (_HasSolution())
		{
			if(!HasFood) this.EditText("FCounter", v=>++v);
			QueueRedraw();
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
	protected virtual List<Vertex> _CreateAdjacents()
	{
		//Get all pheromone from map. 
		var parent = this.GetParent();
		var pheromones = parent.GetChildren().Where(node => node.Name.ToString().Contains("Pheromone"));
		//Create vertex based on current position, N, NE, E, SE, S, SW, W, NW
		List<Vertex> adjacents = new List<Vertex>();
		var orientations = typeof(OrientationOffset).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
		for(var orientation = 0; orientation < orientations.Length; orientation ++){
			var vec = (Vector2)orientations[orientation].Invoke(null, new object[] {AdjacentDistance});
			adjacents.Add(new(Position + vec, AdjacentRatio, orientation));	
		}
		//Filter adjacets to remove the ones which scape the screen
		adjacents = adjacents.Where(v => !v.IsOffscreen(GetViewportRect().Size.X, GetViewportRect().Size.Y)).ToList();
		//The ant should have a default ratio.
		adjacents.ForEach(v =>
		{
			//Compute the pheromone for each vertex (N, NE ... NW)
			var phs = pheromones.Where(ph => v.IntersectWith((ph as Pheromone).Position)).Select(p => p as Pheromone).ToList();
			v.PheromoneCluster = phs;

			//Compute the quality of each vertex (N, NE, ... NW)
			v.Quality = _Heuristic(v);
		});

		return adjacents;
	}

	protected virtual double _Heuristic(Vertex v)
	{
		if (HasFood)
		{
			return 1.0 / ColonyPosition.DistanceSquaredTo(v.Position);
		}
		if (!HasFood)
		{
			if (FoodPosition.HasValue)
			{
				return 1.0 / FoodPosition.Value.DistanceSquaredTo(v.Position);
			}
		}
		return 1.0;
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
			//selectedIndex = Random.Shared.Next(0, adjacents.Count());
		}
		TargetPosition = adjacents[selectedIndex].Position;
		//Put the choosed vertex in the VisitedVertices list.
		VisitedVertices.Add(adjacents[selectedIndex]);
	}

	private void Reset(){
		VisitedVertices = new List<Vertex>();
		if(HasFood){
			Position = FoodPosition.Value;
		}else{
			Position = ColonyPosition;
		}
		TargetPosition = Position;
	}


	public override void _Draw()
	{
		DrawCircle(ToLocal(Position), (float)SelfVertexRatio, new Color
		{
			R = 0,
			G = HasFood ? 1 : 0,
			B = HasFood ? 0 : 1,
			A = .5f,
		});
		base._Draw();
	}

	private double _ProbabilityOfMoving(Vertex currentAdjacent, List<Vertex> adjacents)
	{
		var nonVisitedAdjacents = adjacents.Where(adj => !VisitedVertices.Any(v => adj.IntersectWith(v)));
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
	protected virtual bool _HasSolution()
	{
		var objectiveChanged = PreviousState != HasFood;
		return objectiveChanged;
	}
	private void _DropPheromones(double delta)
	{
		//Copy the solution. 
		var solution = VisitedVertices;
		//Compute solution cost
		var cost = 0.0;
		var path = solution.Zip(solution.Skip(1), (first, second) => new { u = first, v = second });
		foreach (var edge in path)
		{
			cost += edge.u.Position.DistanceTo(edge.v.Position);
		}
		//Create pherormones at each position of solution
		solution.ForEach(v =>
			v.PheromoneCluster.ForEach(pheromone =>
			{
				
				_DropPheromone(pheromone, 1.0 / cost, v.Orientation);
			})
		);

		//Reset visited vertices.
		VisitedVertices = new List<Vertex>();
	}

	private void _DropPheromone(Pheromone pheromone, double ph, int orientation)
	{
		pheromone.Update(ph, orientation);
	}

}
