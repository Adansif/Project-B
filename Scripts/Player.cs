using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public const float NormalSpeed = 100.0f;
	public const float JumpVelocity = -300.0f;
	private const float CrouchSpeed = 40.0f;
	private String currentAnimation = "Idle";
	private Boolean isCrouching = false;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		AudioStreamPlayer2D jump = GetNode<AudioStreamPlayer2D>("./Jump");
		AnimationPlayer animation = GetNode<AnimationPlayer>("./CharacterAnimation");
		Sprite2D sprite = GetNode<Sprite2D>("./CharacterSprite");

		// Add the gravity.
		if (!IsOnFloor()){
			velocity.Y += gravity * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("Jump") && IsOnFloor()){
			velocity.Y = JumpVelocity;
			jump.Play();
			GD.Print("Salto");
		}

		isCrouching = Input.IsActionPressed("Crouch");

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetVector("Left", "Right", "ui_up", "ui_down");
		if (direction != Vector2.Zero)
		{
			velocity.X = isCrouching? direction.X * CrouchSpeed : direction.X * NormalSpeed;
			if (Mathf.Abs(direction.X) > Mathf.Abs(direction.Y))
            {
                currentAnimation = isCrouching? "WalkCrouch" : "Walk";
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
			currentAnimation = isCrouching? "IdleCrouch" : "Idle";
		}

		Velocity = velocity;
		MoveAndSlide();
		animation.Play(currentAnimation);
	}
}
