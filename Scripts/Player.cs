using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public const float normalSpeed = 100.0f;
	public const float jumpVelocity = -300.0f;
	private const float crouchSpeed = 40.0f;
	private String currentAnimation = "Idle";
	private String fatherName;

	private bool IsDead {get; set;} = false;
	private bool IsWaypointActivated {get; set;} = false;
	private bool isChangingScreen = false;
	private bool isCrouching = false;
	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	Vector2 respawnCoords;
	AnimationPlayer animation;
	AudioStreamPlayer2D jump;
	Sprite2D sprite;
	Camera2D camera;
	Node main;

    public override void _Ready()
    {
		jump = GetNode<AudioStreamPlayer2D>("./Jump");
		animation = GetNode<AnimationPlayer>("./CharacterAnimation");
		sprite = GetNode<Sprite2D>("./CharacterSprite");
		camera = GetNode<Camera2D>("./Camera2D");
		camera.PositionSmoothingEnabled = true;
		respawnCoords = this.GlobalPosition;
		fatherName = GetParent().Name;
		main = GetParent().GetParent();
		GD.Print(fatherName);
		GD.Print(main.Name);

    }

    public override void _EnterTree()
    {
		SetMultiplayerAuthority(Int32.Parse(Name));
    }

    public override async void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		if(IsMultiplayerAuthority()){


			if (IsDead){
				await ToSignal(animation, "animation_finished");
				this.GlobalPosition = respawnCoords;
				IsDead = false;
			}

			if(this.GlobalPosition.Y >= 30){
				camera.LimitBottom = 150;
			}

			if(this.GlobalPosition.Y >= 237){
				this.GlobalPosition = respawnCoords;
			}

			// Add the gravity.
			if (!IsOnFloor()){
				velocity.Y += gravity * (float)delta;
			}

			// Handle Jump.
			if (Input.IsActionJustPressed("Jump") && IsOnFloor()){
				velocity.Y = jumpVelocity;
				jump.Play();
			}

			isCrouching = Input.IsActionPressed("Crouch");

			// Get the input direction and handle the movement/deceleration.
			// As good practice, you should replace UI actions with custom gameplay actions.
			Vector2 direction = Input.GetVector("Left", "Right", "ui_up", "ui_down");
			if (direction != Vector2.Zero)
			{
				velocity.X = isCrouching? direction.X * crouchSpeed : direction.X * normalSpeed;
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
				velocity.X = Mathf.MoveToward(Velocity.X, 0, normalSpeed);
				currentAnimation = isCrouching? "IdleCrouch" : "Idle";
			}
		}
		Velocity = velocity;
		MoveAndSlide();
		animation.Play(currentAnimation);
	}

	public void _on_player_area_body_entered(Node body)
	{
		if (body is Waypoint){
			StaticBody2D waypoint = (StaticBody2D)body;
			respawnCoords = waypoint.GlobalPosition;
			GD.Print(respawnCoords);
		}

		if(isChangingScreen == false){
			main.AddChild(GD.Load<PackedScene>("res://Scenes/FadingAnimation.tscn").Instantiate());
			isChangingScreen = true;
		}
		
		if (!(body is Enemy)) return;

		die();

	}

	private void die(){
		IsDead = true;
		animation.Play("Hit");

	}
}
