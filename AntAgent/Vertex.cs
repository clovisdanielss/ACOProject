using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public partial class Vertex : Area2D
{
	public Guid Id { get; set; }
	public List<Edge> Edges { get; private set; }
	public Vertex()
	{
		Id = Guid.NewGuid();
		Edges = new List<Edge>();
	}

	public override void _Draw()
	{
		foreach (var edge in Edges)
		{
			DrawLine(ToLocal(Position), ToLocal(edge.GetOtherVertex(this).Position), new Color()
			{
				R = 1,
				G = 0,
				B = 0,
				A = (float)edge.Pheromone * 100,
			}, 2);
		}

		DrawCircle(ToLocal(Position), 5, new Color
		{
			R = 1,
			G = 0,
			B = 0,
			A = .5f,
		});
		base._Draw();
	}
	public Edge GetEdge(Vertex v)
	{
		return Edges.FirstOrDefault(e => e.U == v || e.V == v);
	}
	public override bool Equals(object obj)
	{
		if (obj is Vertex u)
		{
			return Id == u.Id;
		}
		return base.Equals(obj);
	}

	public static bool operator ==(Vertex v, object u)
	{
		return v?.Equals(u) ?? u == null;
	}

	public static bool operator !=(Vertex v, object u)
	{
		return !v?.Equals(u) ?? u != null;
	}

}
