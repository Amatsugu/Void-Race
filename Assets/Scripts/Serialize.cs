using UnityEngine;
using System.Collections;
[RequireComponent(typeof(NetworkView))]
public class Serialize: MonoBehaviour {
	private bool serialize = true;
	private bool isNetwork = false;
	void Start()
	{
		if(Network.peerType != NetworkPeerType.Disconnected)
			isNetwork = true;
		if(isNetwork)
		{
			serialize = true;
			if(!networkView.isMine)
			{
				if(gameObject.GetComponent<Rigidbody>())
					Destroy(rigidbody);
			}
		}
	}
	
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		if(serialize)
		{
			if(stream.isWriting)
			{
				Vector3 pos = transform.position;
				stream.Serialize(ref pos);
			}
			else
			{
				Vector3 posRec = Vector3.zero;
				stream.Serialize(ref posRec);
				transform.position = posRec;
			}
		}
	}
}
