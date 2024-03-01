using Godot;
using System;
using System.Collections;

public partial class MultiplayerFruits : Node
{
	public int fruitType = 1;
	private Random random = new Random();
	Fruit fruit;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		foreach(Vector2 position in FruitPositions.GetPositionsArray()){
			if (!FruitPositions.GetUsedPositions().Contains(position) && random.Next(1, 1001) == 25){
				fruit = (Fruit)GD.Load<PackedScene>("res://Scenes/Fruit.tscn").Instantiate();
				fruit.textureId = fruitType;
				fruit.position = position;
				FruitPositions.AddUsePositions(position);
				this.AddChild(fruit);
				
			}
		}
	}
}
