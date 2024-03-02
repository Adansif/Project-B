using Godot;
using System;
using System.Collections;

public partial class MultiplayerFruits : Node
{
	private int fruitMaxType = 1;
	private Random random = new Random();
	[Export]
	public Godot.Collections.Array<Node> nodes;
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
			switch(GetParent().GetChildCount()){
				case 7:
					fruitMaxType = 1;
					break;
				case 8:
					fruitMaxType = 2;
					break;
				case 9:
					fruitMaxType = 3;
					break;
				case 10:
					fruitMaxType = 4;
					break;
				default:
					fruitMaxType = 1;
					break;
			}
			
			foreach(Vector2 position in FruitPositions.GetPositionsArray()){
				if(GetChildCount() < 10){
					if (!FruitPositions.GetUsedPositions().Contains(position) && random.Next(1, 701) == 69){
						Fruit fruit = (Fruit)GD.Load<PackedScene>("res://Scenes/Fruit.tscn").Instantiate();
						fruit.textureId = random.Next(1, fruitMaxType + 1);
						fruit.position = position;
						FruitPositions.AddUsePositions(position);
						this.AddChild(fruit, false, InternalMode.Disabled);
						GetChildren();
					}
				}
			}
			nodes = GetChildren();
		
	}
}
