using Staple;
using System.Numerics;

namespace Platformer;

class PlayerMovementSystem : IEntitySystem
{
    public SubsystemType UpdateType => SubsystemType.Update;

    public void Process(float deltaTime)
    {
        Scene.ForEach((Entity entity, ref Transform transform, ref OrbitCamera camera) =>
        {
            if(camera.focus == null ||
                camera.focus.entity.TryGetComponent<PlayerMovement>(out var playerMovement) == false)
            {
                return;
            }

            var rigidBody = Physics.GetBody3D(camera.focus.entity);

            if(rigidBody == null)
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

            if (Input.GetKey(KeyCode.W))
            {
                movement.Y = 1;
            }

            if (Input.GetKey(KeyCode.S))
            {
                movement.Y = -1;
            }

            if (Input.GetKey(KeyCode.A))
            {
                movement.X = -1;
            }

            if (Input.GetKey(KeyCode.D))
            {
                movement.X = 1;
            }

            var velocity = (forward * movement.Y + right * movement.X);

            if (velocity.X != float.NaN && velocity.Y != float.NaN && velocity.Z != float.NaN)
            {
                rigidBody.Velocity = velocity * playerMovement.movementSpeed;
            }
        });
    }

    public void Shutdown()
    {
    }

    public void Startup()
    {
    }
}
