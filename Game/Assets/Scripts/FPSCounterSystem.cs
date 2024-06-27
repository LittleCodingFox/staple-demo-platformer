using Staple;
using Staple.UI;

class FPSCounterSystem : IEntitySystemUpdate
{
    public void Update(float deltaTime)
    {
        Scene.ForEach((Entity entity, ref UIText text, ref FPSCounterComponent component) =>
        {
            text.text = $"FPS: {Time.FPS}";
        });
    }

    public void Shutdown()
    {
    }

    public void Startup()
    {
    }
}
