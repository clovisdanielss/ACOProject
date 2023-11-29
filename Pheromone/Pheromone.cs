using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// Pheromone will work like Edges. 
/// Since there is no specified graph, we will have diferents values of pheromone for different directions. 
/// It will work simmilarly as the ant sensors, N, NE, E, SE, S, SW, W, NW.
/// </summary>
public partial class Pheromone : Area2D
{
	public double[] Values { get; set; } = new double[8];
	private double EvaporationRate = .3;
	public Pheromone()
	{
	}
	public override void _Draw()
	{
		var orientations = typeof(OrientationOffset).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
		for(var orientation = 0; orientation < orientations.Length; orientation ++){
			DrawLine(ToLocal(Position), ToLocal(Position + (Vector2)orientations[orientation].Invoke(null, new object[] {75})), new Color(){
			R = 1,
			G = 0,
			B = 0,
			A = (float)Values[orientation]*100,
		}, 1);
		}
		base._Draw();
	}
	public override void _Ready()
	{
		base._Ready();
	}
	public void Update(double ph, int orientation){
		var degeneratedValue = (1 - EvaporationRate) * Values[orientation];
		Values[orientation] = ph  + degeneratedValue;
		//Diminui os traços de ferormonio das demais arestas.
		for(var i = 0; i < Values.Length; i++){
			if(i != orientation)
				Values[i] = (1 - EvaporationRate) * Values[i];
		}
		QueueRedraw();
	}
	public override void _Process(double delta)
	{
		base._Process(delta);
	}
}
