using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SheildController : MonoBehaviour 
{
	//Public
	public GameObject SheildArray;
	public float EnergyCost = 25;
	public float Duration = 5;
	public float CoolDownTime = 2;
	public float EnergyRatio = .8f;
	public float Offset = 5;
	public ParticleSystem _pSys;
	public GameObject _Sheild;
	public float SheildSpinSpeed;
	//Private
	private EnergyManager _E;
	private ControlMap _Controls;
		
	public bool _isActive;
	private bool _hasSet;
	public float _timeOut;
	public float _coolDown;
	private Transform thisT;
	private float _angle;
	
	
	void Start () 
	{
		_E = GetComponent<EnergyManager>();
		_Controls = GameObject.Find("Controls").GetComponent<ControlMap>();
		_pSys.enableEmission = false;
		thisT = transform;
		_Sheild.SetActive(false);
	}
	
	void Update ()
	{
		if(Time.time >= _coolDown)
		{
			if(Input.GetKeyDown(_Controls.GetKey("ActivateSheild")))
			{
				_isActive = true;
				_timeOut = Time.time + Duration;
				_coolDown = _timeOut + CoolDownTime;
				_E.UseEnergy(EnergyCost);
				_Sheild.SetActive(true);
			}
		}
		if(_isActive)
		{
			if(!_hasSet)
			{
				_hasSet = true;
				_pSys.enableEmission = true;
			}
			if(_angle > 360)
				_angle = 0;
			_angle += SheildSpinSpeed * Time.deltaTime;
			_Sheild.transform.localRotation = Quaternion.Euler(new Vector3(0,_angle,0));
			if(Time.time >= _timeOut)
			{
				_isActive = false;
				_hasSet = false;
				_pSys.enableEmission = false;
				_Sheild.SetActive(false);
			}
		}
	}
}