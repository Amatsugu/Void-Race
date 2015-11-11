using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {
	//Public
	public Transform Target;
	public float Distance = 10f;
	public float Height = 10f;
	public bool lerpRot;
	public float TurnSpeed = 1f;
	public bool lerpPos;
	public float MoveSpeed = 2f;
	//Private
	private Transform thisTransform;
	// Use this for initialization
	void Start () 
	{
		thisTransform = transform;
	}
	
	// Update is called once per frame
	void Update () 
	{
		//Rotation
		Quaternion tarRot = Target.rotation;
		if(lerpRot)
			thisTransform.rotation = Quaternion.Lerp(thisTransform.rotation, tarRot, TurnSpeed);
		else
			thisTransform.rotation = tarRot;
		//Position
		Vector3 thisPos = thisTransform.position;
		Ray R = new Ray(Target.position, Target.forward);
		Vector3 desiredPos = R.GetPoint(-1*Distance);
		Ray R2 = new Ray(desiredPos, Target.up);
		Debug.DrawLine(R.origin, desiredPos,Color.magenta);
		Debug.DrawLine(R2.origin, R2.GetPoint(Height), Color.cyan);
		desiredPos = R2.GetPoint(Height);
		if(lerpPos)
			thisTransform.position = Vector3.Lerp(thisPos, desiredPos, MoveSpeed);
		else
			thisTransform.position = desiredPos;
	}
}
