using Staple;

namespace Platformer;

class SpinnerSystem : IEntitySystem
{
    public SubsystemType UpdateType => SubsystemType.Update;

    public void Process(float deltaTime)
    {
        Scene.ForEach((Entity entity, ref Transform transform, ref SpinnerComponent spinner) =>
        {
            var eulerAngles = Math.ToEulerAngles(transform.LocalRotation);

            eulerAngles.Y += Time.deltaTime * spinner.speed;

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
