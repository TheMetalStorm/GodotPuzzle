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
		
		//TODO: Rand des Boards muss Layer höher sein als Pieces, inneres des Boards niedriger
		//----  - und | = 2
		//|PB|	P = 1
		//|BB|	B = 0
		//----	
		
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
	
	private void MoveRowLeft(int row)
	{
		//TODO: implement
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
	private void MoveColumnDown(int column)
	{
		//TODO: implement
	}
	
	private void MoveColumnUp(int column)
	{
		//TODO: implement
	}
	
	private void MoveRowRight(int row)
	{

		PieceType last = _boardPieces[_boardSize - 1, row].Type;
		for (int x = _boardSize - 1; x > 0; x--)
		{

			if (x == _boardSize - 1)
			{
				fake._sprite.Visible = true;
				fake.Type = _boardPieces[x, row].Type;
				fake.SetColor();
				fake.Position = new Vector2(0, Piece.Size * (row+1));
				fake.AnimateRight();
			} 
			_boardPieces[x, row].Type = _boardPieces[x-1, row].Type;
			_boardPieces[x, row].AnimateRight();


		}
		_boardPieces[0, row].Type = last;
		_boardPieces[0, row].AnimateRight();//AnimateRight();

	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//TODO: replace with real movement
		TempMovePieces();
	}

	private void TempMovePieces()
	{
		if (Input.IsActionJustPressed("ui_left"))
		{
			if (CanMakeMove())
			{ 
				MoveRowLeft(0);
			}
		}
		else if (Input.IsActionJustPressed("ui_right"))
		{
			if (CanMakeMove())
			{
				MoveRowRight(0);
			}
		}
		else if (Input.IsActionJustPressed("ui_down"))
		{
			if (CanMakeMove())
			{
				MoveColumnDown(0);
			}
		}
		else if (Input.IsActionJustPressed("ui_up"))
		{
			if (CanMakeMove())
			{
				MoveColumnUp(0);
			}
		}
	}

	private bool CanMakeMove()
	{
		foreach (Piece piece in _boardPieces)
		{
			if (piece._animationPlayer.CurrentAnimation != "idle")
			{
				return false;
			}
		}
		return true;
	}
}
