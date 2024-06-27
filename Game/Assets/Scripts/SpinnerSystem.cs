using Staple;

namespace Platformer;

class SpinnerSystem : IEntitySystemUpdate
{
    public void Update(float deltaTime)
    {
        Scene.ForEach((Entity entity, ref Transform transform, ref SpinnerComponent spinner) =>
        {
            var eulerAngles = Math.ToEulerAngles(transform.LocalRotation);

            eulerAngles.Y += deltaTime * spinner.speed;

            if(eulerAngles.Y >= 360)
            {
                eulerAngles.Y = 360;
            }

            transform.LocalRotation = Math.FromEulerAngles(eulerAngles);
        });
    }

    public void Shutdown()
    {
    }

    public void Startup()
    {
    }
}
