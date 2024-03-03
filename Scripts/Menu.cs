using Godot;
using System;

public partial class Menu : Node2D
{
	Button singleplayer, multiplayer, options, exit;
	Node main;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		singleplayer = GetNode<Button>("./Singleplayer");
		multiplayer = GetNode<Button>("./Multiplayer");
		options = GetNode<Button>("./Options");
		exit = GetNode<Button>("./Exit");

		main = GetParent();

		exit.Pressed += Exit;
		singleplayer.Pressed += LoadSingleplayer;
		multiplayer.Pressed += LoadMultiplayer;
		options.Pressed += LoadOptions;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}


	private void Exit(){
		GetTree().Quit();
	}

	private void LoadSingleplayer(){
		Node singleplayer = GD.Load<PackedScene>("res://Scenes/Levels/Tutorial.tscn").Instantiate();
		main.AddChild(singleplayer);
		main.RemoveChild(this);
	}

	private void LoadMultiplayer(){
		Node singleplayer = GD.Load<PackedScene>("res://Scenes/Multiplayer/Multiplayer.tscn").Instantiate();
		main.AddChild(singleplayer);
		main.RemoveChild(this);
	}

	private void LoadOptions(){
		GetParent().AddChild(GD.Load<PackedScene>("res://Scenes/OptionsScene.tscn").Instantiate());
		GetParent().RemoveChild(this);
	}
}
