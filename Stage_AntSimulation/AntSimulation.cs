using Godot;
using System;
using System.Linq;

public partial class AntSimulation : Node2D
{
	public double Delay = 0;
	public override void _Ready()
	{
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey key)
		{
			if (key.Pressed)
			{
				GD.Print("Control pressed");
				var node = GetNode<CharacterBody2D>("AntAgent");
				if (node is AntAgent ant)
				{
					ant.TargetPosition = ant.Position;
				}
			}
		}
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Delay -= delta * 1000;
		if (Delay <= 0){
			foreach(var c in this.GetChildren().Where(c => c is AntAgent)) c.QueueFree();
		}
	}

	public Vector2 GetRandomScreenPosition()
	{
		// Get the size of the viewport
		var viewportSize = GetViewport().GetWindow().Size;

		// Generate a random position within the viewport
		var x = GD.Randf() * viewportSize.X;
		var y = GD.Randf() * viewportSize.Y;

		return new Vector2(x, y);
	}

	// private void _DropPheromone(Vector2 position)
	// {
	// 	var parent = (Node)this;
	// 	var pherormoneScene = (PackedScene)GD.Load("res://Pheromone/Pheromone.tscn");
	// 	var pheromone = (Edge)pherormoneScene.Instantiate();
	// 	for(var orientation = 0; orientation < pheromone.Values.Length; orientation++){
	// 		pheromone.Values[orientation] = Normalize(Random.Shared.NextDouble());	
	// 	}
	// 	pheromone.Name = $"Pheromone {Guid.NewGuid()}";
	// 	pheromone.Position = position;
	// 	parent.AddChild(pheromone);
	// }

	private double Normalize(double val){
		var viewportSize = GetViewport().GetWindow().Size;
		var distOrigin = new Vector2(viewportSize.X, viewportSize.Y).DistanceTo(new(0,0));
		return val/distOrigin;
	}

	

	// public void InitializePheromone()
	// {
	// 	var offset = 50;
	// 	var viewportSize = GetViewport().GetWindow().Size;
	// 	for (float i = 0; i < viewportSize.X + offset; i += offset)
	// 	{
	// 		for (float j = 0; j < viewportSize.Y + offset; j += offset)
	// 		{
	// 			_DropPheromone(new(i, j));
	// 		}
	// 	}
	// }
}
