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

            var direction = (forward * movement.Y + right * movement.X);

            var velocity = direction * playerMovement.movementSpeed;

            var yVelocity = rigidBody.Velocity.Y;

            playerMovement.grounded = Physics.RayCast3D(new Ray(rigidBody.Position + new Vector3(0, -1, 0), new Vector3(0, -1, 0)), out var hitBody, out _,
                playerMovement.collisionMask);

            if (spacePress && playerMovement.grounded)
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
