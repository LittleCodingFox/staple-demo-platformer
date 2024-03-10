using Staple;
using System.Numerics;

namespace Platformer;

class PlayerMovementSystem : IEntitySystem
{
    public EntitySubsystemType UpdateType => EntitySubsystemType.Both;

    private bool leftPress = false;
    private bool rightPress = false;
    private bool upPress = false;
    private bool downPress = false;
    private bool spacePress = false;

    public void FixedUpdate(float deltaTime)
    {
        Scene.ForEach((Entity entity, ref Transform transform, ref OrbitCamera camera) =>
        {
            if (camera.focus == null ||
                camera.focus.entity.TryGetComponent<PlayerMovement>(out var playerMovement) == false)
            {
                return;
            }

            var rigidBody = Physics.GetBody3D(camera.focus.entity);

            if (rigidBody == null)
            {
                return;
            }

            var forward = transform.Forward;

            forward.Y = 0.0f;

            forward = Vector3.Normalize(forward);

            var right = transform.Right;

            right.Y = 0.0f;

            right = Vector3.Normalize(right);

            var movement = Vector2.Zero;

            if (upPress)
            {
                movement.Y = 1;
            }

            if (downPress)
            {
                movement.Y = -1;
            }

            if (leftPress)
            {
                movement.X = -1;
            }

            if (rightPress)
            {
                movement.X = 1;
            }

            var velocity = (forward * movement.Y + right * movement.X) * playerMovement.movementSpeed;

            var yVelocity = rigidBody.Velocity.Y;

            if (spacePress)
            {
                yVelocity = playerMovement.jumpStrength;
            }

            velocity.Y = yVelocity;

            if (velocity.X != float.NaN && velocity.Y != float.NaN && velocity.Z != float.NaN)
            {
                rigidBody.Velocity = velocity;
            }
        });

        leftPress = rightPress = upPress = downPress = spacePress = false;
    }

    public void Update(float deltaTime)
    {
        leftPress |= Input.GetKey(KeyCode.A);
        rightPress |= Input.GetKey(KeyCode.D);
        upPress |= Input.GetKey(KeyCode.W);
        downPress |= Input.GetKey(KeyCode.S);
        spacePress |= Input.GetKeyDown(KeyCode.Space);
    }

    public void Shutdown()
    {
    }

    public void Startup()
    {
    }
}
