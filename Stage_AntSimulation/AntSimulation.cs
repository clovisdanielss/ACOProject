using Godot;
using System;
using System.Linq;

public partial class AntSimulation : Node2D
{
	private double Delay = 10000;
	public override void _Ready()
	{
		_InitializePheromone();
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent)
		{
			if (mouseEvent.Pressed)
			{
				// Handle mouse button press
				_CreateAntAgent();
			}
		}
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

	private void _DropPheromone(Vector2 position, double val = 1.0)
	{
		var parent = (Node)this;
		var pherormoneScene = (PackedScene)GD.Load("res://Pheromone/Pheromone.tscn");
		var pheromone = (Pheromone)pherormoneScene.Instantiate();
		pheromone.Update(val);
		pheromone.Name = $"Pheromone {Guid.NewGuid()}";
		pheromone.Position = position;
		parent.AddChild(pheromone);
	}

	private void _CreateAntAgent()
	{
		var parent = (Node)this;
		var agentScene = (PackedScene)GD.Load("res://AntAgent/AntAgent.tscn");
		var antAgent = (AntAgent)agentScene.Instantiate();
		antAgent.Position = this.GetNode<Area2D>("AntColony").Position;
		antAgent.ColonyPosition = antAgent.Position;
		antAgent.TargetPosition = antAgent.Position;
		parent.AddChild(antAgent);
	}

	private void _InitializePheromone()
	{
		var offset = 50;
		var viewportSize = GetViewport().GetWindow().Size;
		for (float i = 0; i < viewportSize.X + offset; i += offset)
		{
			for (float j = 0; j < viewportSize.Y + offset; j += offset)
			{
				_DropPheromone(new(i, j), Random.Shared.NextDouble() / 100);
			}
		}
	}
}
