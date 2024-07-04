using System;
using Godot;

namespace GodotTest.Scripts;

public enum PieceType
{
	Red, Green, Blue, Yellow, Violet
}
public partial class Piece : Node2D
{
	private static readonly Random Rnd = new();
	private bool _updateColor;

	public const int Size = 16;

	public bool IsFakePiece = false;
	public PieceType Type;
	public AnimationPlayer AnimationPlayer;
	public Sprite2D Sprite;

	public override void _Ready()
	{
		AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		Sprite = GetNode<Sprite2D>("Sprite");
		AnimationPlayer.CurrentAnimation = "idle";
		SetColor();
	}

	public void UpdateType(PieceType type)
	{
		Type = type;
		SetColor();
	}
	
	private void SetColor()
	{
		switch (Type)
		{
			case PieceType.Blue:
				Sprite.Frame = 1;
				break;
			case PieceType.Red:
				Sprite.Frame = 2;
				break;
			case PieceType.Yellow:
				Sprite.Frame = 3;
				break;
			case PieceType.Green:
				Sprite.Frame = 4;
				break;
			case PieceType.Violet:
				Sprite.Frame = 0;
				break;
		}
	}

	

	public void SetRandomPiece()
	{		
		var values = Enum.GetValues<PieceType>();
		Type = (values[Rnd.Next(values.Length)]);
	}

	public void AnimateLeft()
	{
		 AnimationPlayer.Play("left", customSpeed: 2);
		 _updateColor = true;			 
	}
	
	public void AnimateRight()
	{
		AnimationPlayer.Play("right", customSpeed: 2);
		_updateColor = true;	
	}
	
	public void AnimateDown()
	{
		AnimationPlayer.Play("down", customSpeed: 2);
		_updateColor = true;	
	}

	public void AnimateUp()
	{
		AnimationPlayer.Play("up", customSpeed: 2);
		_updateColor = true;	
	}
	
	public override void _Process(double delta)
	{
		CleanupAfterColOrRowShift();
	}

	private void CleanupAfterColOrRowShift()
	{
		if (_updateColor && !AnimationPlayer.IsPlaying() && AnimationPlayer.CurrentAnimation != "idle")		
		{
			SetColor();
			AnimationPlayer.Stop();
			AnimationPlayer.Play("idle");
			_updateColor = false;
			if (IsFakePiece)
			{
				Sprite.Visible = false;
			}
		}
	}
}