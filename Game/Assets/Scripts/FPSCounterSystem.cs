using Staple;
using Staple.UI;

class FPSCounterSystem : IEntitySystemUpdate
{
    public void Update(float deltaTime)
    {
        var counters = Scene.ForEach<UIText, FPSCounterComponent>();

        foreach((Entity entity, UIText text, FPSCounterComponent component) in counters)
        {
            text.text = $"FPS: {Time.FPS}";
        }
    }

    public void Shutdown()
    {
    }

    public void Startup()
    {
    }
}
