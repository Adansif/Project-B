using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public const float NormalSpeed = 100.0f;
	public const float JumpVelocity = -300.0f;
	private const float CrouchSpeed = 40.0f;
	private String CurrentAnimation = "Idle";

	private bool IsDead {get; set;} = false;
	private bool IsWaypointActivated {get; set;} = false;
	private bool IsCrouching = false;
	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	Vector2 RespawnCoords;
	AnimationPlayer animation;
	AudioStreamPlayer2D jump;
	Sprite2D sprite;
	Camera2D camera;

    public override void _Ready()
    {
		jump = GetNode<AudioStreamPlayer2D>("./Jump");
		animation = GetNode<AnimationPlayer>("./CharacterAnimation");
		sprite = GetNode<Sprite2D>("./CharacterSprite");
		camera = GetNode<Camera2D>("./Camera2D");
		camera.PositionSmoothingEnabled = true;
		RespawnCoords = this.GlobalPosition;
		GD.Print(RespawnCoords);
    }

    public override async void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		if (IsDead){
			await ToSignal(animation, "animation_finished");
			this.GlobalPosition = RespawnCoords;
			IsDead = false;
		}

		if(this.GlobalPosition.Y >= 30){
			camera.LimitBottom = 150;
		}

		if(this.GlobalPosition.Y >= 237){
			this.GlobalPosition = RespawnCoords;
		}

		// Add the gravity.
		if (!IsOnFloor()){
			velocity.Y += gravity * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("Jump") && IsOnFloor()){
			velocity.Y = JumpVelocity;
			jump.Play();
		}

		IsCrouching = Input.IsActionPressed("Crouch");

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetVector("Left", "Right", "ui_up", "ui_down");
		if (direction != Vector2.Zero)
		{
			velocity.X = IsCrouching? direction.X * CrouchSpeed : direction.X * NormalSpeed;
			if (Mathf.Abs(direction.X) > Mathf.Abs(direction.Y))
            {
                CurrentAnimation = IsCrouching? "WalkCrouch" : "Walk";
				if(direction.X < 0 ){
					sprite.FlipH = true;
				}else{
					sprite.FlipH = false;
				}
            }
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, NormalSpeed);
			CurrentAnimation = IsCrouching? "IdleCrouch" : "Idle";
		}

		Velocity = velocity;
		MoveAndSlide();
		animation.Play(CurrentAnimation);
	}

	public void _on_player_area_body_entered(Node body)
	{
		if (!(body is Enemy)) return;
		die();

	}

	private void die(){
		IsDead = true;
		animation.Play("Hit");

	}
}
