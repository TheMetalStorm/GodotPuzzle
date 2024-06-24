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
	
	[Export]
	private int _boardSize = 5;

	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Position = Position - new Vector2(_boardSize * 8, _boardSize * 8);
		TileMap map = GetNode<TileMap>(".");
		_boardPieces = new Piece[_boardSize, _boardSize];
		//TODO: figure out where middle of scene is
        DrawBoardBg(map);
        PopulatePieces();
        
	}

	// private void DrawPieces(TileMap map)
	// {
	// 	for (int i = 0; i < _boardSize; i++)
	// 	{
	// 		for (int j = 0; j < _boardSize; j++)
	// 		{
	// 			_boardPieces[i, j].Draw();
	// 		}
	// 	}
	// }

	private void PopulatePieces()
	{
		var pieceScene = GD.Load<PackedScene>("res://Scenes/Piece.tscn");

		
			for (int y = 0; y < _boardSize; y++)
			{
				for (int x = 0; x < _boardSize ; x++)
				{
				var piece = pieceScene.Instantiate<Piece>();
				piece.Position =  new Vector2(x*16 + 8, y*16 + 8);
				piece.SetRandomPiece();
				AddChild(piece);
				GD.Print(piece.Type.ToString());
			}
		}
	}


	private void DrawBoardBg(TileMap map)
	{
		Array<Vector2I> boardBg = new Array<Vector2I>();
		
		int moveToMiddle = _boardSize * 16 / 2;

		for (int y = 0; y < _boardSize; y++)
		{
			for (int x = 0; x < _boardSize; x++)
			{
				boardBg.Add(new Vector2I(x, y));		
			}
		}
		
		map.SetCellsTerrainConnect(LayerBoard, boardBg,0, 0);

	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
