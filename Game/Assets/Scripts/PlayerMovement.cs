using Staple;

namespace Platformer;

class PlayerMovement : IComponent
{
    [Min(0)]
    public float movementSpeed = 5;
}
