using Godot;
using System;

public partial class Pheromone : Area2D
{
	private double InitialValue {get;set;}
	public double Value { get; set; }
	private const double Second = 1000;
	private double InitialDuration {get;set;}
	private double Duration { get; set; }
	public Pheromone()
	{
		InitialDuration = 30000;
		Duration = InitialDuration;
		InitialValue = 1;
		Value = InitialValue;
	}

	public void Update(double ph){
		Value = ph;
		InitialValue = ph;
		Duration = InitialDuration;
	}
	public void RenewDuration(double duration){
		InitialDuration = duration;
		Duration = duration;
	}
	public override void _Process(double delta)
	{
		Duration -= delta * Second;
		Value = InitialValue * Duration/InitialDuration;
		if(Duration < 0){
			this.QueueFree();
		}
		base._Process(delta);
	}
}
