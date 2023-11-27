using Godot;
using System;

public partial class Pheromone : Area2D
{
	public double Value { get; set; }
	private const double Second = 1000;
	public double Duration { get; set; }
	public Pheromone()
	{
		Duration = 15000;
		Value = 1;
	}
	public override void _Process(double delta)
	{
		Duration -= delta * Second;
		if(Duration < 0){
			this.QueueFree();
		}
		base._Process(delta);
	}
}
