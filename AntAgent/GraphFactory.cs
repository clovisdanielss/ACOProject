using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public static class GraphFactory{
    private static Vertex _CreateVertex(Vector2 position, Node parent)
	{
		var vertexScene = (PackedScene)GD.Load("res://Vertex/Vertex.tscn");
		var vertex = (Vertex)vertexScene.Instantiate();
		vertex.Name = $"Vertex {vertex.Id}";
		vertex.Position = position;
		parent.AddChild(vertex);
		return vertex;
	}
	public static Vertex CreateGraph(List<Edge> graph, Node parent, int offset, Viewport viewport, string origin = "AntColony")
	{
		var viewportSize = viewport.GetWindow().Size;
		List<Vertex> vertices = new List<Vertex>();
		for (float i = 0; i < viewportSize.X + offset; i += offset)
		{
			for (float j = 0; j < viewportSize.Y + offset; j += offset)
			{
				vertices.Add(_CreateVertex(new(i + Random.Shared.Next(-offset / 3, offset / 3), j + Random.Shared.Next(-offset / 3, offset / 3)), parent));
			}
		}
		var colony = parent.GetNode<Area2D>(origin);
		var colonyVertex = _CreateVertex(colony.Position, parent);
		vertices.Add(colonyVertex);
		var foods = parent.GetChildren().Where(x => x.Name.ToString().Contains("Food")).Select(x => x as Area2D);
		foreach (var food in foods)
		{
			vertices.Add(_CreateVertex(food.Position, parent));
		}

		vertices.ForEach(v =>
		{
			var adjacents = vertices.Where(u => v.Position.IntersectWith(u.Position, offset * 1.34f));

			foreach (var adj in adjacents)
			{
				if (v != adj)
				{
					if (!graph.Any(e => e.IsThisEdge(v, adj)))
					{
						var edge = new Edge(v, adj, Random.Shared.NextDouble());
						graph.Add(edge);
					}
				}
			}
		});

		return colonyVertex;
	}
}