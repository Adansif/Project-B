using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public const float normalSpeed = 100.0f;
	public const float jumpVelocity = -300.0f;
	private const float crouchSpeed = 40.0f;
	private string currentAnimation = "Idle";
	private string fatherName;
	private string texturePath = "res://Sprites/Characters/";

	private bool IsDead {get; set;} = false;
	private bool IsWaypointActivated {get; set;} = false;
	private bool isChangingScreen = false;
	private bool isCrouching = false;
	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	private Vector2 respawnCoords;
	private AnimationPlayer animation;
	private AudioStreamPlayer2D jump;
	private Sprite2D sprite;
	private Camera2D camera;
	private Node main;
	private Texture texture;

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

		switch(GetParent().GetChildCount()){
			case 5:
				texture = GD.Load<Texture>(texturePath + "Doux.png");
				break;
			case 6:
				texture = GD.Load<Texture>(texturePath + "Mort.png");
				break;
			case 7:
				texture = GD.Load<Texture>(texturePath + "Tard.png");
				break;
			case 8:
				texture = GD.Load<Texture>(texturePath + "Vita.png");
				break;
			default:
				texture = GD.Load<Texture>(texturePath + "Doux.png");
				break;
		}

		sprite.Texture = (Texture2D) texture;
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
