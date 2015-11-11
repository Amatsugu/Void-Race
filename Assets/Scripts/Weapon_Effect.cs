using UnityEngine;
using System.Collections;

[AddComponentMenu("Weapon/Weapon Effect")]
[RequireComponent(typeof(Weapon))]

public class Weapon_Effect : MonoBehaviour {
	public Material material;
	public Color ColorA;
	public Color ColorB;
	public float speed;
	public float EnergyUse;
	public GameObject[] OtherOBJs;
	public AudioClip Sound;
	private Material origMat;
	private EnergyManager E;
	private bool hasDrained;
	private bool isActive;
	private bool hasSet;
	// Use this for initialization
	void Start () 
	{
		origMat = gameObject.renderer.material;
		E = GetComponent<EnergyManager>();	
	}
	
	// Update is called once per frame
	public void ActivateNet()
	{
		Go(true);	
	}
	public void Activate()
	{
		Go(false);
	}
	void Go(bool net)
	{
		if(E == null)
			E = GetComponent<EnergyManager>();
		if(!hasDrained)
		{
			if(E.curEnergy > EnergyUse)
			{
				isActive = true;
				//Fire
				if(!hasSet)
				{	
					gameObject.renderer.material = material;
					hasSet = true;
					audio.PlayOneShot(Sound);
				}
				gameObject.tag = "Invis";
				gameObject.renderer.material.color = Color.Lerp(gameObject.renderer.material.color, ColorB, Time.deltaTime * speed);
				foreach(GameObject g in OtherOBJs)
				{
					g.SetActive(false);
				}
				E.UseEnergy(EnergyUse * Time.deltaTime);
				if(net)
					networkView.RPC("SyncEffect", RPCMode.Others, GetComponent<MeshRenderer>().material, false);
			}else
			{
				if(isActive)
				{
					//UnFire
					gameObject.tag = "Player";
					gameObject.renderer.material = origMat;
					hasDrained = true;
					isActive = false;
					hasSet = false;
					if(net)
						networkView.RPC("SyncEffect", RPCMode.Others, GetComponent<MeshRenderer>().material, true);
					foreach(GameObject g in OtherOBJs)
					{
						g.SetActive(true);
					}
				}
			}
		}
	}
	public void Reset(bool net)
	{
		if(!net)
		{
			//Debug.Log("Reset");
			gameObject.tag = "Player";
			isActive = false;
			hasSet = false;
			//reset
			gameObject.renderer.material = origMat;
			isActive = false;
			if(net)
				networkView.RPC("SyncEffect", RPCMode.Others, gameObject.renderer.material, true);
			foreach(GameObject g in OtherOBJs)
			{
				g.SetActive(true);
			}
			hasDrained = false;	
		}else
		{
			networkView.RPC("SyncEffect", RPCMode.Others, gameObject.renderer.material, true);
			hasDrained = false;
		}
	}
	[RPC]
	public void SyncEffect(Material mat, bool on)
	{
		gameObject.renderer.material = mat;
		foreach(GameObject g in OtherOBJs)
		{
			g.SetActive(on);
		}
	}
}
