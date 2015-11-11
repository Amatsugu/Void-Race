using UnityEngine;
using System.Collections;
[AddComponentMenu("Weapon/Weapon Beam")]
[RequireComponent(typeof(Weapon))]
public class Weapon_Beam : MonoBehaviour {
	public float Damage;
	public float Range;
	public float Speed;
	public float EnergyUse;
	public float Rate;
	public float Width;
	public GameObject BeamStart;
	public float Particles;
	public float BeamForce = 500;
	public ParticleSystem[] BeamHitEffect;
	private LineRenderer Beam;
	public ParticleSystem[] BeamFireEffect;
	private Transform thisTransform;
	private EnergyManager E;
	private bool isFiring;
	private Vector3 BeamHitStart;
	private Vector3 curBeamPos;
	private bool hasDrained;
	private float beamLightIntensity;
	void Start()
	{
		if(thisTransform == null)
			thisTransform = transform;
		E = GetComponent<EnergyManager>();
		if(BeamStart == null)
		{
			Debug.LogError("Beam start point not assigned.");
			Debug.Break();
			return;
		}
		if(BeamStart.GetComponent<LineRenderer>() != null)
			Beam = BeamStart.GetComponent<LineRenderer>();
		else
		{
			Debug.LogError("Beam start point does not contain a LineRenderer.");
			Debug.Break();
			return;
		}
		if(BeamStart.GetComponent<ParticleSystem>() == null)
		{
			Debug.LogError("Beam start point does not contain a ParticleSystem.");
			Debug.Break();
			return;
		}
		if(BeamHitEffect == null)
		{
			Debug.LogError("Beam hit effect not assigned.");
			Debug.Break();
			return;
		}
		foreach(ParticleSystem p in BeamHitEffect)
		{
			BeamHitStart = p.transform.position;
		}
		beamLightIntensity = BeamStart.light.intensity;
		BeamStart.light.intensity = 0;
		BeamHitEffect[0].light.intensity = 0;
	}
	
	public void FireBeamNet()
	{
		Fire(true);	
	}
	public void FireBeam()
	{
		Fire(false);
	}
	void Fire(bool net)
	{
		if(E == null)
			E = GetComponent<EnergyManager>();
		if(!hasDrained)
		{
			if(E.curEnergy > EnergyUse)
			{
				isFiring = true;
				if(!Beam.audio.isPlaying)
					Beam.audio.Play();
				BeamStart.light.intensity = beamLightIntensity * E.curEnergyRatio;
				BeamHitEffect[0].light.intensity = beamLightIntensity * E.curEnergyRatio;
				RaycastHit BeamHit;
				if(Physics.Raycast(BeamStart.transform.position, BeamStart.transform.forward, out BeamHit, Range))
				{
					Beam.useWorldSpace = true;
					Beam.SetPosition(0, BeamStart.transform.position);
					Beam.SetPosition(1, BeamHit.point);
					Beam.SetWidth(Width*E.curEnergyRatio, Width*E.curEnergyRatio);
					if(net)
					{
						networkView.RPC("SyncBeam", RPCMode.Others, BeamStart, BeamHit.point, Width*E.curEnergyRatio);
					}
					foreach(ParticleSystem p in BeamFireEffect)
					{
						p.Emit((int)(Particles*E.curEnergyRatio));
					}
					foreach(ParticleSystem p in BeamHitEffect)
					{
						p.transform.position = BeamHit.point;
						p.Emit((int)(Particles*E.curEnergyRatio));
					}
					if(BeamHit.collider.tag != "Player" || BeamHit.collider.tag != "Invis")
						E.UseEnergy(EnergyUse/Rate);
					if(BeamHit.collider.tag == "Player" || BeamHit.collider.tag == "Invis")
					{
						//BeamHit.collider.GetComponent<PhysicsMovement>().DealDamage((Damage*Time.deltaTime)*E.curEnergyRatio, GetComponent<PhysicsMovement>().PlayerName);	
					}
					if(BeamHit.rigidbody != null)
						BeamHit.rigidbody.AddForceAtPosition(thisTransform.forward * BeamForce * E.curEnergyRatio, BeamHit.point);
				}else
				{
					Beam.useWorldSpace = false;
					curBeamPos = Vector3.forward * Range;//Vector3.Lerp(curBeamPos , Vector3.forward * Range, Time.deltaTime* Speed);
					Beam.SetPosition(0, Vector3.zero);
					Beam.SetPosition(1, curBeamPos);
					Beam.SetWidth(Width*E.curEnergyRatio, Width*E.curEnergyRatio);
					if(net)
					{
						networkView.RPC("SyncBeam", RPCMode.Others, BeamStart, curBeamPos, Width*E.curEnergyRatio);
					}
					foreach(ParticleSystem p in BeamFireEffect)
					{
						p.Emit((int)(Particles*E.curEnergyRatio));
					}
					E.UseEnergy(EnergyUse/Rate);
				}
			}else
			{
				if(isFiring)
				{
					isFiring = false;
					foreach(ParticleSystem p in BeamHitEffect)
					{
						p.transform.position = BeamHitStart;
						p.Clear();
					}
					Beam.SetWidth(0,0);
					Beam.audio.Stop();
				}
				hasDrained = true;
			}
		}
	}
	public void Reset(bool net)
	{
		if(!net)
		{
			isFiring = false;
			foreach(ParticleSystem p in BeamHitEffect)
			{
				p.transform.position = BeamHitStart;
				p.Clear();
			}
			Beam.audio.Stop();
			Beam.SetWidth(0,0);
			BeamStart.light.intensity = 0;
			BeamHitEffect[0].light.intensity = 0;
			hasDrained = false;
		}else
		{
			networkView.RPC("SyncBeam", RPCMode.Others, BeamStart, BeamStart, 0);
			hasDrained = false;
		}
	}
	[RPC]
	void SyncBeam(Vector3 start, Vector3 end, float width)
	{
		Beam.SetPosition(0, start);
		Beam.SetPosition(1, end);
		Beam.SetWidth(width, width);
		foreach(ParticleSystem p in BeamFireEffect)
		{
			p.Emit((int)(Particles*E.curEnergyRatio));
		}
		foreach(ParticleSystem p in BeamHitEffect)
		{
			p.transform.position = end;
		}
	}
}
