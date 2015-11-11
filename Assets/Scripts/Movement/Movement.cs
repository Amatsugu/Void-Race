using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(InputController))]
public class Movement : MonoBehaviour 
{
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{

	}

	void Drive()
	{

	}
	void Hover()
	{

	}
	public void Steer(int dir)
	{
		Steer((float)dir);
	}
	public void Steer(float dir)
	{
		//curRot.y += dir;
	}
	public void Fly(Vector3 atv)
	{

	}
	public void Respawn()
	{

	}
}
