using Godot;
using System;

public partial class Thanks : Node2D
{
	Button menu, exit;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		menu = GetNode<HBoxContainer>("./ButtonContainer").GetNode<Button>("./Menu");
		exit = GetNode<HBoxContainer>("./ButtonContainer").GetNode<Button>("./Exit");

		menu.Pressed += LoadMenu;

		GetTree().Paused = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void LoadMenu(){
		GetTree().Paused = false;
		GetParent().AddChild(GD.Load<PackedScene>("res://Scenes/Menu.tscn").Instantiate());
		GetParent().RemoveChild(this);
	}

	private void ExitGame(){
		GetTree().Paused = false;
		GetTree().Quit();
	}
}
