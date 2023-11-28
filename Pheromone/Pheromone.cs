using Godot;
using System;

public partial class Pheromone : Area2D
{
	public double Value { get; set; }
	private double EvaporationRate = .5;
	public Pheromone()
	{
	}
	public override void _Draw()
	{
		DrawCircle(ToLocal(Position), 75, new Color(){
			R = 1,
			G = 0,
			B = 0,
			A = (float)Value*100,
		});
		base._Draw();
	}
	public override void _Ready()
	{
		base._Ready();
	}
	public void Update(double ph){
		var degeneratedValue = (1 - EvaporationRate) * Value;
		Value = ph * EvaporationRate + degeneratedValue;
		QueueRedraw();
	}
	public override void _Process(double delta)
	{
		if(Value <= 0){
			this.QueueFree();
		}
		base._Process(delta);
	}
}
