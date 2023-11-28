using Godot;
using System;

public partial class DroppingPheromones : Node2D
{
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
				_DropPheromone(GetViewport().GetMousePosition());
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
		var node = GetNode<CharacterBody2D>("AntAgent");
		if (node is AntAgent ant)
		{
			ant.ColonyPosition = this.GetNode<Area2D>("AntColony").Position;
		}
		if (!GetTree().Root.GetChild(0).HasNode("Food"))
		{
			var foodScene = (PackedScene)GD.Load("res://Food/Food.tscn");
			var foodNode = foodScene.Instantiate();
			if (foodNode is Area2D foodStatic)
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

	private void _InitializePheromone(){
		var viewportSize = GetViewport().GetWindow().Size;
		for(float i = 0; i < viewportSize.X + 50; i+=50){
			for(float j = 0; j < viewportSize.Y + 50; j+=50){
				_DropPheromone(new(i,j), Random.Shared.NextDouble()/100);
			}
		}
	}
}
