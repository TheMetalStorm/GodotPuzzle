using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using Godot.Collections;
using GodotTest.Scripts;

public partial class Board : TileMap
{
	private const int LayerBoard = 0;
	private const int LayerPieces = 1;
	private Piece[,] _boardPieces;
	
	[Export]
	private int _boardSize = 6;

	private Piece fake;
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var pieceScene = GD.Load<PackedScene>("res://Scenes/Piece.tscn");
		fake = pieceScene.Instantiate<Piece>();
		fake.GetChild<Sprite2D>(0).VisibilityLayer = 1;		
		fake.fakePiece = true;
		AddChild(fake);
		fake._sprite.Visible = false;

		
		Position -= new Vector2((_boardSize+1) * 8, (_boardSize+1) * 8);
		TileMap map = GetNode<TileMap>(".");
		_boardPieces = new Piece[_boardSize, _boardSize];
        DrawBoardBg(map);
        PopulatePieces();
	}

	private void PopulatePieces()
	{
		var pieceScene = GD.Load<PackedScene>("res://Scenes/Piece.tscn");
		for (int y = 0; y < _boardSize; y++)
		{
			for (int x = 0; x < _boardSize ; x++)
			{
				var piece = pieceScene.Instantiate<Piece>();
	
				piece.Position =  new Vector2((x+1)*Piece.Size, (y+1)*Piece.Size);
				piece.GetChild<Sprite2D>(0).VisibilityLayer = LayerPieces;
				piece.SetRandomPiece();
				_boardPieces[x, y] = piece;
				AddChild(piece);
			}
		}
	}
	

	private void DrawBoardBg(TileMap map)
	{
		Array<Vector2I> boardBg = new Array<Vector2I>();
		
		for (int y = 0; y <= _boardSize; y++)
		{
			for (int x = 0; x <= _boardSize; x++)
			{
				boardBg.Add(new Vector2I(x, y));		
			}
		}
		
		map.SetCellsTerrainConnect(LayerBoard, boardBg,0, 0);
		
	}
	
	private void MoveLineLeft(int line)
	{

		// for (int x = 0; x < _boardSize-1; x++)
		// {
		// 	(_boardPieces[x, line].Type, _boardPieces[x+1, line].Type) = (_boardPieces[x+1, line].Type, _boardPieces[x, line].Type);
		//
		// 	//_boardPieces[x, line].AnimateLeft();
		// 	
		// 	_boardPieces[x, line].SetColor();
		// 	_boardPieces[x+1, line].SetColor();
		// 	
		// }
	}
	
	private void MoveLineRight(int line)
	{

		PieceType last = _boardPieces[_boardSize - 1, line].Type;
		for (int x = _boardSize - 1; x > 0; x--)
		{

			if (x == _boardSize - 1)
			{
				fake._sprite.Visible = true;
				fake.Type = _boardPieces[x, line].Type;
				fake.SetColor();
				fake.Position = new Vector2(0, Piece.Size * (line+1));
				fake.AnimateRight();
			} 
			_boardPieces[x, line].Type = _boardPieces[x-1, line].Type;
			_boardPieces[x, line].AnimateRight();


		}
		_boardPieces[0, line].Type = last;
		_boardPieces[0, line].AnimateRight();//AnimateRight();

	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	
		if (Input.IsActionJustPressed("ui_left"))
		{
			if (canMakeMove())
			{
				//MoveLineRight(1);
				MoveLineLeft(0);
			}
			
		}
		else if (Input.IsActionJustPressed("ui_right"))
		{
			if (canMakeMove())
			{
				MoveLineRight(0);
			}
		}
	
	}

	private bool canMakeMove()
	{
		foreach (Piece piece in _boardPieces)
		{
			if (piece._animationPlayer.IsPlaying())
			{
				return false;
			}
		}
	
		return true;
	}
}
