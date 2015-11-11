using UnityEngine;
using System.Collections;

public class DefaultData : MonoBehaviour{

	public ControlMap RegisterDefaultControls(ControlMap _Controls)
	{
		Debug.Log("Resetting Controls");
		_Controls.Clear();
		_Controls.RegisterKey("Accelerate", KeyCode.W);
		_Controls.RegisterKey("Deccelerate", KeyCode.S);
		_Controls.RegisterKey("SteerLeft", KeyCode.A);
		_Controls.RegisterKey("SteerRight", KeyCode.D);
		_Controls.RegisterKey("RollLeft", KeyCode.Q);
		_Controls.RegisterKey("RollRight", KeyCode.E);
		_Controls.RegisterKey("Respawn", KeyCode.R);
		_Controls.RegisterKey("PowerControl", KeyCode.LeftShift);
		_Controls.RegisterKey("UseAbility", KeyCode.Space);
		_Controls.RegisterKey("LookBack", KeyCode.Tab);
		_Controls.RegisterKey("ToggleScreenLock", KeyCode.Escape);
		_Controls.RegisterKey("TargetingMode", KeyCode.Mouse1);
		_Controls.RegisterKey("ChangeShip", KeyCode.C);
		_Controls.RegisterKey("ActivateSheild", KeyCode.LeftControl);
		return _Controls;
	}
	public DataMap RegisterDefaultData(DataMap _DataMap)
	{
		Debug.Log("Resetting Data");
		_DataMap.Clear();
		_DataMap.RegisterData("SensitivityX", 4f);
		_DataMap.RegisterData("SensitivityY", 4f);
		_DataMap.RegisterData("InvertPitch", true);
		return _DataMap;
	}
}
