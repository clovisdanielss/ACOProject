using Godot;
using System;

public partial class Ant : CharacterBody2D
{
	/// <summary>
	/// Ant by default is Idle.
	/// Any ant that moves should hinerit from Ant.
	/// </summary>
	public const float MaxSpeed = 200.0f;
	public const float Speed = 50.0f;
	public const float SpeedAdjustment = .85f;
	public Vector2 TargetPosition { get; set; } = new Vector2(1, 0);
	public Vector2 TargetVector => TargetPosition - Position;
	public Vector2 OriginalPosition { get; set; }
	public bool ShouldReturn { get; set; }

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	public Ant() : base()
	{
		OriginalPosition = Position;
		TargetPosition = Position;
	}

	
	protected virtual void _ProcessDirection()
	{
		LookAt(TargetPosition);
	}
	public override void _Process(double delta)
	{
		_ProcessDirection();
		base._Process(delta);
	}
	public override void _PhysicsProcess(double delta)
	{
		if (Velocity.Length() > MaxSpeed)
		{
			Velocity *= SpeedAdjustment;
		}
		else
		{
			Vector2 velocity = Velocity;
			velocity += TargetVector.Normalized() * Speed;
			Velocity = velocity;
			
		}

		KinematicCollision2D collision = MoveAndCollide(Velocity * (float)delta);
		if(_CheckTargetCollision(collision))
			ShouldReturn = !ShouldReturn;
	}

	private bool _CheckTargetCollision(KinematicCollision2D collision){
		if(collision == null)
			return false;
		else{
			if(collision.GetCollider() is Node food && food.Name.ToString().Contains("Food")){
				food.QueueFree();
			}
			else if(collision.GetCollider() is Node colony && colony.Name.ToString().Contains("AntColony")){
				return ShouldReturn ? true : false;
			}
			return true;
		}
	}
}
