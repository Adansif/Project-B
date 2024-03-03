using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public const float normalSpeed = 100.0f;
	public const float jumpVelocity = -300.0f;
	private const float crouchSpeed = 40.0f;
	[Export]
	public int TextureId;
	public int fruitCount;
	private string currentAnimation = "Idle";
	private string fatherName;
	private string characterTexturePath = "res://Sprites/Characters/";
	private string fruitTexturePath = "res://Sprites/Fruits/";

	private bool IsDead {get; set;} = false;
	private bool IsWaypointActivated {get; set;} = false;
	private bool isCrouching = false;
	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	private Vector2 respawnCoords;
	private AnimationPlayer animation;
	private AudioStreamPlayer2D jump;
	private Sprite2D sprite;
	private Camera2D playerCamera, multiplayerCamera;
	private Node main;
	private Texture texture, headTexture, fruitTexture;
	private Label pointLabel;
	private Points pointsScript;

    public override void _Ready()
    {
		jump = GetNode<AudioStreamPlayer2D>("./Jump");
		animation = GetNode<AnimationPlayer>("./CharacterAnimation");
		sprite = GetNode<Sprite2D>("./CharacterSprite");
		playerCamera = GetNode<Camera2D>("./Camera2D");
		playerCamera.PositionSmoothingEnabled = true;
		respawnCoords = this.GlobalPosition;
		fatherName = GetParent().Name;
		main = GetParent().GetParent();
		GD.Print(fatherName);
		GD.Print(main.Name);


		switch(GetParent().GetChildCount()){
			case 7:
				texture = GD.Load<Texture>(characterTexturePath + "Doux.png");
				headTexture = GD.Load<Texture>(characterTexturePath + "DouxHead.png");
				fruitTexture = GD.Load<Texture>(fruitTexturePath + "Apple.png");
				TextureId = 1;
				break;
			case 8:
				texture = GD.Load<Texture>(characterTexturePath + "Mort.png");
				headTexture = GD.Load<Texture>(characterTexturePath + "MortHead.png");
				fruitTexture = GD.Load<Texture>(fruitTexturePath + "Cherries.png");
				TextureId = 2;
				break;
			case 9:
				texture = GD.Load<Texture>(characterTexturePath + "Tard.png");
				headTexture = GD.Load<Texture>(characterTexturePath + "TardHead.png");
				fruitTexture = GD.Load<Texture>(fruitTexturePath + "Banana.png");
				TextureId = 3;
				break;
			case 10:
				texture = GD.Load<Texture>(characterTexturePath + "Vita.png");
				headTexture = GD.Load<Texture>(characterTexturePath + "VitaHead.png");
				fruitTexture = GD.Load<Texture>(fruitTexturePath + "Kiwi.png");
				TextureId = 4;
				break;
			default:
				texture = GD.Load<Texture>(characterTexturePath + "Doux.png");
				headTexture = GD.Load<Texture>(characterTexturePath + "DouxHead.png");
				fruitTexture = GD.Load<Texture>(fruitTexturePath + "Apple.png");
				TextureId = 1;
				break;
		}

		pointsScript = (Points)GD.Load<PackedScene>("res://Scenes/Points.tscn").Instantiate();
		pointsScript.headTexture = this.headTexture;
		pointsScript.fruitTexture = this.fruitTexture;

		if(fatherName == "Multiplayer"){
			multiplayerCamera = GetParent().GetNode<Camera2D>("./Camera2D");
			
			switch(multiplayerCamera.GetChildCount()){
				case 0:
					multiplayerCamera.AddChild(pointsScript);
					break;
				case 1:
					pointsScript.GlobalPosition = new Vector2(pointsScript.GlobalPosition.X + 100, pointsScript.GlobalPosition.Y);
					multiplayerCamera.AddChild(pointsScript);
					break;
				case 2:
					pointsScript.GlobalPosition = new Vector2(pointsScript.GlobalPosition.X + 200, pointsScript.GlobalPosition.Y);
					multiplayerCamera.AddChild(pointsScript);
					break;
				case 3:
					pointsScript.GlobalPosition = new Vector2(pointsScript.GlobalPosition.X + 300, pointsScript.GlobalPosition.Y);
					multiplayerCamera.AddChild(pointsScript);
					break;
			}
		}else{
			this.AddChild(pointsScript);
		}

		sprite.Texture = (Texture2D) texture;
		GD.Print(GetParent().GetChildren());
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
				playerCamera.LimitBottom = 150;
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
		pointsScript.points = fruitCount;
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
			return;
		}
		if ((body is Enemy)){
			die();
			return;
		}

		
		main.AddChild(GD.Load<PackedScene>("res://Scenes/FadingAnimation.tscn").Instantiate());
		GD.Print("He chocado");
		
	}

	private void die(){
		IsDead = true;
		animation.Play("Hit");
	}
}
