using System;
using Godot;

namespace GodotTest.Scenes;
public enum PieceType
{
	RED, GREEN, BLUE, YELLOW, QUESTION
}
public partial class Piece : Node2D
{
	private static readonly Random Rnd = new Random();
	public PieceType Type;
	public static int size = 16;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//assume square pieces
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


	
}