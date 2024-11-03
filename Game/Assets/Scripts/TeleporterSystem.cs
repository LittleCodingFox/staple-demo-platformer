using Staple;

namespace Platformer;

class TeleporterSystem : IEntitySystemLifecycle, IPhysicsReceiver3D
{
    public void Startup()
    {
    }

    public void Shutdown()
    {
    }

    public void OnBodyActivated(IBody3D body)
    {
    }

    public void OnBodyDeactivated(IBody3D body)
    {
    }

    public void OnContactAdded(IBody3D A, IBody3D B)
    {
        static void Handle(IBody3D body, TeleporterComponent teleporter)
        {
            body.Position = teleporter.position;
        }

        if(A.Entity.TryGetComponent<TeleporterComponent>(out var teleporter))
        {
            Handle(B, teleporter);
        }
        else if(B.Entity.TryGetComponent(out teleporter))
        {
            Handle(A, teleporter);
        }
    }

    public void OnContactPersisted(IBody3D A, IBody3D B)
    {
    }

    public void OnContactRemoved(IBody3D A, IBody3D B)
    {
    }

    public bool OnContactValidate(IBody3D A, IBody3D B)
    {
        return true;
    }
}
