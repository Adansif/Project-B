using Godot;
using System;

public partial class Multiplayer : Node2D
{
	ENetMultiplayerPeer peer = new();
	[Export]
	PackedScene playerPackedScene;
	Button host, join;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		host = GetNode<Button>("./Host");
		join = GetNode<Button>("./Join");

		host.Pressed += HostPressed;
		join.Pressed += JoinPressed;

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void HostPressed(){
		peer.CreateServer(135);
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
    }

	private void JoinPressed(){
		peer.CreateClient("localhost", 135);
		Multiplayer.MultiplayerPeer = peer;
		host.Hide();
		join.Hide();
	}
}
