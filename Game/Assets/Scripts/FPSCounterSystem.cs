﻿using Staple;
using Staple.UI;

class FPSCounterSystem : IEntitySystemUpdate
{
    private readonly SceneQuery<UIText, FPSCounterComponent> counters = new();

    public void Update(float deltaTime)
    {
        var fps = $"FPS: {Time.FPS}";

        foreach ((_, UIText text, _) in counters)
        {
            text.text = fps;
        }
    }

    public void Shutdown()
    {
    }

    public void Startup()
    {
    }
}
