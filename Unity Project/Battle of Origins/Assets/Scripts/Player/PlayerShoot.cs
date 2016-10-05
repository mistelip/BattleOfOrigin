using UnityEngine;
using System.Collections;

public class PlayerShoot : MonoBehaviour
{

	public float TimeBetweenShots;
	public float TimeToFire;
	public GameObject ShotEffect;
	Animator anim;
	float timer;
	bool isFiring = false;
	AudioManager audioManager;
	Character character;

	void Awake ()
	{		
		// Set up references.
		this.anim = this.GetComponentInParent<Animator> ();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
	}
	
	void FixedUpdate ()
	{
		if (!Model.controlsEnabled) {
			return;
		}
		// Add the time since Update was last called to the timer.
		this.timer += Time.deltaTime;
		
		// If the Fire1 button is being press and it's time to fire...
		// and not stunned
		// and no active wonder
		if (Input.GetButton (character.InputPrefix + "Fire1") && this.timer >= this.TimeBetweenShots && (character.Mode != PlayingMode.Stunned) && character.Mode != PlayingMode.Wonder && character.Mode != PlayingMode.Praying) {
			//if not in praying cooldown
			if (character.TimeSinceLastPray >= character.PrayingCooldown) {
				this.Shoot ();
			}
		}

		if (this.isFiring && this.timer >= this.TimeToFire) {
			this.Fire ();
		}
	}
	
	public void Shoot ()
	{				
		this.anim.SetTrigger ("Shoot");
		// Reset the timer.
		this.timer = 0f;
		this.isFiring = true;
	}
	
	void Fire ()
	{
        audioManager.PlayWhoosh();
		GameObject fireball = Instantiate (ShotEffect, this.transform.position, this.transform.rotation) as GameObject;
		ShootingBehavior script = fireball.GetComponent<ShootingBehavior> ();
		script.Shooter = character;
		script.Origin = copy (character.MyTransform.position);
		script.RotationWhenShooting = copy (character.MyTransform.rotation);
		this.isFiring = false;
	}
	
	private Vector3 copy (Vector3 toCopy)
	{
		return new Vector3 (toCopy.x, toCopy.y, toCopy.z);
	}

	private Quaternion copy (Quaternion toCopy)
	{
		return new Quaternion (toCopy.x, toCopy.y, toCopy.z, toCopy.w);
	}

	//Properties
	public Character Character {
		get {
			return character;
		}
		set {
			character = value;
		}
	}
}
