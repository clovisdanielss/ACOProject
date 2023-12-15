using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class SetOfSimulationParameters
{
	public double InitialTimer = 0;
	public int GridOffset;
	public double Alpha { get; set; } = 1;
	public double Beta { get; set; } = 1;
	public int Ants { get; set; } = 10;
	public int IterationsPerSetOfParameters { get; set; } = 10;

	public void Update(SetOfSimulationParameters input)
	{
		InitialTimer = input.InitialTimer;
		GridOffset = input.GridOffset;
		Alpha = input.Alpha;
		Beta = input.Beta;
		Ants = input.Ants;
		IterationsPerSetOfParameters = input.IterationsPerSetOfParameters;
	}
}
public partial class AntDynamicSimulation : Node2D
{
	public SetOfSimulationParameters SimulationParameters { get; set; } = new SetOfSimulationParameters();
	public double InitialTimer => SimulationParameters.InitialTimer;
	public double Timer = 0;
	public int GridOffset => SimulationParameters.GridOffset;
	public double Alpha => SimulationParameters.Alpha;
	public double Beta => SimulationParameters.Beta;
	public int Ants => SimulationParameters.Ants;
	public int IterationsPerSetOfParameters => SimulationParameters.IterationsPerSetOfParameters;
	public Stack<SetOfSimulationParameters> Simulations { get; set; }
	public bool ShouldOutput = true;
	public List<Edge> Graph = new List<Edge>();
	public override void _Ready()
	{
		this.AppendText("Output", "Alpha; Beta; Ants; FoodConsumed; GridPheromoneOffset; Time");
		Simulations = new Stack<SetOfSimulationParameters>();
		Simulations.Push(new()
		{
			Alpha = 1,
			Beta = 1,
			Ants = 20,
			GridOffset = 100,
			InitialTimer = 600000,
			IterationsPerSetOfParameters = 2,
		});
		Simulations.Push(new()
		{
			Alpha = 0,
			Beta = 1,
			Ants = 20,
			GridOffset = 100,
			InitialTimer = 5000,
			IterationsPerSetOfParameters = 1,
		});
		SimulationParameters.Update(Simulations.Pop());
		Simulate();
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey key)
		{
			if (key.Pressed)
			{
				Vertex.SetDebug();
			}
		}
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Timer -= delta * 1000;
		if (Timer >= 0)
			this.EditText("SimulationTime", v => (float)Timer / 1000);
		if (Timer <= 0 && ContinueSimulation())
		{
			Simulate();
		}
	}

	private bool ContinueSimulation()
	{
		if (ShouldOutput)
		{
			this.AppendText("Output", $"{Alpha}; {Beta}; {Ants}; {this.GetParameterFromText("FCounter")}; {GridOffset}; {InitialTimer}");
		}
		if (--SimulationParameters.IterationsPerSetOfParameters > 0) return true;
		else
		{
			if (Simulations.Count == 0)
			{
				foreach (var c in this.GetChildren().Where(c => c is AntAgent)) c.QueueFree();
				foreach (var c in this.GetChildren().Where(c => c is Vertex)) c.QueueFree();
				ShouldOutput = false;
				return false;
			}
			SimulationParameters.Update(Simulations.Pop());
			return true;
		}
	}

	private void Simulate()
	{
		this.EditText("FCounter", v => 0);
		this.EditText("Alpha", v => (float)Alpha);
		this.EditText("Beta", v => (float)Beta);
		this.EditText("Ants", v => (float)Ants);
		foreach (var c in this.GetChildren().Where(c => c is AntAgent)) c.QueueFree();
		foreach (var c in this.GetChildren().Where(c => c is Vertex)) c.QueueFree();
		Timer = InitialTimer;
		Graph = new List<Edge>();
		var origin = GraphFactory.CreateGraph(Graph, GetTree().Root.GetChild(0), GridOffset, GetViewport());
		for (int i = 0; i < Ants; i++)
			_CreateAntAgent(origin);
	}
	private double Normalize(double val)
	{
		var viewportSize = GetViewport().GetWindow().Size;
		var distOrigin = new Vector2(viewportSize.X, viewportSize.Y).DistanceTo(new(0, 0));
		return val / distOrigin;
	}

	private void _CreateAntAgent(Vertex origin)
	{
		var parent = (Node)this;
		var agentScene = (PackedScene)GD.Load("res://AntAgent/AntAgent.tscn");
		var antAgent = (AntAgent)agentScene.Instantiate();
		antAgent.Position = parent.GetNode<Area2D>("AntColony").Position;
		antAgent.ColonyPosition = antAgent.Position;
		antAgent.TargetPosition = antAgent.Position;
		antAgent.Graph = Graph;
		antAgent.Alpha = Alpha;
		antAgent.Beta = Beta;
		antAgent.CurrentVertex = origin;
		parent.AddChild(antAgent);
	}
}
