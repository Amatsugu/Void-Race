using UnityEngine;
using System.Collections;

public class NetProjectileData : MonoBehaviour {
	
	public GameObject PlayerOwner;
	public float Damage;
	// Use this for initialization
	public bool SetData(GameObject owner)
	{
		PlayerOwner = owner;
//		networkView.RPC("SyncData", RPCMode.All, PlayerOwner);
		return true;
	}
	public bool SetData(GameObject owner, Vector3 move, float damage, float lifeTime, bool net)
	{
		PlayerOwner = owner;
		Damage = damage;
		if(lifeTime != 0)
			GetComponent<ObjectDestroyer>().LifeTime = lifeTime;
		if(move != Vector3.zero)
		{
			rigidbody.velocity = owner.rigidbody.velocity;
			rigidbody.AddForce(move);
		}
		if(net)
			networkView.RPC("SyncData", RPCMode.Others, owner, damage);
	//	networkView.RPC("SyncData", RPCMode.All, PlayerOwner);
		return true;
	}
	public bool SetData(GameObject owner, Vector3 move, float damage, float lifeTime)
	{
		return SetData(owner, move, damage, lifeTime, false);
	}
	public bool SetData(GameObject owner, float damage, bool net)
	{
		return SetData(owner, Vector3.zero, damage, 0, net);
	}
	public GameObject GetData()
	{
		return PlayerOwner;
	}
	[RPC]
	public void SyncData(GameObject owner, float damage)
	{
		PlayerOwner = owner;
		Damage = damage;
	}
	void OnCollisionEnter(Collision col)
	{
		if(col.collider.tag == "Player")
		{
			//Debug.Log("Collided with: "+col.collider.name);
			//col.gameObject.GetComponent<PhysicsMovement>().DealDamage(Damage, PlayerOwner.GetComponent<PhysicsMovement>().PlayerName);
		}
	}
	public void SendDataTo(GameObject target, bool net)
	{
		target.GetComponent<NetProjectileData>().SetData(PlayerOwner, Damage, net);
	}
}
