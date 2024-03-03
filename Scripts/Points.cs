using Godot;
using System;

public partial class Points : Node2D
{
	public Texture headTexture, fruitTexture;
	public Sprite2D head, fruit;
	public Label label;
	public int points;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		head = GetNode<Sprite2D>("./Head");
		fruit = GetNode<Sprite2D>("./Fruit");

		label= GetNode<Label>("./PointLabel");

		head.Texture = (Texture2D)headTexture;
		fruit.Texture = (Texture2D)fruitTexture;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(IsMultiplayerAuthority()){
			label.Text = points.ToString();
		}
	}
}
