using Staple;
using System.Numerics;

namespace Platformer;

class OrbitCamera : IComponent
{
    public float distance = 5.0f;
    public float focusRadius = 1.0f;
    public float focusCentering = 0.5f;
    public float rotationSpeed = 90;
    public float minVerticalAngle = -90;
    public float maxVerticalAngle = 60;

    internal Transform focus;

    internal Vector3 focusPosition;
    internal Vector2 orbitAngles = new Vector2(45, 0);
}
