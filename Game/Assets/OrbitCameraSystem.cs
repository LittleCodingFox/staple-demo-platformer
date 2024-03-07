using Staple;
using System.Numerics;

namespace Platformer;

class OrbitCameraSystem : IEntitySystem
{
    public SubsystemType UpdateType => SubsystemType.Update;

    private void UpdateFocusPoint(Transform transform, OrbitCamera camera)
    {
        var target = camera.focus.Position;

        if (camera.focusRadius > 0)
        {
            var distance = Vector3.Distance(target, camera.focusPosition);

            var t = 1.0f;

            if (distance > 0.01f && camera.focusCentering > 0)
            {
                t = Math.Pow(1.0f - camera.focusCentering, Time.deltaTime);
            }

            if (distance > camera.focusRadius)
            {
                t = Math.Min(t, camera.focusRadius / distance);
            }

            camera.focusPosition = Vector3.Lerp(target, camera.focusPosition, t);
        }
        else
        {
            camera.focusPosition = target;
        }
    }

    private bool UpdateRotation(OrbitCamera camera)
    {
        var input = new Vector2(-Input.MouseRelativePosition.Y, -Input.MouseRelativePosition.X);

        var e = 0.001f;

        if(input.X < -e || input.X > e || input.Y < -e || input.Y > e)
        {
            camera.orbitAngles += input * camera.rotationSpeed * Time.deltaTime;

            return true;
        }

        return false;
    }

    private void ConstrainAngles(OrbitCamera camera)
    {
        camera.orbitAngles.X = Math.Clamp(camera.orbitAngles.X, camera.minVerticalAngle, camera.maxVerticalAngle);

        if(camera.orbitAngles.Y < 0)
        {
            camera.orbitAngles.Y += 360;
        }
        else if(camera.orbitAngles.Y >= 360)
        {
            camera.orbitAngles.Y -= 360;
        }
    }

    public void Process(float deltaTime)
    {
        Scene.ForEach((Entity entity, bool enabled, ref Transform transform, ref OrbitCamera camera) =>
        {
            if(enabled == false)
            {
                return;
            }

            if(camera.focus == null)
            {
                camera.focus = Scene.FindEntity("Player").GetComponent<Transform>();

                if (camera.focus == null)
                {
                    return;
                }

                camera.focusPosition = camera.focus.Position;
                transform.LocalRotation = Math.FromEulerAngles(new Vector3(camera.orbitAngles.X, camera.orbitAngles.Y, 0));
            }

            UpdateFocusPoint(transform, camera);

            Quaternion rotation;

            if(UpdateRotation(camera))
            {
                ConstrainAngles(camera);

                rotation = Math.FromEulerAngles(new Vector3(camera.orbitAngles.X, camera.orbitAngles.Y, 0));
            }
            else
            {
                rotation = transform.LocalRotation;
            }

            var direction = Vector3.Transform(new Vector3(0, 0, -1), rotation);

            var position = camera.focusPosition - direction * camera.distance;

            transform.LocalPosition = position;
            transform.LocalRotation = rotation;
        });
    }

    public void Shutdown()
    {
    }

    public void Startup()
    {
        World.AddComponentAddedCallback(typeof(OrbitCamera), (World world, Entity entity, Transform transform, ref IComponent component) =>
        {
            if(component is not OrbitCamera orbitCamera)
            {
                return;
            }
        });
    }
}
