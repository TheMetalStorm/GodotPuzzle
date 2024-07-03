using System;
using Godot;

namespace GodotTest.Scripts;

public enum PieceType
{
	RED, GREEN, BLUE, YELLOW, VIOLET
}
public partial class Piece : Node2D
{
	public double AnimTimer = 0;
	public bool fakePiece = false;
	private bool updateColor = false;
	private static readonly Random Rnd = new Random();
	public PieceType Type;
	public const int Size = 16;
	public AnimationPlayer _animationPlayer;
	public Sprite2D _sprite;
	public override void _Ready()
	{
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_sprite = GetNode<Sprite2D>("Sprite");
		_animationPlayer.CurrentAnimation = "idle";
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
			case PieceType.BLUE:
				_sprite.Frame = 1;
				break;
			case PieceType.RED:
				_sprite.Frame = 2;
				break;
			case PieceType.YELLOW:
				_sprite.Frame = 3;
				break;
			case PieceType.GREEN:
				_sprite.Frame = 4;
				break;
			case PieceType.VIOLET:
				_sprite.Frame = 0;
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
		 _animationPlayer.Play("left", customSpeed: 2);
		 updateColor = true;			 
	}
	
	public void AnimateRight()
	{
		_animationPlayer.Play("right", customSpeed: 2);
		updateColor = true;	
	}
	
	public void AnimateDown()
	{
		_animationPlayer.Play("down", customSpeed: 2);
		updateColor = true;	
	}

	public void AnimateUp()
	{
		_animationPlayer.Play("up", customSpeed: 2);
		updateColor = true;	
	}
	
	public override void _Process(double delta)
	{
		CleanupAfterColOrRowShift();
	}

	private void CleanupAfterColOrRowShift()
	{
		if (updateColor && !_animationPlayer.IsPlaying() && _animationPlayer.CurrentAnimation != "idle")		
		{
			SetColor();
			_animationPlayer.Stop();
			_animationPlayer.Play("idle");
			updateColor = false;
			if (fakePiece)
			{
				_sprite.Visible = false;
			}
		}
	}
}