using Staple;

public class Rotater : CallbackComponent
{
    public float speed = 90;
    public float offset = 0;

    private float timer = 0.0f;
    private Transform transform;

    public override void Awake()
    {
        transform = entity.GetComponent<Transform>();
    }

    public override void Update()
    {
        timer += Time.deltaTime;

        transform.LocalRotation = Math.FromEulerAngles(new(0, timer * speed + offset, 0));
    }
}
