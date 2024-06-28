using System.Collections;
using System.Diagnostics;
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
	
	[Export]
	private int _boardSize = 6;

	private CustomSignals _customSignals;	

	public override void _Ready()
	{
		_customSignals = GetNode<CustomSignals>("/root/CustomSignals");
		ZIndex = ZIndexBorder; 
		_pieceScene = GD.Load<PackedScene>("res://Scenes/Piece.tscn");
	
		
		CreateFakePiece();
		_boardPieces = new Piece[_boardSize, _boardSize];
		DrawBoardBg();
		SetupPath2D();
		PopulatePieces();
	}



	private void SetupPath2D()
	{
		var upLeft = Vector2.Zero;
		var upRight = upLeft + Vector2.Right * (_boardSize+1f) * Piece.Size;
		var downLeft = upLeft + Vector2.Down * (_boardSize+1f) * Piece.Size;
		var downRight = upRight + Vector2.Down * (_boardSize+1f) * Piece.Size;

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
		_fake.GetChild<Sprite2D>(0).ZIndex = ZIndexPiece;
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
	
				piece.Position =  new Vector2((x+1)*Piece.Size, (y+1)*Piece.Size);
				_fake.GetChild<Sprite2D>(0).ZIndex = ZIndexPiece;
				piece.SetRandomPiece();
				_boardPieces[x, y] = piece;
				AddChild(piece);
			}
		}
	}
	

	private void DrawBoardBg()
	{

		Array<Vector2I> boardBg = new Array<Vector2I>();
		
		for (int y = 0; y <= _boardSize; y++)
		{
			for (int x = 0; x <= _boardSize; x++)
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
				_fake.Position = new Vector2((_boardSize+1)*Piece.Size, Piece.Size * (row+1));
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
				_fake.Position = new Vector2(0, Piece.Size * (row+1));
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
				_fake.Position = new Vector2((column+1)*Piece.Size, 0);
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
				_fake.Position = new Vector2((column+1)*Piece.Size, (_boardSize+1) * Piece.Size);
				_fake.AnimateUp();
			}

			_boardPieces[column, y].Type = _boardPieces[column, y + 1].Type;
			_boardPieces[column, y].AnimateUp();


		}

		_boardPieces[column, _boardSize - 1].Type = highest;
		_boardPieces[column, _boardSize - 1].AnimateUp();
	}

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
				ShiftRowLeft(1);
			}
		}
		else if (Input.IsActionJustPressed("ui_right"))
		{
			if (CanMakeMove())
			{
				ShiftRowRight(1);
			}
		}
		else if (Input.IsActionJustPressed("ui_down"))
		{
			if (CanMakeMove())
			{
				ShiftColumnDown(2);
			}
		}
		else if (Input.IsActionJustPressed("ui_up"))
		{
			if (CanMakeMove())
			{
				ShiftColumnUp(0);
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