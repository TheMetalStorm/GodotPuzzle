using System;
using Godot;

namespace GodotTest.Scripts;

public enum PieceType
{
	RED, GREEN, BLUE, YELLOW, QUESTION
}
public partial class Piece : Node2D
{
	public double AnimTimer = 0;
	public bool fakePiece = false;
	private bool updateColor = false;
	private static readonly Random Rnd = new Random();
	public PieceType Type;
	public const int Size = 16;
	private AnimationPlayer _animationPlayer;
	public override void _Ready()
	{
		_animationPlayer = GetChild<AnimationPlayer>(1);

		SetColor();
	}

	public void SetColor()
	{
		Sprite2D mySprite = GetChild<Sprite2D>(0);

        
		switch (Type)
		{
			case PieceType.BLUE:
				mySprite.Frame = 49;
				break;
			case PieceType.RED:
				mySprite.Frame = 50;
				break;
			case PieceType.YELLOW:
				mySprite.Frame = 51;
				break;
			case PieceType.GREEN:
				mySprite.Frame = 52;
				break;
			case PieceType.QUESTION:
				mySprite.Frame = 53;
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
		 _animationPlayer.Play("left", customSpeed: 4);
		 updateColor = true;			 
		
	}
	
	public void AnimateRight()
	{
		_animationPlayer.Play("right", customSpeed: 4);
		updateColor = true;			 
		
	}
	
	public override void _Process(double delta)
	{
		if (updateColor && _animationPlayer.CurrentAnimation != "idle" && !_animationPlayer.IsPlaying())
		{
			if (fakePiece)
			{
				QueueFree();
				updateColor = false;
			}
			_animationPlayer.Stop();
			_animationPlayer.Play("idle");
			SetColor();
			updateColor = false;
			

		}
	}

}