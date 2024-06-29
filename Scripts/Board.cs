using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using Godot;
using Godot.Collections;

namespace GodotTest.Scripts;

public partial class Board : TileMap
{
	private const int LayerIndexBorder = 0;
	private const int ZIndexPiece = 0;
	private const int ZIndexBorder = 1;

	private Piece[,] _boardPieces;
	private Piece _fake;
	private PackedScene _pieceScene;
	private bool _checkForEndOfMove;
	
	[Export]
	private int _boardSize = 6;

	private CustomSignals _customSignals;	

	public override void _Ready()
	{
		_customSignals = GetNode<CustomSignals>("/root/CustomSignals");
		ZIndex = ZIndexBorder; 
		_pieceScene = GD.Load<PackedScene>("res://Scenes/Piece.tscn");
		_customSignals.ShiftLine += OnShiftLine;

		
		CreateFakePiece();
		_boardPieces = new Piece[_boardSize, _boardSize];
		DrawBoardBg();
		SetupPath2D();
		PopulatePieces();
	}



	private void SetupPath2D()
	{
		var upLeft = new Vector2(-Piece.Size / 2f  , -Piece.Size / 2f);
		var upRight = upLeft + Vector2.Right * (_boardSize+1) * Piece.Size;
		var downLeft = upLeft + Vector2.Down * (_boardSize+1) * Piece.Size;
		var downRight = upRight + Vector2.Down * (_boardSize+1) * Piece.Size;

		Array<Vector2> path = new Array<Vector2>
		{
			upLeft,
			upRight,
			downRight,
			downLeft,
			upLeft
		};

		_customSignals.EmitSignal(nameof(CustomSignals.SetupPath2DPoints), path);

	}

	private void CreateFakePiece()
	{
		_fake = _pieceScene.Instantiate<Piece>();
		_fake.GetNode<Sprite2D>("Sprite").ZIndex = ZIndexPiece;
		_fake.fakePiece = true;
		AddChild(_fake);
		_fake._sprite.Visible = false;
	}

	private void PopulatePieces()
	{
		for (int y = 0; y < _boardSize; y++)
		{
			for (int x = 0; x < _boardSize ; x++)
			{
				var piece = _pieceScene.Instantiate<Piece>();
	
				piece.Position =  new Vector2(x * Piece.Size, Position.Y + y * Piece.Size);
				piece.SetRandomPiece();
				piece.GetChild<Sprite2D>(0).ZIndex = ZIndexPiece;
				_boardPieces[x, y] = piece;
				AddChild(piece);

			}
		}
	}
	

	private void DrawBoardBg()
	{

		Array<Vector2I> boardBg = new Array<Vector2I>();
		
		for (int y = -1; y <= _boardSize; y++)
		{
			for (int x = -1; x <= _boardSize; x++)
			{
				boardBg.Add(new Vector2I(x, y));		
				EraseCell(LayerIndexBorder,new Vector2I(x, y));
			}
		}
		
		SetCellsTerrainConnect(LayerIndexBorder, boardBg,0, 0);
		
	}
	
	private void ShiftRowLeft(int row)
	{
		PieceType first = _boardPieces[0, row].Type;
		for (int x = 0; x < _boardSize-1; x++)
		{
			if (x == 0)
			{
				_fake._sprite.Visible = true;
				_fake.Type = _boardPieces[x, row].Type;
				_fake.SetColor();
				_fake.Position = new Vector2(_boardSize*Piece.Size, Piece.Size * row);
				_fake.AnimateLeft();
			} 
			_boardPieces[x, row].Type = _boardPieces[x+1, row].Type;
			_boardPieces[x, row].AnimateLeft();


		}
		_boardPieces[_boardSize-1, row].Type = first;
		_boardPieces[_boardSize-1, row].AnimateLeft();
	}
	
