using Godot;
using System;

public partial class Fruit : StaticBody2D
{
	private Random random = new Random();
	private Texture texture;
	private Sprite2D sprite;
	private AnimationPlayer animation;
	public Vector2 position;
	private Player playerScript;
	private Timer timer;
	public int textureId = 1;
	private string fruitPath = "res://Sprites/Fruits/";
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		sprite = GetNode<Sprite2D>("./Sprite");
		animation = GetNode<AnimationPlayer>("./Animation");
		timer = GetNode<Timer>("./Timer");
		this.GlobalPosition = position;

		switch(textureId){
			case 1:
				texture = GD.Load<Texture>(fruitPath + "Apple.png");
				break;
			case 2:
				texture = GD.Load<Texture>(fruitPath + "Cherries.png");
				break;
			case 3:
				texture = GD.Load<Texture>(fruitPath + "Bananas.png");
				break;
			case 4:
				texture = GD.Load<Texture>(fruitPath + "Kiwi.png");
				break;
			default:
				texture = GD.Load<Texture>(fruitPath + "Apple.png");
				break;
		}
		if(GetParent().Name == "MultiplayerFruits"){
			sprite.Texture = (Texture2D)texture;
			timer.Start();
		}else{
			sprite.Texture = (Texture2D) GD.Load<Texture>("res://Sprites/Fruits/Apple.png");
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void _on_fruit_area_body_entered(Node body){
		if (body is Player){
			playerScript = (Player)body;
			if(playerScript.TextureId == this.textureId){
				GetHit();
			}
		}
	}

	private  async void GetHit(){
		animation.Play("Hit");
		await ToSignal(animation, "animation_finished");
		playerScript.fruitCount++;
		FruitPositions.GetUsedPositions().Remove(this.GlobalPosition);
		GetParent().RemoveChild(this);
	}

	private void _on_timer_timeout(){
		FruitPositions.GetUsedPositions().Remove(this.GlobalPosition);
		GetParent().RemoveChild(this);
	}
}
