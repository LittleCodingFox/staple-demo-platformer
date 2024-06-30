using Staple;

namespace Platformer;

class SpinnerSystem : IEntitySystemUpdate
{
    public void Update(float deltaTime)
    {
        var spinners = Scene.ForEach<Transform, SpinnerComponent>();

        foreach((Entity entity, Transform transform, SpinnerComponent spinner) in spinners)
        {
            var eulerAngles = Math.ToEulerAngles(transform.LocalRotation);

            eulerAngles.Y += deltaTime * spinner.speed;

            if(eulerAngles.Y >= 360)
            {
                eulerAngles.Y = 360;
            }

            transform.LocalRotation = Math.FromEulerAngles(eulerAngles);
        }
    }

    public void Shutdown()
    {
    }

    public void Startup()
    {
    }
}
