using Staple;
using System.Numerics;

namespace Platformer;

//Based on https://catlikecoding.com/unity/tutorials/movement/orbit-camera/
class OrbitCameraSystem : IEntitySystemUpdate
{
    private void UpdateFocusPoint(OrbitCamera camera)
    {
        camera.previousFocusPoint = camera.focusPoint;

        var target = camera.focus.Position;

        if (camera.focusRadius > 0)
        {
            var distance = Vector3.Distance(target, camera.focusPoint);

            var t = 1.0f;

            if (distance > 0.01f && camera.focusCentering > 0)
            {
                t = Math.Pow(1.0f - camera.focusCentering, Time.deltaTime);
            }

            if (distance > camera.focusRadius)
            {
                t = Math.Min(t, camera.focusRadius / distance);
            }

            camera.focusPoint = Vector3.Lerp(target, camera.focusPoint, t);
        }
        else
        {
            camera.focusPoint = target;
        }
    }

    private bool ManualRotation(OrbitCamera camera)
    {
        var input = new Vector2(Input.MouseRelativePosition.Y, Input.MouseRelativePosition.X);

        if(Input.GetGamepadCount() > 0)
        {
            input = Input.GetGamepadRightAxis(0);

            (input.X, input.Y) = (-input.Y, -input.X);
        }

        var e = 0.001f;

        if(input.X < -e || input.X > e || input.Y < -e || input.Y > e)
        {
            camera.orbitAngles += input * camera.rotationSpeed * Time.deltaTime;
            camera.lastManualRotationTime = Time.time;

            return true;
        }

        return false;
    }

    private bool AutomaticRotation(OrbitCamera camera)
    {
        if (Time.time - camera.lastManualRotationTime < camera.alignDelay)
        {
            return false;
        }

        var movement = new Vector2(camera.focusPoint.X - camera.previousFocusPoint.X,
            camera.focusPoint.Z - camera.previousFocusPoint.Z);

        var deltaSqr = movement.LengthSquared();

        if(deltaSqr < 0.0001f)
        {
            return false;
        }

        var headingAngle = GetAngle(movement / Math.Sqrt(deltaSqr));

        var deltaAbs = Math.Abs(Math.Min(camera.orbitAngles.Y, headingAngle));
        var rotationChange = camera.rotationSpeed * Math.Min(Time.deltaTime, deltaSqr);

        if(deltaAbs < camera.alignSmoothRange)
        {
            rotationChange *= deltaAbs / camera.alignSmoothRange;
        }
        else if(180.0f - deltaAbs < camera.alignSmoothRange)
        {
            rotationChange *= (180.0f - deltaAbs) / camera.alignSmoothRange;
        }

        camera.orbitAngles.Y = Math.MoveTowards(camera.orbitAngles.Y, headingAngle, rotationChange);

        return true;
    }

    private float GetAngle(Vector2 direction)
    {
        var angle = Math.Rad2Deg * Math.Acos(direction.Y);

        return direction.X < 0 ? 360.0f - angle : angle;
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

    public void Update(float deltaTime)
    {
        Scene.ForEach((Entity entity, ref Transform transform, ref OrbitCamera camera) =>
        {
            if(camera.firstFrame && camera.focus != null)
            {
                camera.focusPoint = camera.focus.Position;
                transform.LocalRotation = Math.FromEulerAngles(new Vector3(camera.orbitAngles.X, camera.orbitAngles.Y, 0));
            }

            UpdateFocusPoint(camera);

            Quaternion rotation;

            if(ManualRotation(camera) || AutomaticRotation(camera))
            {
                ConstrainAngles(camera);

                rotation = Math.FromEulerAngles(new Vector3(camera.orbitAngles.X, camera.orbitAngles.Y, 0));
            }
            else
            {
                rotation = transform.LocalRotation;
            }

            var direction = Vector3.Transform(new Vector3(0, 0, -1), rotation);

            var position = camera.focusPoint - direction * camera.distance;

            transform.LocalPosition = position;
            transform.LocalRotation = rotation;
        });
    }

    public void Shutdown()
    {
    }

    public void Startup()
    {
    }
}
