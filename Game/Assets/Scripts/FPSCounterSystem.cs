using Staple;

class FPSCounterSystem : IEntitySystem
{
    public EntitySubsystemType UpdateType => EntitySubsystemType.Update;

    public void FixedUpdate(float deltaTime)
    {
    }

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
