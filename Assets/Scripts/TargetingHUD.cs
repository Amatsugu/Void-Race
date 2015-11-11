using UnityEngine;
using System.Collections;

public class TargetingHUD : MonoBehaviour {

	public Vector2 targetPos;
	public float scale;
	public Texture2D TexBlue1;
	public Texture2D TexRed1;
	public Texture2D TexBlue2;
	public Texture2D TexRed2;
	private bool hasHit;
	private bool isActive;
	private bool isTarget;
	private float TargetingRange = 4000f;
	public Transform thisTransform;
	private ControlMap _Controls;
	private float curRotation;
	void Start()
	{
		_Controls = GameObject.Find("Controls").GetComponent<ControlMap>();
	}
	void Update()
	{
		RaycastHit hit;
		Physics.Raycast(thisTransform.position, thisTransform.forward,out hit, TargetingRange);
		if(hit.point != Vector3.zero)
		{
			//targetPos = GetComponent<PhysicsMovement>().thisCam.WorldToScreenPoint(hit.point);
			hasHit = true;
		}else
		{
			hasHit = false;
		}
		targetPos.y = Screen.height - targetPos.y;
		scale = Vector3.Distance(hit.point, thisTransform.position)/TargetingRange;
		scale = 1-scale;
		Debug.DrawLine(thisTransform.position, hit.point);
		if(hit.collider != null)
		{
			if(hit.collider.tag == "Player" || hit.collider.tag == "Ignore")
			{
				isTarget = true;
			}else
			{
				isTarget = false;
			}
		}
		isActive = Input.GetKey(_Controls.GetKey("TargetingMode"));
	}
	void OnGUI()
	{
		if(isActive)
		{
			if(hasHit)
			{
				GUIUtility.ScaleAroundPivot(new Vector2(.5f, .5f) * scale, targetPos);
				GUIUtility.RotateAroundPivot(curRotation, targetPos);
				if(isTarget)
				{
					GUI.DrawTexture(new Rect(targetPos.x - TexRed1.width/2, targetPos.y - TexRed1.height/2, TexRed1.width, TexRed1.height), TexRed1);
				}else
				{
					GUI.DrawTexture(new Rect(targetPos.x - TexBlue1.width/2, targetPos.y - TexBlue1.height/2, TexBlue1.width, TexBlue1.height), TexBlue1);
				}
				GUIUtility.RotateAroundPivot(curRotation*-2, targetPos);
				if(isTarget)
				{
					GUI.DrawTexture(new Rect(targetPos.x - TexRed2.width/2, targetPos.y - TexRed2.height/2, TexRed2.width, TexRed2.height), TexRed2);
				}else
				{
					GUI.DrawTexture(new Rect(targetPos.x - TexBlue2.width/2, targetPos.y - TexBlue2.height/2, TexBlue2.width, TexBlue2.height), TexBlue2);
				}
				if(curRotation != 360)
					curRotation += 90 * Time.deltaTime;
				else
					curRotation = 0;
			}
		}
	}
}