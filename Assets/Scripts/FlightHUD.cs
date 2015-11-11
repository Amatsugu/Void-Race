using UnityEngine;
using System.Collections;

public class FlightHUD : MonoBehaviour {

	// Use this for initialization
	public Texture2D BaseTex;
	public Texture2D[] IndicatorTex;
	private InputController InputC;
	void Start () 
	{
		InputC = GetComponent<InputController>();
	}
	void OnGUI()
	{
		float steerMagnitude =  Mathf.Sqrt((InputC.TorqueVector.x * InputC.TorqueVector.x)+(InputC.TorqueVector.y * InputC.TorqueVector.y));
		/*if(!InputC.Inst.isGrounded)
		{
			float vectorAngle = 0;
			if(InputC.TorqueVector != Vector2.zero)
			{
				vectorAngle = Mathf.Atan(InputC.TorqueVector.y/InputC.TorqueVector.x);
			}else
			{
				vectorAngle = 0;
			}
			vectorAngle *= Mathf.Rad2Deg;
			if(InputC.TorqueVector.x < 0)
				vectorAngle*= -1;
			else if(InputC.TorqueVector.y < 0 && InputC.TorqueVector.x > 0)
				vectorAngle-= 180;
			else if(InputC.TorqueVector.y > 0 && InputC.TorqueVector.x > 0)
				vectorAngle-= 180;
			if(InputC.TorqueVector.x > 0 && InputC.TorqueVector.y > 0 && vectorAngle < 0)
				vectorAngle*=-1;
			if(InputC.TorqueVector.x > 0 && InputC.TorqueVector.y < 0 && vectorAngle < 0)
				vectorAngle*=-1;
			if(vectorAngle > 360)
				vectorAngle = 360;
			if(vectorAngle < -360)
				vectorAngle = -360;
			//Debug.Log(vectorAngle + "x: " + InputC.TorqueVector.x + " y: " + InputC.TorqueVector.y);
			GUIUtility.RotateAroundPivot(vectorAngle, new Vector2(Screen.width/2, Screen.height/2));
			GUI.DrawTexture(new Rect((Screen.width/2) - (BaseTex.width/2), (Screen.height/2) - (BaseTex.height/2), BaseTex.width, BaseTex.height), BaseTex);
			//80%
			if(steerMagnitude > InputC.FlightSteerClamp * .75)
			{
				GUI.DrawTexture(new Rect(Screen.width/2 - IndicatorTex[3].width/2, Screen.height/2 - IndicatorTex[3].height/2, IndicatorTex[3].width, IndicatorTex[3].height), IndicatorTex[3]);
			}
			//75%
			if(steerMagnitude <= InputC.FlightSteerClamp * .75 && steerMagnitude > InputC.FlightSteerClamp * .5)
			{
				GUI.DrawTexture(new Rect(Screen.width/2 - IndicatorTex[2].width/2, Screen.height/2 - IndicatorTex[2].height/2, IndicatorTex[2].width, IndicatorTex[2].height), IndicatorTex[2]);
			}
			//50%
			if(steerMagnitude <= InputC.FlightSteerClamp * .5 && steerMagnitude > InputC.FlightSteerClamp * .25)
			{
				GUI.DrawTexture(new Rect(Screen.width/2 - IndicatorTex[1].width/2, Screen.height/2 - IndicatorTex[1].height/2, IndicatorTex[1].width, IndicatorTex[1].height), IndicatorTex[1]);
			}
			//25%
			if(steerMagnitude <= InputC.FlightSteerClamp * .25 && steerMagnitude > 0)
			{
				GUI.DrawTexture(new Rect(Screen.width/2 - IndicatorTex[0].width/2, Screen.height/2 - IndicatorTex[0].height/2, IndicatorTex[0].width, IndicatorTex[0].height), IndicatorTex[0]);
			}
		}*/
	}
}
