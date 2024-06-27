using Staple;
using System.Numerics;

namespace Platformer;

class PlayerMovementSystem : IEntitySystemUpdate, IEntitySystemFixedUpdate
{
    private Vector2 movement;
    private bool jumpPress = false;

    public void FixedUpdate(float deltaTime)
    {
        Scene.ForEach((Entity entity, ref Transform transform, ref OrbitCamera camera) =>
        {
            if (camera.focus == null ||
                camera.focus.entity.TryGetComponent<PlayerMovement>(out var playerMovement) == false ||
                (camera.focus.GetChild(0)?.entity.TryGetComponent<SkinnedMeshAnimator>(out var animator) ?? false) == false)
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

            if(forward != Vector3.Zero)
            {
                forward = Vector3.Normalize(forward);
            }

            var right = transform.Right;

            right.Y = 0.0f;

            if(right != Vector3.Zero)
            {
                right = Vector3.Normalize(right);
            }

            var direction = (forward * movement.Y + right * movement.X);

            var velocity = direction * playerMovement.movementSpeed;

            var yVelocity = rigidBody.Velocity.Y;

            playerMovement.grounded = Physics.RayCast3D(new Ray(rigidBody.Position, new Vector3(0, -3.0f, 0)), out var hitBody, out _,
                playerMovement.collisionMask);

            if (jumpPress && playerMovement.grounded)
            {
                yVelocity = playerMovement.jumpStrength;
            }

            velocity.Y = yVelocity;

            rigidBody.Velocity = velocity;

            if(movement != Vector2.Zero)
            {
                var rotation = Math.LookAt(Vector3.Normalize(direction), new Vector3(0, 1, 0));

                playerMovement.targetRotation = rotation;
            }

            if(rigidBody.Rotation != playerMovement.targetRotation)
            {
                rigidBody.Rotation = Quaternion.Slerp(rigidBody.Rotation, playerMovement.targetRotation, Time.fixedDeltaTime * playerMovement.turnSpeed);
            }

            animator.animationController?.SetBoolParameter("Movement", movement != Vector2.Zero);
            animator.animationController?.SetBoolParameter("Jump", playerMovement.grounded == false);
        });
    }

    public void Update(float deltaTime)
    {
        movement = Vector2.Zero;
        jumpPress = false;

        if (Input.GetGamepadCount() > 0)
        {
            movement = Input.GetGamepadLeftAxis(0);
            jumpPress |= Input.GetGamepadButton(0, GamepadButton.A);
        }
        else
        {
            movement.X = Input.GetKey(KeyCode.A) ? -1 : Input.GetKey(KeyCode.D) ? 1 : 0;
            movement.Y = Input.GetKey(KeyCode.W) ? 1 : Input.GetKey(KeyCode.S) ? -1 : 0;
            jumpPress |= Input.GetKey(KeyCode.Space);
        }
    }

    public void Shutdown()
    {
    }

    public void Startup()
    {
    }
}
