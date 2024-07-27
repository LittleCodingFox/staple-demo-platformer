using Staple;
using Staple.UI;

class FPSCounterSystem : IEntitySystemUpdate
{
    public void Update(float deltaTime)
    {
        var counters = Scene.Query<UIText, FPSCounterComponent>();

        foreach((_, UIText text, _) in counters)
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
