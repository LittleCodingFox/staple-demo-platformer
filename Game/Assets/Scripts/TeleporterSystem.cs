using Staple;

namespace Platformer;

class TeleporterSystem : IEntitySystem, IPhysicsReceiver3D
{
    public EntitySubsystemType UpdateType => EntitySubsystemType.FixedUpdate;

    public void FixedUpdate(float deltaTime)
    {
    }

    public void Update(float deltaTime)
    {
    }

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
        void Handle(IBody3D body, TeleporterComponent teleporter)
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
