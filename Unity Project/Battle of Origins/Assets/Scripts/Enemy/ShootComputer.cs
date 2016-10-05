using UnityEngine;
using System.Collections;

public class ShootComputer : MonoBehaviour
{

	public float TimeBetweenShots = 0.6f;
	public float TimeToFire = 0.4f;
	public GameObject ShotEffect;
	Animator anim;
	float timer;
	bool isFiring = false;
	Character shooter;

	void Awake ()
	{		
		// Set up references.
		this.anim = this.GetComponentInParent<Animator> ();
	}

	void FixedUpdate ()
	{
		// Add the time since Update was last called to the timer.
		this.timer += Time.deltaTime;

		if (this.isFiring && this.timer >= this.TimeToFire) {
			this.Fire ();
		}
	}

	public void Shoot (Character shootingCharacter)
	{	
		this.shooter = shootingCharacter;
		this.anim.SetTrigger ("Shoot");
		// Reset the timer.
		this.timer = 0f;
		this.isFiring = true;
	}

	void Fire(){
		GameObject fireball = Instantiate (ShotEffect, this.transform.position, this.transform.rotation) as GameObject;
		ShootingBehavior script = fireball.GetComponent<ShootingBehavior>();
		script.Shooter = shooter;
		script.Origin = copy(shooter.MyTransform.position);
		script.RotationWhenShooting = copy(shooter.MyTransform.rotation);
		this.isFiring = false;
	}

	private Vector3 copy(Vector3 toCopy){
		return new Vector3(toCopy.x,toCopy.y,toCopy.z);
	}
	private Quaternion copy(Quaternion toCopy){
		return new Quaternion(toCopy.x,toCopy.y,toCopy.z,toCopy.w);
	}
}