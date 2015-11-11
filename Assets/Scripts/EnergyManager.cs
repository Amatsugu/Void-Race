using UnityEngine;
using System.Collections;
[AddComponentMenu("Weapon/Energy")]
public class EnergyManager : MonoBehaviour {
	
	public float MaxEnergy = 100;
	public float PassiveEnergyGain = 0.1f;
	public float StartEnergy = 1;
	public float curEnergyRatio;
	public float curEnergy;
	private float TargetEnergy;
	private float EnergyRate;
	private bool isUsingEnergy = false;
	// Use this for initialization
	void Start () 
	{
		curEnergy = StartEnergy;
	}
	
	void Update()
	{
		if(!isUsingEnergy)
		{
			curEnergy += PassiveEnergyGain * Time.deltaTime;
		}
		else
		{
			if(curEnergy <= MaxEnergy)
			{
				if(curEnergy != TargetEnergy)
				{
					Mathf.Lerp(curEnergy, TargetEnergy, Time.deltaTime * EnergyRate);
					if(curEnergy -0.1f >= TargetEnergy)
					{
						curEnergy = TargetEnergy;
					}
					if(curEnergy +0.1f >= TargetEnergy)
					{
						curEnergy = TargetEnergy;
					}
				}else
				{
					isUsingEnergy = false;
				}
			}
		}
		if(curEnergy > MaxEnergy)
			curEnergy = MaxEnergy;
		curEnergyRatio = curEnergy/MaxEnergy;
	}
	public void OnRespawn()
	{
		AddEnergy(MaxEnergy - curEnergy);
	}
	public void AddEnergy(float Ammount)
	{
		curEnergy += Ammount;
		EnergyRate = 1;
	}
	public void AddEnergy(float Ammount, bool net)
	{
		if(net)
		{
			if(networkView.isMine)
				AddEnergy(Ammount);
			else
				networkView.RPC("AddNetEnergy", RPCMode.Others, Ammount);
		}else
			AddEnergy(Ammount);
	}
	[RPC]
	void AddNetEnergy(float Ammount)
	{
		curEnergy += Ammount;
		EnergyRate = 1;
	}
	public void UseEnergy(float Ammount)
	{
		isUsingEnergy = true;
		TargetEnergy = curEnergy - Ammount;
		EnergyRate = 1;
	}
	public void UseEnergy(float Ammount, float rate)
	{
		isUsingEnergy = true;
		TargetEnergy = curEnergy - Ammount;
		EnergyRate = rate;
	}
}
