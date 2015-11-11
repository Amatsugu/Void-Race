using UnityEngine;
using System.Collections;
public enum WeaponType 
{
	Off,
	Projectile,
	Beam,
	Effect
}
[AddComponentMenu("Weapon/Weapon Base")]
[RequireComponent(typeof(EnergyManager))]
public class Weapon : MonoBehaviour
{
	public WeaponType Type = WeaponType.Off;
	static public Transform thisTransform;
	void Start () 
	{
		if(Type == WeaponType.Off)
		{
			if(GetComponent<Weapon_Beam>() != null)
				Type = WeaponType.Beam;
			if(GetComponent<Weapon_Projectile>() != null)
				Type = WeaponType.Projectile;
		}
		if(Type == WeaponType.Beam && GetComponent<Weapon_Beam>() == null)
			Debug.Log("No Beam Module Attached");
		if(Type == WeaponType.Projectile && GetComponent<Weapon_Projectile>() == null)
			Debug.Log("No Projectile Module Attached");
		if(Type == WeaponType.Beam && GetComponent<Weapon_Beam>().GetType() != typeof(Weapon_Beam))
		{
			Debug.Log("Weapon Type Mismatch");
		}
		if(Type == WeaponType.Projectile && GetComponent<Weapon_Projectile>().GetType() != typeof(Weapon_Projectile))
		{
			Debug.Log("Weapon Type Mismatch");
		}
		if(Type == WeaponType.Effect && GetComponent<Weapon_Effect>().GetType() != typeof(Weapon_Effect))
		{
			Debug.Log("Weapon Type Mismatch");
		}
		if(Type == WeaponType.Effect && GetComponent<Weapon_Effect>() == null)
		{
			Debug.Log("No Effect Module Attached");
		}
		thisTransform = transform;
		thisTransform.FindChild("B_Spawn");
	}
	public void ShootPrimraryNET () 
	{
		Shoot(true);
		//Debug.Log("Net Fire");
	}
	public void ShootPrimrary()
	{
		Shoot(false);
		//Debug.Log("Fire");
	}
	void Shoot(bool net)
	{
		if(Type == WeaponType.Projectile)
		{
			if(net)
			{
				GetComponent<Weapon_Projectile>().FireProjNet();
			}
			else
			{	
				GetComponent<Weapon_Projectile>().FireProj();
			}
		}else if(Type == WeaponType.Beam)
		{
			if(net)
			{
				GetComponent<Weapon_Beam>().FireBeamNet();
			}
			else
			{	
				GetComponent<Weapon_Beam>().FireBeam();
			}
		}else if(Type == WeaponType.Effect)
		{
			if(net)
			{
				GetComponent<Weapon_Effect>().ActivateNet();
			}
			else
			{	
				GetComponent<Weapon_Effect>().Activate();
			}
		}
	}
}