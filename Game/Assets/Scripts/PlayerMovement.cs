using Staple;

namespace Platformer;

class PlayerMovement : IComponent
{
    [Min(0)]
    public float movementSpeed = 5;

    [Min(0)]
    public float jumpStrength = 5;
}
