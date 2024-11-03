using Staple;

public class Spawner : CallbackComponent
{
	public Prefab prefab;
	public int amount;

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
			Entity.Instantiate(prefab, null);
		}

		entity.Destroy();
	}
}
