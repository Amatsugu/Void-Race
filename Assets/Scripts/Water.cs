using UnityEngine;
using System.Collections;

public class Water : MonoBehaviour {

	public float boyancy = 100f;
	public float minSplashSpeed = 20f;
	public float SplashForce = 100f;
	public GameObject SplashEffect;
	
	void OnTriggerStay(Collider col)
	{
		if(col.GetComponent<Rigidbody>() != null)
		{
			col.rigidbody.AddForce(Vector3.up * boyancy);
		}
	}
	void OnTriggerEnter(Collider col)
	{
		if(col.rigidbody.velocity.magnitude > minSplashSpeed)
		{
			col.rigidbody.AddForce(col.rigidbody.velocity.normalized * -1 * SplashForce);
			Splash(col.transform.position);
		}
	}
	void OnTriggerExit(Collider col)
	{
		if(col.rigidbody.velocity.magnitude > minSplashSpeed)
			Splash(col.transform.position);
	}
	void Splash(Vector3 pos)
	{
		Instantiate(SplashEffect, pos, Quaternion.identity);
	}
}
