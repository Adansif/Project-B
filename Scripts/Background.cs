using Godot;
using System;

public partial class Background : ParallaxLayer
{

	private float BackgroundSpeed = 15.0f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector2 newVector = new Vector2(MotionOffset.X, MotionOffset.Y + (float)(BackgroundSpeed * delta));
		this.MotionOffset = newVector;
	}
}
