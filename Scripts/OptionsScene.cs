using Godot;
using System;

public partial class OptionsScene : Node2D
{

	private Slider masterSlider, musicSlider, effectsSlider;
	private int masterAudio, musicAudio, effectsAudio;
	private Button goBack;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		masterSlider = GetNode<Slider>("./MasterSlider");
		musicSlider = GetNode<Slider>("./MusicSlider");
		effectsSlider = GetNode<Slider>("./EffectsSlider");
		goBack = GetNode<Button>("./Back");

		masterAudio = AudioServer.GetBusIndex("Master");
		musicAudio = AudioServer.GetBusIndex("Music");
		effectsAudio = AudioServer.GetBusIndex("Effects");

		goBack.Pressed += GoToMenu;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		AudioServer.SetBusVolumeDb(masterAudio, Mathf.LinearToDb((float)masterSlider.Value));
		AudioServer.SetBusVolumeDb(musicAudio, Mathf.LinearToDb((float)musicSlider.Value));
		AudioServer.SetBusVolumeDb(effectsAudio, Mathf.LinearToDb((float)effectsSlider.Value));
	}

	private void GoToMenu(){
		GetParent().AddChild(GD.Load<PackedScene>("res://Scenes/Menu.tscn").Instantiate());
		GetParent().RemoveChild(this);
	}
}
