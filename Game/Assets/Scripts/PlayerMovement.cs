using Staple;
using System.Numerics;

namespace Platformer;

class PlayerMovement : IComponent
{
    [Min(0)]
    public float movementSpeed = 5;

    [Min(0)]
    public float jumpStrength = 5;

    [Min(1)]
    public float turnSpeed = 5;

    public LayerMask collisionMask = LayerMask.Everything;

    internal bool grounded = false;
    internal Quaternion targetRotation = Quaternion.Identity;
}
