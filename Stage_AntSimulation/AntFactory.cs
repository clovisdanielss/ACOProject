using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class AntFactory : Button
{
	public double Alpha { get; set; } = 1;
	public double Beta { get; set; } = 1;
	public float MapScale { get; set; } = 1;
	public int Ants { get; set; } = 10;
	public override void _Pressed()
	{
		this.EditText("FCounter", v => 0);
		foreach (var c in GetTree().Root.GetChild(0).GetChildren().Where(c => c is AntAgent)) c.QueueFree();
		foreach (var c in GetTree().Root.GetChild(0).GetChildren().Where(c => c is Vertex)) c.QueueFree();
		(GetTree().Root.GetChild(0) as AntSimulation).Delay = 80000;
		var graph = new List<Edge>();
		var origin = GraphFactory.CreateGraph(graph, GetTree().Root.GetChild(0), 150, GetViewport());
		for (int i = 0; i < Ants; i++)
			_CreateAntAgent(origin, graph);
		base._Pressed();
	}

	private void _CreateAntAgent(Vertex origin, List<Edge> graph)
	{
		var parent = GetTree().Root.GetChild(0);
		var agentScene = (PackedScene)GD.Load("res://AntAgent/AntAgent.tscn");
		var antAgent = (AntAgent)agentScene.Instantiate();
		antAgent.Position = parent.GetNode<Area2D>("AntColony").Position;
		antAgent.ColonyPosition = antAgent.Position;
		antAgent.TargetPosition = antAgent.Position;
		antAgent.Graph = graph;
		antAgent.Alpha = Alpha;
		antAgent.Beta = Beta;
		antAgent.CurrentVertex = origin;
		parent.AddChild(antAgent);
	}
}
