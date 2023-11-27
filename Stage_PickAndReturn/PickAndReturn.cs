using Godot;
using System;

public partial class PickAndReturn : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var node = GetNode<CharacterBody2D>("Ant");
		var food = GetNode<StaticBody2D>("Food");
		if (node is Ant ant)
		{
			ant.OriginalPosition = this.GetNode<StaticBody2D>("AntColony").Position;
			if (ant.ShouldReturn)
			{
				ant.TargetPosition = ant.OriginalPosition;
			}
			else
			{
				ant.TargetPosition = GetViewport().GetMousePosition();
			}
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
}
