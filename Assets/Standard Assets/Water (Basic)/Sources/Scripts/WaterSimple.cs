using UnityEngine;

// Sets up transformation matrices to scale&scroll water waves
// for the case where graphics card does not support vertex programs.

[ExecuteInEditMode]
public class WaterSimple : MonoBehaviour
{
//	private bool inv = false;
//	public float curScale;
//	public float scaleSpeed = 5;
//	public float minScale = 50;
//	public float maxScale = 200;
//	void Start()
//	{
//		curScale = minScale;
//	}
	void Update()
	{
		if( !GetComponent<Renderer>() )
			return;
		Material mat = GetComponent<Renderer>().sharedMaterial;
		if( !mat )
			return;
		
		
//		if(!inv)
//		{
//			curScale += scaleSpeed * Time.deltaTime;
//			if(curScale > maxScale)
//				inv = true;
//		}else
//		{
//			curScale -= scaleSpeed * Time.deltaTime;
//			if(curScale < minScale)
//				inv = false;
//		}
//		mat.SetTextureScale("_MainTex", new Vector2(curScale, curScale));
//		mat.SetTextureScale("_BumpMap", new Vector2(curScale, curScale));
//		mat.SetTextureScale("_HeigthMap", new Vector2(curScale, curScale));
		Vector4 waveSpeed = mat.GetVector( "WaveSpeed" );
		float waveScale = mat.GetFloat( "_WaveScale" );
		float t = Time.time / 20.0f;
		
		Vector4 offset4 = waveSpeed * (t * waveScale);
		Vector4 offsetClamped = new Vector4(Mathf.Repeat(offset4.x,1.0f), Mathf.Repeat(offset4.y,1.0f), Mathf.Repeat(offset4.z,1.0f), Mathf.Repeat(offset4.w,1.0f));
		mat.SetVector( "_WaveOffset", offsetClamped );
		
		Vector3 scale = new Vector3( 1.0f/waveScale, 1.0f/waveScale, 1 );
		Matrix4x4 scrollMatrix = Matrix4x4.TRS( new Vector3(offsetClamped.x,offsetClamped.y,0), Quaternion.identity, scale );
		mat.SetMatrix( "_WaveMatrix", scrollMatrix );
				
		scrollMatrix = Matrix4x4.TRS( new Vector3(offsetClamped.z,offsetClamped.w,0), Quaternion.identity, scale * 0.45f );
		mat.SetMatrix( "_WaveMatrix2", scrollMatrix );
	}
}
