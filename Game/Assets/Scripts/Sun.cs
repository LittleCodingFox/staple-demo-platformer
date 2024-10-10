using Staple;

namespace Platformer;

public class Sun : CallbackComponent
{
    public float time = 10.0f;
    public Color startColor = Color.White;
    public Color endColor = Color.White;

    private Transform transform;
    private Light light;
    private float timer = 0.0f;

    public override void Awake()
    {
        transform = entity.GetComponent<Transform>();
        light = entity.GetComponent<Light>();
    }

    public override void FixedUpdate()
    {
        if(light == null)
        {
            return;
        }

        timer += Time.fixedDeltaTime;

        if (timer >= time)
        {
            timer = 0.0f;
        }

        var t = timer / time;

        var rotation = Math.FromEulerAngles(new(t * 360, 0, 0));

        transform.LocalRotation = rotation;
    }
}
