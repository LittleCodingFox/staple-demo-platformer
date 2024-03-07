using Staple;
using System.Numerics;

namespace Platformer;

class OrbitCameraSystem : IEntitySystem
{
    public SubsystemType UpdateType => SubsystemType.Update;

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
            }

            var target = camera.focus.Position;

            if(camera.focusRadius > 0)
            {
                var distance = Vector3.Distance(target, camera.focusPosition);

                var t = 1.0f;

                if(distance > 0.01f && camera.focusCentering > 0)
                {
                    t = Math.Pow(1.0f - camera.focusCentering, Time.deltaTime);
                }

                if(distance > camera.focusRadius)
                {
                    t = Math.Min(t, camera.focusRadius / distance);
                }

                camera.focusPosition = Vector3.Lerp(target, camera.focusPosition, t);
            }

            var direction = transform.Forward;

            transform.LocalPosition = camera.focusPosition - direction * camera.distance;
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
