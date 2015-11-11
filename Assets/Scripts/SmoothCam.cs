using UnityEngine;

using System.Collections;

public class SmoothCam : MonoBehaviour {
	
	public Transform FWDTarget;
	public Transform BackTarget;
	public float distance = 10.0f;
	public float height = 5.0f;
	public float rotationDamping = 3.0f;
	private Transform curTarget;
	private Transform thisTransform;
	private ControlMap _Controls;
	// Use this for initialization
	void Start () {
		curTarget = FWDTarget;
		thisTransform = transform;
		_Controls = GameObject.Find("Controls").GetComponent<ControlMap>();
	}
	
	// Update is called once per frame
	void Update () {
		if (!curTarget)
			return;
		if(Input.GetKeyDown(_Controls.GetKey("LookBack")))
			curTarget = BackTarget;
		if(Input.GetKeyUp(_Controls.GetKey("LookBack")))
			curTarget = FWDTarget;
		Vector3 curPos = curTarget.localPosition;
		curPos.z -= distance;
		curPos.y += height;
		thisTransform.localPosition = curPos;
		Vector3 desiredDir = Quaternion.LookRotation(curTarget.localPosition - thisTransform.localPosition).eulerAngles;
		thisTransform.localRotation = Quaternion.Euler(desiredDir.x, desiredDir.y, desiredDir.z);
		Vector3 desiredEuler = thisTransform.localRotation.eulerAngles;
		Quaternion desiredRoll = Quaternion.Euler(new Vector3(desiredEuler.x, desiredEuler.y, curTarget.localRotation.eulerAngles.z));
		thisTransform.localRotation = desiredRoll;
	}
}
