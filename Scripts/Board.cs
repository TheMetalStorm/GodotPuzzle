using Godot;
using Godot.Collections;

namespace GodotTest.Scripts;

public partial class Board : TileMap
{
	private const int LayerBoard = 0;
	private const int LayerPieces = 1;
	private Piece[,] _boardPieces;
	private Piece _fake;
	private PackedScene pieceScene;
	[Export]
	private int _boardSize = 3;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		pieceScene = GD.Load<PackedScene>("res://Scenes/Piece.tscn");
		CreateFakePiece();


		_boardPieces = new Piece[_boardSize, _boardSize];
		CenterBoard();
		DrawBoardBg();
		PopulatePieces();
	}

	private void CenterBoard()
	{
		Position -= new Vector2((_boardSize+1) * 8, (_boardSize+1) * 8);
	}

	private void CreateFakePiece()
	{
		_fake = pieceScene.Instantiate<Piece>();
		_fake.GetChild<Sprite2D>(0).VisibilityLayer = 1;		
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
				var piece = pieceScene.Instantiate<Piece>();
	
				piece.Position =  new Vector2((x+1)*Piece.Size, (y+1)*Piece.Size);
				piece.GetChild<Sprite2D>(0).VisibilityLayer = LayerPieces;
				piece.SetRandomPiece();
				_boardPieces[x, y] = piece;
				AddChild(piece);
			}
		}
	}
	

	private void DrawBoardBg()
	{
		TileMap map = GetNode<TileMap>(".");
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
				_fake._sprite.Visible = true;
				_fake.Type = _boardPieces[x, row].Type;
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