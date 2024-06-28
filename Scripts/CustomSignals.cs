using Godot;
using Godot.Collections;

namespace GodotTest.Scripts;

public partial class CustomSignals : Node
{
    [Signal]
    public delegate void SetupPath2DPointsEventHandler(Array<Vector2> input);
    [Signal]
    public delegate void ShiftLineEventHandler(Vector2 lilGuiPoss);
    [Signal]
    public delegate void ShiftAnimEndedEventHandler();
    public delegate void SetupCameraEventHandler(Vector2 cameraOffset);

}