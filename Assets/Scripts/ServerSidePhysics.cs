using UnityEngine;
using System.Collections;
[RequireComponent(typeof(NetworkView))]
[RequireComponent(typeof(Rigidbody))]
public class ServerSidePhysics : MonoBehaviour {

	void Awake()
	{
		if(!networkView.isMine && Network.peerType != NetworkPeerType.Disconnected)
		{
			if(GetComponent("ConstantForce"))
			   Destroy(constantForce);
			Destroy(rigidbody);
		}
	}
}
