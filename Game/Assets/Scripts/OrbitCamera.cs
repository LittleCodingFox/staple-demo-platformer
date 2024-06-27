using Staple;
using System.Numerics;

namespace Platformer;

class OrbitCamera : IComponent
{
    [Range(1.0f, 20.0f)]
    public float distance = 5.0f;

    [Min(0)]
    public float focusRadius = 1.0f;

    [Range(0.0f, 1.0f)]
    public float focusCentering = 0.5f;

    [Range(1.0f, 360.0f)]
    public float rotationSpeed = 90;

    [Range(-89.0f, 89.0f)]
    public float minVerticalAngle = -30;

    [Range(-89.0f, 89.0f)]
    public float maxVerticalAngle = 60;

    [Min(0)]
    public float alignDelay = 5.0f;

    [Range(0, 90)]
    public float alignSmoothRange = 45.0f;

    public Transform focus;

    internal Vector3 focusPoint;
    internal Vector3 previousFocusPoint;
    internal Vector2 orbitAngles = new Vector2(-45, 0);
    internal float lastManualRotationTime;
    internal bool firstFrame = true;
}
