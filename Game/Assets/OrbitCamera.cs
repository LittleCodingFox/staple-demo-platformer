using Staple;
using System.Numerics;

namespace Platformer;

class OrbitCamera : IComponent
{
    public float distance = 5.0f;
    public float focusRadius = 1.0f;
    public float focusCentering = 0.5f;

    internal Transform focus;

    internal Vector3 focusPosition;
}
