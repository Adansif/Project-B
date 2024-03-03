using Godot;
using System;

public partial class FadingAnimation : CanvasLayer
{
	private AnimationPlayer animation;

	private string[] lvlNames = {"Tutorial", "Lvl1", "Lvl2", "Lvl3", "Lvl4", "Lvl5"};
	public string lvlName;
	Node main, currentLevel;
	Godot.Collections.Array<Node> currentLevelArray;
	// Called when the node enters the scene tree for the first time.
	public override async void _Ready()
	{
		animation = GetNode<AnimationPlayer>("./Animation");
		main = GetParent();
		currentLevelArray = main.GetChildren();
		GD.Print(currentLevelArray);

		foreach(Node node in currentLevelArray){
			foreach(string name in lvlNames){
				if(node.Name == name){
					currentLevel = node;
					lvlName = currentLevel.Name;
					GD.Print(lvlName);
				}
			}
		}

		animation.Play("In");
		await ToSignal(animation, "animation_finished");
		ChangeScene();
		animation.Play("Out");
		main.RemoveChild(currentLevel);
		await ToSignal(animation, "animation_finished");
		main.RemoveChild(this);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void ChangeScene(){
			Node lvl;
		switch(lvlName){
			case "Tutorial":
				lvl = GD.Load<PackedScene>("res://Scenes/Levels/Lvl1.tscn").Instantiate();
				lvl.Name = "Lvl1";
				main.AddChild(lvl);
				main.RemoveChild(this);
				break;
			case "Lvl1":
				lvl = GD.Load<PackedScene>("res://Scenes/Levels/Lvl2.tscn").Instantiate();
				lvl.Name = "Lvl2";
				main.AddChild(lvl);
				main.RemoveChild(this);
				break;
			case "Lvl2":
				lvl = GD.Load<PackedScene>("res://Scenes/Thanks.tscn").Instantiate();
				lvl.Name = "Lvl5";
				main.AddChild(lvl);
				main.RemoveChild(this);
				break;
			
		}
	}
}
