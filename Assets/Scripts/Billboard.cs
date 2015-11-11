using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour {
	public bool useMainCam;
	public Transform Target;
	public int SetLayer = 8;
	public float TurnSpeed = 2;
	private bool isNetwork;
	private Transform owner;
	// Use this for initialization
	void Start () 
	{
		if(useMainCam)
			Target = Camera.main.transform;
		if(Network.peerType != NetworkPeerType.Disconnected)
			isNetwork = true;
		if(!isNetwork || networkView.isMine)
		{
			gameObject.layer = SetLayer;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(transform != null)
			transform.rotation = Quaternion.Lerp(transform.rotation, Target.rotation,2 * Time.deltaTime);
	}
}
