using Godot;
using System;

public partial class Multiplayer : Node2D
{
	ENetMultiplayerPeer peer = new();
	MultiplayerFruits multiplayerFruits;
	[Export]
	PackedScene playerPackedScene;
	Button host, join;
	TileMap tilemap;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		host = GetNode<Button>("./Host");
		join = GetNode<Button>("./Join");

		tilemap = GetNode<TileMap>("./TileMap");

		host.Pressed += HostPressed;
		join.Pressed += JoinPressed;

		multiplayerFruits = (MultiplayerFruits)GD.Load<PackedScene>("res://Scenes/Multiplayer/MultiplayerFruits.tscn").Instantiate();

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void HostPressed(){
		peer.CreateServer(50000, 4);
		CallDeferred("add_child", multiplayerFruits);
		Multiplayer.MultiplayerPeer = peer;
		Multiplayer.PeerConnected += AddPlayer;
		AddPlayer();
	}

    private void AddPlayer(long id = 1)
    {
        Node player =  playerPackedScene.Instantiate();
		player.Name = id.ToString();
		CallDeferred("add_child", player);
		host.Hide();
		join.Hide();
		tilemap.Show();
    }

	private void JoinPressed(){
		peer.CreateClient("192.168.1.87", 50000);
		CallDeferred("add_child", multiplayerFruits);
		Multiplayer.MultiplayerPeer = peer;
		host.Hide();
		join.Hide();
		tilemap.Show();
	}
}
