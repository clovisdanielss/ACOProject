using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

public partial class Vertex : Area2D
{
	private static bool Debug { get; set; } = true;
	public Guid Id { get; set; }
	public List<Edge> Edges { get; private set; }
	public Vertex()
	{
		Id = Guid.NewGuid();
		Edges = new List<Edge>();
	}

	public static void SetDebug()
	{
		Vertex.Debug = !Vertex.Debug;
	}
	public override void _Draw()
	{
		if (!Debug) { base._Draw(); return; };
		foreach (var edge in Edges)
		{
			DrawLine(ToLocal(Position), ToLocal(edge.GetOtherVertex(this).Position), new Color()
			{
				R = 1,
				G = 0,
				B = 0,
				A = (float)(edge.Pheromone)*1000,
			}, 2);
			DrawLine(ToLocal(Position), ToLocal(edge.GetOtherVertex(this).Position), new Color()
			{
				R = 0,
				G = 0,
				B = 1,
				A = 0.3f,
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

	public override string ToString()
	{
		return $"Vertex {Id} at position {Position}";
	}

}
