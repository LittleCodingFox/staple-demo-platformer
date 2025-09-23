using Staple;
using System.Numerics;

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

        transform.LocalRotation = Quaternion.Euler(new(0, timer * speed + offset, 0));
    }
}
