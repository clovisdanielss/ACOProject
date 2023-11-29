using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Vertex
{
	public double Radius { get; set; }
	public double Quality { get; set; }
	public List<Pheromone> PheromoneCluster { get; set; }
	public double Pheromone => PheromoneCluster.Sum(p => p.Values[Orientation]);
	public Vector2 Position { get; set; }
	public int Orientation { get; set; }

	public Vertex(Vector2 pos, double r, int orientation)
	{
		PheromoneCluster = new List<Pheromone>();
		Position = pos;
		Radius = r;
		Orientation = orientation;
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
