using Godot;
using System;

public partial class Waypoint : StaticBody2D
{
	private bool isActivated = false;

	AnimationPlayer animation;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animation = GetNode<AnimationPlayer>("./Animation");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _on_waypoint_area_body_entered(Node body){
		if (body is Player && isActivated == false){
			playAnim();
			isActivated = true;
		}
	}

	private async void playAnim(){
		animation.Play("Raising");
		await ToSignal(animation, "animation_finished");
		animation.Play("Waving");

	}
}
