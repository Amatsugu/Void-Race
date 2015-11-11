using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Movement))]
public class InputController : MonoBehaviour {
	public enum _ControlMode{
		Player,
		AI
	};
	public _ControlMode ControlMode = _ControlMode.Player;
	public Movement Inst;
	public Vector2 TorqueVector = new Vector3();
	public float FlightSteerClamp = 20;
	private ControlMap _Controls;
	private DataMap _Data;
	public GameObject[] wayPoints;
	public int curWaypoint = 0;
	private bool hadFired;
	// Use this for initialization
	void Start ()
	{
		Inst = GetComponent<Movement>();
		_Controls = GameObject.Find("Controls").GetComponent<ControlMap>();
		_Data = GameObject.Find("Controls").GetComponent<DataMap>();
		LoadInputSettings();
		//Screen.lockCursor = true;
		wayPoints = GameObject.Find("wp0").GetComponent<WaypointReg>().GetWayPoints();
		if(ControlMode == _ControlMode.AI)
		{
//			Inst.thisCam.enabled = false;
//			Inst.thisCam.GetComponent<AudioListener>().enabled = false;
			if(GetComponent<HUD>() != null)
				GetComponent<HUD>().enabled = false;
		}
		if(Inst == null)
			Inst = GetComponent<Movement>();
	}
	void LoadInputSettings()
	{
		Debug.Log("Loading Configs");
		string configPath = Application.dataPath+"/KeyConfigs.txt";
		string dataPath = Application.dataPath+"/DataConfigs.txt";
		if(File.Exists(configPath))
		{
			_Controls.LoadMap(File.ReadAllLines(configPath));
		}
		else
		{
			Debug.Log("Creating Config File");
			StreamWriter configFile = File.CreateText(configPath);
			_Controls = _Controls.RegisterDefaultControls(_Controls);
			
			string[] controlMapping = _Controls.GetMap();
			foreach(string s in controlMapping)
			{
				configFile.WriteLine(s);
			}
			configFile.Flush();
		}
		if(File.Exists(dataPath))
		{
			_Data.LoadMap(File.ReadAllLines(dataPath));
		}else
		{
			Debug.Log("Creating Data File");
			StreamWriter dataFile = File.CreateText(dataPath);
			_Data = _Data.RegisterDefaultData(_Data);
			
			string[] dataMapping = _Data.GetMap();
			foreach(string s in dataMapping)
			{
				dataFile.WriteLine(s);
			}
			dataFile.Flush();
		}
	}
	// Update is called once per frame
	void Update () 
	{
		if(ControlMode == _ControlMode.Player)
		{

		}else
		{

		}
		if(Input.GetKeyDown(_Controls.GetKey("ToggleScreenLock")) && ControlMode != _ControlMode.AI)
		{
			Screen.lockCursor = !Screen.lockCursor;
		}
	}
	float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up) {
		Vector3 perp = Vector3.Cross(fwd, targetDir);
		float dir = Vector3.Dot(perp, up);
		
		if (dir > 0f) {
			return 1f;
		} else if (dir < 0f) {
			return -1f;
		} else {
			return 0f;
		}
	}
}
