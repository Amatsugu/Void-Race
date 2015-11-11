using UnityEngine;
using System.Collections;

public class NetworkDisable : MonoBehaviour {
	public bool isNetwork = false;
	// Use this for initialization
	void Start () 
	{
		if(Network.peerType != NetworkPeerType.Disconnected)
			isNetwork = true;
		if(!networkView.isMine)
		{
			foreach(LensFlare l in GetComponentsInChildren<LensFlare>())
			{
				l.enabled = false;
			}
		}
	}
}
