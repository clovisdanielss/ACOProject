using Godot;
using System;
using System.Linq;

public partial class Ant : CharacterBody2D
{
	/// <summary>
	/// Ant by default is Idle.
	/// Any ant that moves should hinerit from Ant.
	/// </summary>
	public const float MaxSpeed = 200.0f;
	public const float Speed = 50.0f;
	public const float SpeedAdjustment = .85f;
	public Vector2? TargetPosition { get; set; } = null;
	public Vector2? TargetVector => TargetPosition.HasValue ? TargetPosition - Position : null;
	public Vector2 OriginalPosition { get; set; }
	public bool ShouldReturn { get; set; }

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	public Ant() : base()
	{
		OriginalPosition = Position;
	}

	
	protected virtual void _ProcessDirection()
	{
		if(TargetPosition.HasValue)
			LookAt(TargetPosition.Value);
	}
	public override void _Process(double delta)
	{
		_ProcessDirection();
		_ProcessMoviment();
		base._Process(delta);
	}

	public void _ProcessMoviment(){
		if(!TargetVector.HasValue) return;
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
		if(_CheckTargetCollision())
			ShouldReturn = !ShouldReturn;
	}
	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
	}

	private bool _CheckTargetCollision(){
		var colony = this.GetParent()?.GetNode<Area2D>("AntColony");
		if(colony != null && colony.OverlapsBody(this)){
			return ShouldReturn ? true : false;
		}
		var foods = this.GetParent()?.GetChildren().Where(n => n.Name.ToString().Contains("Food"));
		var foodCollided = foods.FirstOrDefault(food => (food as Area2D).OverlapsBody(this));
		foodCollided?.QueueFree();
		return foodCollided != null;
	}
}
