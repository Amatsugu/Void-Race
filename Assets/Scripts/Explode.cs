using UnityEngine;
using System.Collections;

public class Explode : MonoBehaviour {
	public float Force = 1000f;
	public float Radius = 10f;
	public float UpwardMod = 3f;
	// Use this for initialization
	void Start()
	{
		Vector3 explosionPos = transform.position;
    	Collider[] colliders = Physics.OverlapSphere (explosionPos, Radius);
    
    	foreach(Collider hit in colliders)
		{
			if(hit.tag == "Player" || hit.tag == "Invis")
			{
				float curDMG = GetComponent<NetProjectileData>().Damage;
				curDMG *= Vector3.Distance(explosionPos, hit.transform.position)/Radius;
				//Apply Explosion
			}
   		}
	}
	void FixedUpdate() 
	{
		Vector3 explosionPos = transform.position;
    	Collider[] colliders = Physics.OverlapSphere (explosionPos, Radius);
    
    	foreach(Collider hit in colliders)
		{
        	if (hit.rigidbody)
            	hit.rigidbody.AddExplosionForce(Force, explosionPos, Radius, UpwardMod);
			if(hit.tag == "Player" || hit.tag == "Invis")
			{
				float curDMG = GetComponent<NetProjectileData>().Damage * Time.deltaTime;
				curDMG *= Vector3.Distance(explosionPos, hit.transform.position)/Radius;
				//Apply Explosion
			}
   		}
	}
}