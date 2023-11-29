using Godot;
using System;
using System.Linq;

public partial class AntFactory : Button
{
	public double Alpha { get; set; } = 1;
	public double Beta { get; set; } = 1;
	public float MapScale { get; set; } = 1;
	public override void _Pressed()
	{
		this.EditText("FCounter", v => 0);
		foreach(var c in this.GetParent().GetChildren().Where(c => c is AntAgent)) c.QueueFree();
		foreach(var c in this.GetParent().GetChildren().Where(c => c is Pheromone)) c.QueueFree();
		(this.GetParent() as AntSimulation).Delay = 80000;
		(this.GetParent() as AntSimulation).InitializePheromone();
		for (int i = 0; i < 10; i++)
			_CreateAntAgent();
		base._Pressed();
	}

	private void _CreateAntAgent()
	{
		var parent = (Node)this.GetParent();
		var agentScene = (PackedScene)GD.Load("res://AntAgent/AntAgent.tscn");
		var antAgent = (AntAgent)agentScene.Instantiate();
		antAgent.Position = parent.GetNode<Area2D>("AntColony").Position;
		antAgent.ColonyPosition = antAgent.Position;
		antAgent.TargetPosition = antAgent.Position;
		antAgent.PheromoneImportance = Alpha;
		antAgent.QualityImportance = Beta;
		antAgent.Scale = new Vector2(MapScale, MapScale);
		parent.AddChild(antAgent);
	}
}