	private void ShiftRowRight(int row)
	{
		PieceType last = _boardPieces[_boardSize - 1, row].Type;
		for (int x = _boardSize - 1; x > 0; x--)
		{

			if (x == _boardSize - 1)
			{
				_fake._sprite.Visible = true;
				_fake.Type = last;
				_fake.SetColor();
				_fake.Position = new Vector2(-1 * Piece.Size, Piece.Size * row);
				_fake.AnimateRight();
			} 
			_boardPieces[x, row].Type = _boardPieces[x-1, row].Type;
			_boardPieces[x, row].AnimateRight();


		}
		_boardPieces[0, row].Type = last;
		_boardPieces[0, row].AnimateRight();
	}
	
	private void ShiftColumnDown(int column)
	{
		PieceType lowest = _boardPieces[column, _boardSize-1].Type;
		for (int y = _boardSize-1; y > 0 ; y--)
		{
			if (y == _boardSize-1)
			{
				_fake._sprite.Visible = true;
				_fake.Type = lowest;
				_fake.SetColor();
				_fake.Position = new Vector2(column * Piece.Size, -1 * Piece.Size);
				_fake.AnimateDown();
			} 
			
			_boardPieces[column, y].Type = _boardPieces[column, y-1 ].Type;
			_boardPieces[column, y].AnimateDown();
		
		
		}
		_boardPieces[column, 0].Type = lowest;
		_boardPieces[column, 0].AnimateDown();
	}

	private void ShiftColumnUp(int column)
	{
		PieceType highest = _boardPieces[column, 0].Type;
		for (int y = 0; y < _boardSize - 1; y++)
		{
			if (y == 0)
			{
				_fake._sprite.Visible = true;
				_fake.Type = _boardPieces[column, y].Type;
				_fake.SetColor();
				_fake.Position = new Vector2(column * Piece.Size, _boardSize * Piece.Size);
				_fake.AnimateUp();
			}

			_boardPieces[column, y].Type = _boardPieces[column, y + 1].Type;
			_boardPieces[column, y].AnimateUp();


		}

		_boardPieces[column, _boardSize - 1].Type = highest;
		_boardPieces[column, _boardSize - 1].AnimateUp();
	}


	private void OnShiftLine(Vector2 lilGuyPos)
	{
		var lilGuyMapPos = LocalToMap(lilGuyPos);
		if (!IsValidMovePos(lilGuyMapPos)) return;
		_customSignals.EmitSignal(nameof(CustomSignals.ShiftAllowed));
		if (AllPiecesIdle())
		{
			if (lilGuyMapPos.X == -1)
			{
				ShiftRowRight(lilGuyMapPos.Y);
			}
			else if (lilGuyMapPos.X == _boardSize)
			{
				ShiftRowLeft(lilGuyMapPos.Y);
			}
			else if (lilGuyMapPos.Y == -1)
			{
				ShiftColumnDown(lilGuyMapPos.X);
			}
			else if (lilGuyMapPos.Y == _boardSize)
			{
				ShiftColumnUp(lilGuyMapPos.X);
			}
			_checkForEndOfMove = true;
		}
	}

	private bool IsValidMovePos(Vector2I lilGuyMapPos)
	{
		return lilGuyMapPos != new Vector2I(-1,-1) &&
		       lilGuyMapPos != new Vector2I(-1,_boardSize) &&
		       lilGuyMapPos != new Vector2I(_boardSize,_boardSize) &&
		       lilGuyMapPos != new Vector2I(_boardSize,-1);
	}

	public override void _Process(double delta)
	{
		if (_checkForEndOfMove)
		{
			if (AllPiecesIdle())
			{
				_checkForEndOfMove = false;
				_customSignals.EmitSignal(nameof(CustomSignals.ShiftAnimEnded));
			}
		}
	}
	private bool AllPiecesIdle()
	{
		return _boardPieces.Cast<Piece>().All(piece => piece._animationPlayer.CurrentAnimation == "idle");
	}
}