using Staple;

namespace Platformer;

class CoinSystem : IEntitySystemLifecycle, IPhysicsReceiver3D
{
    public void Shutdown()
    {
    }

    public void Startup()
    {
    }

    public void OnBodyActivated(IBody3D self)
    {
    }

    public void OnBodyDeactivated(IBody3D self)
    {
    }

    public void OnContactAdded(IBody3D self, IBody3D other)
    {
        static void Handle(Entity entity, CoinComponent coin)
        {
            if (entity.TryGetComponent<AudioSource>(out var source) && coin.pickupClip != null)
            {
                source.audioClip = coin.pickupClip;

                source.Play();
            }

            entity.Destroy();
        }

        if (self.Entity.TryGetComponent<CoinComponent>(out var coin))
        {
            Handle(self.Entity, coin);
        }
        else if(other.Entity.TryGetComponent(out coin))
        {
            Handle(other.Entity, coin);
        }
    }

    public void OnContactPersisted(IBody3D self, IBody3D other)
    {
    }

    public void OnContactRemoved(IBody3D A, IBody3D B)
    {
    }

    public bool OnContactValidate(IBody3D self, IBody3D other)
    {
        return true;
    }
}
