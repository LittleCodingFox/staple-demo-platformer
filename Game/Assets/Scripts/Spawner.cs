using Staple;
using System.Numerics;

public class Spawner : CallbackComponent
{
	public Prefab prefab;
	public int amount;
	public float radius = 5.0f;

	public override void Start()
	{
		if(prefab == null)
		{
			Log.Debug($"No prefab assigned to Spawner!");

			return;
		}

		Log.Debug($"Instancing {amount} prefabs!");

		for (var i = 0; i < amount; i++)
		{
			var instance = Entity.Instantiate(prefab, null);

			if(instance.IsValid == false)
			{
				continue;
			}

			var body = Physics.GetBody3D(instance);

            if (body == null)
            {
				continue;
            }

            body.Position += new Vector3((Randomizer.Default.RandomNormalized() - 0.5f) * radius, 0, (Randomizer.Default.RandomNormalized() - 0.5f) * radius);
		}

		entity.Destroy();
	}
}
