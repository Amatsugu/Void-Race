using UnityEngine;
using System.Collections;

public class ObjectDestroyer : MonoBehaviour {
	
	public float LifeTime = 2;
	public bool DropChildren = false;
	public bool DestroyOnCollide = true;
	public bool DestroyDrop;
	public GameObject Drop;
	public bool ColorDrop;
	public Color ObjColor;
	public bool PassData;
	//private
	private bool wasP = false;
	private float PausedTime;
	private float StartTime;
	private bool isPaused = false;
	private bool isNetwork = false;
	void Start()
	{
		wasP = false;
		PausedTime = 0f;
		LifeTime += Time.time;
		StartTime = Time.time;
		if(Network.peerType != NetworkPeerType.Disconnected)
		{
			isNetwork = true;
			if(GetComponent<NetworkView>() == null)
				gameObject.AddComponent<NetworkView>();
		}
		//Debug.Log("Start Time: " + StartTime);
	}
	public void Pause()
	{
		isPaused = true;
	}
	public void UnPause()
	{
		isPaused = false;
	}
	void Update () 
	{
		float curTime = Time.time;
		if(!isPaused)
		{
			if(wasP)
			{
				LifeTime += PausedTime;
				//Debug.Log("Time Paused: " + PausedTime);
				//Debug.Log("LifeTime On Resume: " + LifeTime);
				PausedTime = 0;
				wasP = false;
			}
			if(Input.GetKeyUp(KeyCode.Escape) && !wasP)
			{
				StartTime = Time.time;
			}
			if(LifeTime <= curTime)
			{
				DestroyMe();
			}
		}
		if(isPaused)
		{
			wasP = true;
			PausedTime = curTime - StartTime;
		}
	}
	void OnCollisionEnter()
	{
		if(isNetwork)
		{
			if(DestroyOnCollide && networkView.isMine)
				DestroyMe();
		}else
		{
			if(DestroyOnCollide)
				DestroyMe();
		}
	}
	public void DestroyMe()
	{
		if(DropChildren)
		{
			transform.DetachChildren();
		}
		NetProjectileData NPD = GetComponent<NetProjectileData>();
		if(DestroyDrop)
		{
			GameObject Clone = null;
			if(isNetwork)
			{
				if(networkView.isMine)
				{
					Clone = Network.Instantiate(Drop, transform.position, Quaternion.identity, 1) as GameObject;
					if(PassData)
						Clone.GetComponent<NetProjectileData>().SetData(NPD.PlayerOwner, NPD.Damage, true);
				}
				
			}
			else
			{
				Clone = Instantiate(Drop, transform.position, Quaternion.identity) as GameObject;
				if(PassData)
					Clone.GetComponent<NetProjectileData>().SetData(NPD.PlayerOwner, NPD.Damage, false);
				if(ColorDrop)
				{
					Clone.SendMessage("SetColor", ObjColor);
				}
			}
		}
		if(isNetwork)
		{
			if(networkView.isMine)
				Network.Destroy(gameObject);
		}
		else
			Destroy(gameObject);
	}
}
