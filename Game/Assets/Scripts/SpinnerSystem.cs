using Staple;
using System.Numerics;

namespace Platformer;

class SpinnerSystem : IEntitySystemUpdate
{
    private readonly SceneQuery<SpinnerComponent> spinners = new();

    public void Update(float deltaTime)
    {
        foreach(var spinner in spinners.Contents)
        {
            var eulerAngles = spinner.Transform.LocalRotation.ToEulerAngles();

            eulerAngles.Y += deltaTime * spinner.speed;

            if(eulerAngles.Y >= 360)
            {
                eulerAngles.Y = 360;
            }

            spinner.Transform.LocalRotation = Quaternion.Euler(eulerAngles);
        }
    }
}
