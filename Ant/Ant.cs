using Godot;
using System;
using System.Linq;

public partial class Ant : CharacterBody2D
{
	/// <summary>
	/// Ant by default is Idle.
	/// Any ant that moves should hinerit from Ant.
	/// </summary>
	public const float MaxSpeed = 500.0f;
	public const float Speed = 500.0f;
	public const float SpeedAdjustment = .5f;
	public Vector2? TargetPosition { get; set; } = null;
	public Vector2? TargetVector => TargetPosition.HasValue ? TargetPosition - Position : null;
	public Vector2 ColonyPosition { get; set; }
	public Vector2? FoodPosition { get; set; }
	public bool HasFood { get; set; }

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	public Ant() : base()
	{
		ColonyPosition = Position;
	}


	protected virtual void _ProcessDirection()
	{
		if (TargetPosition.HasValue)
			LookAt(TargetPosition.Value);
	}
	public override void _Process(double delta)
	{
		_ProcessDirection();
		_ProcessMoviment();
		base._Process(delta);
	}

	public void _ProcessMoviment()
	{
		if (!TargetVector.HasValue) return;
		if (Velocity.Length() > MaxSpeed)
		{
			Velocity *= SpeedAdjustment;
		}
		else
		{
			Vector2 velocity = Velocity;
			velocity += TargetVector.Value.Normalized() * Speed;
			Velocity = velocity;

		}

		MoveAndSlide();
		_GoSearchForFood();
		_GoSearchForColony();
	}
	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
	}

	private void _GoSearchForFood()
	{
		var colony = this.GetParent()?.GetNode<Area2D>("AntColony");
		if (colony != null && colony.OverlapsBody(this))
		{
			if (HasFood) HasFood = false;
		}
	}

	private void _GoSearchForColony()
	{
		var foods = this.GetParent()?.GetChildren().Where(n => n.Name.ToString().Contains("Food"));
		var food = foods.FirstOrDefault(food => (food as Area2D).OverlapsBody(this));
		if (food != null)
		{
			FoodPosition = (food as Area2D).Position;
			if (!HasFood) HasFood = true;
		}
	}
}
