using Godot;
using Godot.Collections;

namespace GodotTest.Scripts;

public partial class LilGuy : Path2D
{
	private const int ZIndexLine = 3;

	private CustomSignals _customSignals;
	private bool _gotPoints = false;
	private PathFollow2D _traveller;
	private AnimationPlayer _animationPlayer;
	private Sprite2D _sprite2D;
	private bool _canMove = true;
	[Export]
	private float _speed = 20;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ZIndex = ZIndexLine;
		_traveller = GetNode<PathFollow2D>("BorderPath");
		_sprite2D = GetNode<Sprite2D>("BorderPath/Sprite");
		_animationPlayer = GetNode<AnimationPlayer>("BorderPath/AnimationPlayer");
		_animationPlayer.Play("walk");
		_customSignals = GetNode<CustomSignals>("/root/CustomSignals");
		_customSignals.SetupPath2DPoints += OnPointsReceived;
		_customSignals.ShiftAllowed += OnShiftAllowed;
		_customSignals.ShiftAnimEnded += OnShiftAnimEnd;

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(!_gotPoints) return;
		if(!_canMove) return;
		if (Input.IsActionJustPressed("ui_accept"))
		{
			_speed = -_speed;
			_sprite2D.FlipH = !_sprite2D.FlipH;
		}

		_traveller.Progress += (float)delta * _speed;

		if (Input.IsActionJustPressed("ui_down"))
		{
			
			_customSignals.EmitSignal(nameof(CustomSignals.ShiftLine), _traveller.Position);
		}

	}

	private void OnShiftAllowed()
	{
		_canMove = false;
		_animationPlayer.Play("push");
	}
	
	
	// public override void _Draw()
	// {
		// if(!_gotPoints) return;
		// DrawPolyline( Curve.GetBakedPoints(), Colors.Aquamarine, 1);
	// }

	private void OnPointsReceived(Array<Vector2> input)
	{
		_gotPoints = true;
		foreach (var v2 in input)
		{
			Curve.AddPoint(v2);	
		}
		
	}
	
	private void OnShiftAnimEnd()
	{
		_canMove = true;
		_animationPlayer.Stop();
		_animationPlayer.Play("walk");
		
	}
}