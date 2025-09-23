using Staple;
using System.Numerics;

namespace Platformer;

class SpinnerSystem : IEntitySystemUpdate
{
    private readonly SceneQuery<Transform, SpinnerComponent> spinners = new();

    public void Update(float deltaTime)
    {
        foreach((_, Transform transform, SpinnerComponent spinner) in spinners.Contents)
        {
            var eulerAngles = transform.LocalRotation.ToEulerAngles();

            eulerAngles.Y += deltaTime * spinner.speed;

            if(eulerAngles.Y >= 360)
            {
                eulerAngles.Y = 360;
            }

            transform.LocalRotation = Quaternion.Euler(eulerAngles);
        }
    }
}
