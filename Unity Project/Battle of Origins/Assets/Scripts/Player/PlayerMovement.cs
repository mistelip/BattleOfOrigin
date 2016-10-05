using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerMovement : CommonMovement
{
	public float Speed = 50;
	public float TurningSpeed = 6f;
	Vector3 movement;                   // The vector to store the direction of the player's movement.
	Vector3 turning;
	Rigidbody playerRigidbody;          // Reference to the player's rigidbody.

	public bool invertAxis = false;

	//TODO: set to 0
	public float tempPlayerSpeedAdvantage = 0;

	//int dummy = 0;

	new void Awake ()
	{
		// Set up references.
		base.Awake ();
		this.playerRigidbody = this.GetComponent<Rigidbody> ();
	}

	void FixedUpdate ()
	{
		if (!Model.controlsEnabled) {
			return;
		}

		base.FixedUpdate ();
		character.TimeSinceLastPray += Time.deltaTime;

		/* 
        //Debugging for explosion impulse

        if (dummy > 0)
        {
            GetComponent<Rigidbody>().AddForce(new Vector3(dummy, 0f, 0f), ForceMode.VelocityChange);
            --dummy;
        }

        if (Input.GetButton(character.InputPrefix + "Fire1"))
        {
            if (dummy == 0)
            {
                dummy = 5;
            }

        }
        // */


		if (character.Mode == PlayingMode.Stunned) {
			fallingTimer += Time.deltaTime;
			if (fallingTimer > character.FallingTimeout) {
				//Debug.Log ("Player going Idle but is still Immune");
				character.Mode = PlayingMode.Idle;
				immuneTimer = 0f;
                
				//put back on his feet and freeze rotations
				Rigidbody rb = GetComponent<Rigidbody> ();
				rb.rotation = Quaternion.Euler (0f, 0f, 0f);
				rb.constraints = RigidbodyConstraints.FreezeRotation;
			} else {
				return;
			}
		} else if (character.IsImmune && character.Mode != PlayingMode.Wonder) {
			immuneTimer += Time.deltaTime;
			if (immuneTimer > character.ImmunityTimeout) {
				//Debug.Log ("Player is not Immune anymore");
				character.setVulnerable ();
			}
		}


		//Cooldown after praying
		if (character.Mode != PlayingMode.Praying && character.TimeSinceLastPray < character.PrayingCooldown) {
			Speed = 0f;
			character.Mode = PlayingMode.Idle;
			//TODO: set animation trigger to not running
			return;
		}

		if (character.CanMove ()) {

			if (character.Mode == PlayingMode.Wonder) {
				Speed = character.WonderSpeed + tempPlayerSpeedAdvantage;
			}

			// Store the input axes.
			float h1 = Input.GetAxisRaw (character.InputPrefix + "Horizontal1");
			float v1 = Input.GetAxisRaw (character.InputPrefix + "Vertical1");

			if (h1 != 0 || v1 != 0) {
				this.Move (h1, v1);
			} else {
				this.Animating (0f, 0f);
			}

			if (character.Mode != PlayingMode.Wonder) {
				if (Input.GetButton (character.InputPrefix + "Pray")) {
					animator.SetBool ("IsRunning", false);
					Speed = 0f;
					character.Mode = PlayingMode.Praying;
                    
					//Debug.Log("player committed to praying");
				} else if (animator.GetBool ("IsRunning")) {
					character.Mode = PlayingMode.Running;
					character.evolveSpeed (Time.deltaTime);//Evolution
					Speed = character.RunningSpeed + tempPlayerSpeedAdvantage;
				} else {
					character.Mode = PlayingMode.Idle;
					Speed = character.RunningSpeed + tempPlayerSpeedAdvantage;
				}
			}

			// Store the input axes.
			float h2 = Input.GetAxisRaw (character.InputPrefix + "Horizontal2");
			float v2 = Input.GetAxisRaw (character.InputPrefix + "Vertical2");

			if (invertAxis) {
				Turn(-h2,-v2);
			} else {
				Turn (h2, v2);
			}
		}

		//NavMesh off == I am currently flying through the air
		if (!navMeshAgent.isActiveAndEnabled) {
			if (transform.position.y < 1) {
				Debug.Log ("Reanabling Navmesh Player");
				navMeshAgent.enabled = true;
				if(navMeshAgent.isActiveAndEnabled  && navMeshAgent.isOnNavMesh){
					navMeshAgent.Resume ();
				}
				else{
					//reset
					Debug.Log("Resume() failed");
					transform.position = Model.RandomPoint(0);
						//new Vector3(Random.Range(-10.0F, 10.0F), 0.0f, Random.Range(-10.0F, 10.0F));
					GetComponent<Rigidbody>().velocity = Vector3.zero;
					transform.rotation = new Quaternion(0, 180, 0, 0);
					Debug.Log("character reseted");
				}
			} else {
				return;
			}
		}
	}

	void Move (float h, float v)
	{
		// Set the movement vector based on the axis input.
		this.movement.Set (h, 0f, v);

		// Normalise the movement vector and make it proportional to the speed per second.
		this.movement = this.movement.normalized * this.Speed * Time.deltaTime;

		// Move the player to it's current position plus the movement.
		this.playerRigidbody.MovePosition (this.transform.position + this.movement);

		// Animate the player.
		this.Animating (h, v);
	}

	void Turn (float h, float v)
	{
		// Set the movement vector based on the axis input.
		if (h != 0 || v != 0) {
			this.turning.Set (h, 0f, v);

			// Normalise the movement vector and make it proportional to the speed per second.
			this.turning = this.turning.normalized * this.TurningSpeed * Time.deltaTime;

			Quaternion newRotation = Quaternion.LookRotation (turning);

			// Move the player to it's current position plus the movement.
			this.playerRigidbody.MoveRotation (newRotation);

			//TODO: don't rotate wonder cylinder
			//Transform wonderCylinder = GameObject.FindGameObjectWithTag ("WonderCylinderMaterial").GetComponent<Transform> ();
			// wonderCylinder.rotation = Quaternion.Slerp(newRotation,wonderCylinder.localRotation, 0);
		}
	}

	void Animating (float h, float v)
	{
		// Create a boolean that is true if either of the input axes is non-zero.
		bool running = h != 0f || v != 0f;

		// Tell the animator whether or not the player is running.
		this.animator.SetBool ("IsRunning", running);
	}


}