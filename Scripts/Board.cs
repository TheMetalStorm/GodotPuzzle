using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using Godot;
using Godot.Collections;

namespace GodotTest.Scripts;

enum BoardState
{
	Shifting, PlayerControl
}
public partial class Board : TileMap
{
	private const int LayerIndexBorder = 0;
	private const int ZIndexPiece = 0;
	private const int ZIndexBorder = 1;

	private Piece[,] _boardPieces;
	private Piece _fake;
	private Piece _fake2;
	private PackedScene _pieceScene;
	private BoardState _boardState = BoardState.PlayerControl;

	private CustomSignals _customSignals;	

	[Export]
	private int _boardSize = 6;


	public override void _Ready()
	{
		_customSignals = GetNode<CustomSignals>("/root/CustomSignals");
		ZIndex = ZIndexBorder; 
		_pieceScene = GD.Load<PackedScene>("res://Scenes/Piece.tscn");
		_customSignals.ShiftLine += OnShiftLine;

		
		CreateFakePieces();
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

		var path = new Array<Vector2>
		{
			upLeft,
			upRight,
			downRight,
			downLeft,
			upLeft
		};

		_customSignals.EmitSignal(nameof(CustomSignals.SetupPath2DPoints), path);

	}

	private void CreateFakePieces()
	{
		_fake = _pieceScene.Instantiate<Piece>();
		_fake.GetNode<Sprite2D>("Sprite").ZIndex = ZIndexPiece;
		_fake.IsFakePiece = true;
		AddChild(_fake);
		_fake.Sprite.Visible = false;

		_fake2 = _pieceScene.Instantiate<Piece>();
		_fake2.GetNode<Sprite2D>("Sprite").ZIndex = ZIndexPiece;
		_fake2.IsFakePiece = true;
		AddChild(_fake2);
		_fake2.Sprite.Visible = false;
	}

	private void PopulatePieces()
	{
		for (var y = 0; y < _boardSize; y++)
		{
			for (var x = 0; x < _boardSize ; x++)
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
		var boardBg = new Array<Vector2I>();

		for (var y = -1; y <= _boardSize; y++)
		{
			for (var x = -1; x <= _boardSize; x++)
			{
				boardBg.Add(new Vector2I(x, y));		
				EraseCell(LayerIndexBorder,new Vector2I(x, y));
			}
		}
		
		SetCellsTerrainConnect(LayerIndexBorder, boardBg,0, 0);
		
	}
	
	private void ShiftRowLeft(int row)
	{
		var first = _boardPieces[0, row].Type;
		var second = _boardPieces[1, row].Type;

		for (int x = 0; x < _boardSize-1; x++)
		{
			if (x == 0)
			{
				PrepareFakePieceForAnimation(_fake, first, new Vector2(_boardSize*Piece.Size, Piece.Size * row));
				_fake.AnimateLeft();
				PrepareFakePieceForAnimation(_fake2, second, new Vector2((_boardSize+1)*Piece.Size, Piece.Size * row));
				_fake2.AnimateLeft();
			} 
			_boardPieces[x, row].Type = _boardPieces[x+1, row].Type;
			_boardPieces[x, row].AnimateLeft();


		}
		_boardPieces[_boardSize-1, row].Type = first;
		_boardPieces[_boardSize-1, row].AnimateLeft();
	}
	
	private void ShiftRowRight(int row)
	{
		var last = _boardPieces[_boardSize - 1, row].Type;
		var beforeLast = _boardPieces[_boardSize - 2, row].Type;

		for (var x = _boardSize - 1; x > 0; x--)
		{

			if (x == _boardSize - 1)
			{
				PrepareFakePieceForAnimation(_fake, last, new Vector2(-1 * Piece.Size, Piece.Size * row));
				_fake.AnimateRight();
				PrepareFakePieceForAnimation(_fake2, beforeLast, new Vector2(-2 * Piece.Size, Piece.Size * row));
				_fake2.AnimateRight();
			} 
			_boardPieces[x, row].Type = _boardPieces[x-1, row].Type;
			_boardPieces[x, row].AnimateRight();


		}
		_boardPieces[0, row].Type = last;
		_boardPieces[0, row].AnimateRight();
	}

	private void ShiftColumnUp(int column)
	{
		var highest = _boardPieces[column, 0].Type;
		
		for (var y = 0; y < _boardSize - 1; y++)
		{
			if (y == 0)
			{
				PrepareFakePieceForAnimation(_fake, _boardPieces[column, y].Type, new Vector2(column * Piece.Size, _boardSize * Piece.Size));
				_fake.AnimateUp();
				PrepareFakePieceForAnimation(_fake2, _boardPieces[column, y+1].Type, new Vector2(column * Piece.Size, (_boardSize+1) * Piece.Size));
				_fake2.AnimateUp();
			}
			_boardPieces[column, y].Type = _boardPieces[column, y + 1].Type;
			_boardPieces[column, y].AnimateUp();
			
		}
		_boardPieces[column, _boardSize - 1].Type = highest;
		_boardPieces[column, _boardSize - 1].AnimateUp();
	}

	private void ShiftColumnDown(int column)
	{
		var lowest = _boardPieces[column, _boardSize-1].Type;
		var secondLowest = _boardPieces[column, _boardSize-2].Type;//only needed because anim overshoots 

		for (var y = _boardSize-1; y > 0 ; y--)
		{
			if (y == _boardSize-1)
			{
				PrepareFakePieceForAnimation(_fake, lowest, new Vector2(column * Piece.Size, -1 * Piece.Size));
				_fake.AnimateDown();
				PrepareFakePieceForAnimation(_fake2, secondLowest, new Vector2(column * Piece.Size, -2 * Piece.Size));
				_fake2.AnimateDown();
			} 
			_boardPieces[column, y].Type = _boardPieces[column, y-1 ].Type;
			_boardPieces[column, y].AnimateDown();
		}
		_boardPieces[column, 0].Type = lowest;
		_boardPieces[column, 0].AnimateDown();
	}
	
	private static void PrepareFakePieceForAnimation(Piece fakePiece, PieceType newType, Vector2 newPos)
	{
		fakePiece.Sprite.Visible = true;
		fakePiece.UpdateType(newType);
		fakePiece.Position = newPos;
	}

	private void OnShiftLine(Vector2 lilGuyPos)
	{
		var lilGuyMapPos = LocalToMap(lilGuyPos);
		if (!IsValidMovePos(lilGuyMapPos)) return;
		_customSignals.EmitSignal(nameof(CustomSignals.ShiftAllowed));
		if (_boardState == BoardState.PlayerControl)
		{
			_boardState = BoardState.Shifting;

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
		if (_boardState == BoardState.Shifting)
		{
			if (AllPiecesIdle())
			{
				_boardState = BoardState.PlayerControl;
				_customSignals.EmitSignal(nameof(CustomSignals.ShiftAnimEnded));
			}
		}
	}
	private bool AllPiecesIdle()
	{
		return _boardPieces.Cast<Piece>().All(piece => piece.AnimationPlayer.CurrentAnimation == "idle");
	}
}