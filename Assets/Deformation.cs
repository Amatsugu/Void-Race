using UnityEngine;
using System.Collections;

public class Deformation : MonoBehaviour {
	
	public float MinDeformVelocity = 1;
	public float MaxDeformation = 2;
	
	private Rigidbody thisRigidbody;
	private Transform thisTransform;
	private Mesh thisMesh;
	// Use this for initialization
	void Start () 
	{
		thisMesh = GetComponent<MeshFilter>().mesh;
		thisTransform = transform;
		thisRigidbody = rigidbody;
	}
	
	void OnCollisionEnter(Collision c)
	{
		if(thisRigidbody.velocity.magnitude >= MinDeformVelocity)
		{
			Vector3[] vertices = thisMesh.vertices;
			int i = 0;
			foreach(Vector3 v in vertices)
			{
				if(v == c.contacts[0].point)
				{
					vertices[i] -= thisRigidbody.velocity.normalized * MaxDeformation;
				}
				i++;
			}
		}
	}
}
