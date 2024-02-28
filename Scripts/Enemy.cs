using Godot;
using System;

public partial class Enemy : Node2D
{
	Random random = new Random();

	Sprite2D sprite;
	AnimationPlayer animation;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		sprite = this.GetNode<Sprite2D>("Sprite");
		animation = this.GetNode<AnimationPlayer>("Animation");

		sprite.Frame = 4;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override async void _Process(double delta)
	{
		int randomNumber = random.Next(1, 201);

		if(randomNumber == 100){
			GD.Print("Entro aqui");
			animation.Play("Blink");
		}else{
			await ToSignal(animation, "animation_finished");
			sprite.Frame = 4;
		}
	}
}
