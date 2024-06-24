using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using Godot.Collections;
using GodotTest.Scenes;

public partial class Board : TileMap
{
	private const int LayerBoard = 0;
	private const int LayerPieces = 1;
	private Piece[,] _boardPieces;
	
	double timeAcc = 0;
	private bool didThing = false;
	[Export]
	private int _boardSize = 4;

	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
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
	
				piece.Position =  new Vector2((x+1)*Piece.size, (y+1)*Piece.size);
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

	private void RerenderPieces()
	{
		foreach (var child in GetChildren())
		{
			RemoveChild(child);
		}
		
		for (int y = 0; y < _boardSize; y++)
		{
			for (int x = 0; x < _boardSize ; x++)
			{
				_boardPieces[x, y].Position =  new Vector2((x+1)*Piece.size, (y+1)*Piece.size);
				AddChild(_boardPieces[x, y]);
			}
		}
	}
	
	private void MoveLineLeft(int line)
	{

		for (int x = 0; x < _boardSize-1; x++)
		{
			(_boardPieces[x, line].Type, _boardPieces[x+1, line].Type) = (_boardPieces[x+1, line].Type, _boardPieces[x, line].Type);
			_boardPieces[x, line].SetColor();
			_boardPieces[x+1, line].SetColor();
		}
	}
	
	private void MoveLineRight(int line)
	{

		PieceType last = _boardPieces[_boardSize - 1, line].Type;
		for (int x = _boardSize - 1; x >= 1; x--)
		{
			_boardPieces[x, line].Type = _boardPieces[x-1, line].Type;//
			_boardPieces[x, line].SetColor();
		}

		_boardPieces[0, line].Type = last;
		_boardPieces[0, line].SetColor();

	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("ui_left"))
		{
			MoveLineLeft(0);
		}
		
		if (Input.IsActionJustPressed("ui_right"))
		{
			MoveLineRight(0);
		}
	}
}
