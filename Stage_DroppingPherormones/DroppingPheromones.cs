using Godot;
using System;

public partial class DroppingPheromones : Node2D
{
	public override void _Ready()
	{
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent)
		{
			if (mouseEvent.Pressed)
			{
				// Handle mouse button press
				_ManualDropPheromone();
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
					ant.IsStoped = !ant.IsStoped;
				}
			}
		}
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var node = GetNode<CharacterBody2D>("AntAgent");
		var food = GetNode<StaticBody2D>("Food");
		if (node is AntAgent ant)
		{
			ant.OriginalPosition = this.GetNode<StaticBody2D>("AntColony").Position;
		}
		if (food == null)
		{
			var foodScene = (PackedScene)GD.Load("res://Food/Food.tscn");
			var foodNode = foodScene.Instantiate();
			if (foodNode is StaticBody2D foodStatic)
			{
				foodStatic.Position = GetRandomScreenPosition();
			}
			this.AddChild(foodNode);
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

	private void _ManualDropPheromone()
	{
		var parent = (Node)this;
		var pherormoneScene = (PackedScene)GD.Load("res://Pheromone/Pheromone.tscn");
		var pheromone = (Pheromone)pherormoneScene.Instantiate();
		pheromone.Value = 1;
		pheromone.Name = $"Pheromone {Guid.NewGuid()}";
		pheromone.Position = GetViewport().GetMousePosition();
		parent.AddChild(pheromone);
	}
}
