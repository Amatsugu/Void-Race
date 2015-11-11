using UnityEngine;
using System.Collections;
[AddComponentMenu("Weapon/Weapon Projectile")]
[RequireComponent(typeof(Weapon))]
public class Weapon_Projectile : MonoBehaviour {
	public float Damage;
	public float LifeTime;
	public float Speed;
	public GameObject Prefab;
	public float EnergyUse;
	public float EnergyModdifier;
	public float FireRate;
	public Transform Spawn;
	private EnergyManager E;
	private float nextFireTime;
	void Start()
	{
		E = GetComponent<EnergyManager>();
		nextFireTime = Time.time;
	}
	public void FireProjNet()
	{
		Shoot(true);
		Debug.Log("Shooting Net");
	}
	
	public void FireProj()
	{
		Shoot(false);
		//Debug.Log("Shooting");
	}
	void Shoot(bool net)
	{
		if(Time.time >= nextFireTime)
		{
			if(E.curEnergy >= EnergyUse)
			{
				if(net)
				{
					GameObject clone = Network.Instantiate(Prefab, Spawn.position, Quaternion.identity,1) as GameObject;
					clone.GetComponent<NetProjectileData>().SetData(gameObject, Spawn.forward * Speed, Damage *(EnergyModdifier * E.curEnergyRatio), LifeTime, true);
					E.UseEnergy(EnergyUse);
				}else
				{
					GameObject clone = Instantiate(Prefab, Spawn.position, Quaternion.identity) as GameObject;
					clone.GetComponent<NetProjectileData>().SetData(gameObject, Spawn.forward * Speed, Damage *(EnergyModdifier * E.curEnergyRatio), LifeTime);
					E.UseEnergy(EnergyUse);
				}
				nextFireTime = Time.time + FireRate;
			}
		}
	}
	public void Reset()
	{
		return;
	}
}
