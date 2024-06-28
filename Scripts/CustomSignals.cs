using Godot;
using Godot.Collections;

namespace GodotTest.Scripts;

public partial class CustomSignals : Node
{
    [Signal]
    public delegate void SetupPath2DPointsEventHandler(Array<Vector2> input);
    public delegate void SetupCameraEventHandler(Vector2 cameraOffset);

}