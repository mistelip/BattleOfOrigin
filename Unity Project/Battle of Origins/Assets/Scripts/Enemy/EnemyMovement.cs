using UnityEngine;
using System.Collections.Generic;

public class EnemyMovement : CommonMovement
{
	public GameObject WonderEnergyReligioner;
	public GameObject wonderEnergyDarwinist;
    float shootingTimer;
    ShootComputer shootController;
    float timeSinceLastEvent = 0f;
    float timeUntilNewTargetIsChosen = 5f;

    float inAirTimer, maxInAirTime = 10f;

    new void Awake()
    {
        base.Awake();
        this.shootController = this.GetComponentInChildren<ShootComputer>();
        inAirTimer = 0;
    }

    protected void Update()
    {
        //Check if he is stuck in the air (or on a building)
        if (this.transform.position.y > 1)
        {
            //Debug.Log("Flying: " + this.transform.position.y + " time: " + inAirTimer);
            inAirTimer += Time.deltaTime;
        }
        else
        {
            inAirTimer = 0;
        }

        //Reset after max time has passed
        if (inAirTimer > maxInAirTime)
        {
            Debug.Log("in air for too long (" + inAirTimer + "s), resetig character");
            inAirTimer = 0;
			transform.position = Model.RandomPoint(0);
				//new Vector3(Random.Range(-10.0F, 10.0F), 0.0f, Random.Range(-10.0F, 10.0F));
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            transform.rotation = new Quaternion(0, 180, 0, 0);
            ArtificialIntelligence.findNewTarget(this.character);
            Debug.Log("assigned new target");
        }
    }
    protected void FixedUpdate()
    {
		if (!Model.controlsEnabled) {
			//animator.SetTrigger("Idle");
			return;
		}
        base.FixedUpdate();
        timeSinceLastEvent += Time.deltaTime;
        character.TimeSinceLastPray += Time.deltaTime;


        if (timeSinceLastEvent > timeUntilNewTargetIsChosen || (character.Target != null && Model.isHeCurrentlyPossessingAWonder(character.Target)))
        {
            //choose  a new target after some time
            //or if the target has the wonder
            ArtificialIntelligence.findNewTarget(character);
			timeSinceLastEvent = 0f;
        }

        if (character.Mode == PlayingMode.Stunned)
        {//falling
			animator.SetBool("IsPraying", false);
            fallingTimer += Time.deltaTime;
            if (fallingTimer > character.FallingTimeout)
            {
                //Debug.Log ("NPC going Idle but is still Immune");
                character.Mode = PlayingMode.Idle;
                ArtificialIntelligence.findNewTarget(character);
                timeSinceLastEvent = 0f;
                immuneTimer = 0f;

                //put back on his feet and freeze rotations
                Rigidbody rb = GetComponent<Rigidbody>();
                rb.rotation = Quaternion.Euler(0f, 0f, 0f);
                rb.constraints = RigidbodyConstraints.FreezeRotation;
            }
            else
            {
                return;
            }
        }
        else if (character.IsImmune && character.Mode != PlayingMode.Wonder)
        {//immune
            immuneTimer += Time.deltaTime;
            if (immuneTimer > character.ImmunityTimeout)
            {
                //	Debug.Log ("NPC is not Immune anymore");
                character.setVulnerable();
            }
        }
		if (character.Mode != PlayingMode.Praying)
		{
			animator.SetBool("IsPraying", false);
			if (this.createWonderIndicator.isPlaying){
				this.createWonderIndicator.Stop(true);
			}
		}

        //NavMesh off == I am currently flying through the air
        if (!navMeshAgent.isActiveAndEnabled)
        {
            if (transform.position.y < 1)
            {
                // Debug.Log("Reanabling Navmesh NPC");

                NavMeshHit hit;
                NavMesh.SamplePosition(transform.position, out hit, 2f, NavMesh.AllAreas);

                /*if (!Vector3.Equals(this.transform.position,hit.position)){
                    Debug.Log("Repositioned from " + this.transform.position + " to " + hit.position);
                }*/

                if (Vector3.Equals(hit.position, new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity)))
                {
                    //Guy is over the ocean and will be respawned soon

                    //Debug.Log("got infinity. Seed: " + this.transform.position);
                    return;
                }

                this.transform.position = hit.position;

                navMeshAgent.enabled = true;
                navMeshAgent.Resume();
            }
            else
            {
                return;
            }
        }

        if (character.Mode == PlayingMode.Wonder)
        {// I have the wonder
            //Debug.Log("Playing mode is wonder");
            if (character.Target == null || character.Target.Race == character.Race)
            {
                ArtificialIntelligence.findNewEnemyTarget(character,true);
                //Debug.Log ("find new enemy target");
            }
            if (character.Target == null)//no suitable enemy left
                return;
			if(navMeshAgent.isActiveAndEnabled && navMeshAgent.isOnNavMesh){
            	navMeshAgent.SetDestination(character.Target.MyTransform.position);
            	navMeshAgent.speed = character.WonderSpeed;
			}
			else{
				//reset
				Debug.Log("SetDestination() failed");
				transform.position = Model.RandomPoint(0);
				//new Vector3(Random.Range(-10.0F, 10.0F), 0.0f, Random.Range(-10.0F, 10.0F));
				GetComponent<Rigidbody>().velocity = Vector3.zero;
				transform.rotation = new Quaternion(0, 180, 0, 0);
				ArtificialIntelligence.findNewTarget(this.character);
				Debug.Log("assigned new target");
			}
			return;
        }

        //Cooldown after praying
        if (character.Mode != PlayingMode.Praying && character.TimeSinceLastPray < character.PrayingCooldown)
        {
            navMeshAgent.speed = 0f;
            character.Mode = PlayingMode.Idle;
            //TODO: set animation trigger to not running
            return;
        }

        Character iMustFleeFrom = Model.whomDoINeedToFleeFrom(character);
        if (iMustFleeFrom != null)
        {//i need to flee
            if (navMeshAgent.isActiveAndEnabled)
            {
                //Debug.Log("Fleeing");
				if(navMeshAgent.isActiveAndEnabled && navMeshAgent.isOnNavMesh){
                	navMeshAgent.SetDestination(character.MyTransform.position + 2 * (character.MyTransform.position - iMustFleeFrom.MyTransform.position));
                	navMeshAgent.speed = character.RunningSpeed;
				}
				else{
					//reset
					Debug.Log("SetDestination() failed");
					transform.position = Model.RandomPoint(0);
						//new Vector3(Random.Range(-10.0F, 10.0F), 0.0f, Random.Range(-10.0F, 10.0F));
					GetComponent<Rigidbody>().velocity = Vector3.zero;
					transform.rotation = new Quaternion(0, 180, 0, 0);
					ArtificialIntelligence.findNewTarget(this.character);
					Debug.Log("assigned new target");
				}
				character.evolveSpeed(Time.deltaTime);//Evolution
            }

        }
        else
        {// no close enemy has the wonder

            Character closestHumanPrayingPartner = Model.getClosestHumanPrayingPartner(character);
            if (closestHumanPrayingPartner != null)
            {
                if ((closestHumanPrayingPartner.MyTransform.position - character.MyTransform.position).magnitude < character.PrayingWithHumanDistance)
                {
                    //the closest human player that wants to pray is actually close
                    character.Target = closestHumanPrayingPartner;
                }
            }

			if(Model.isWonderOwnedBy(character.Race) && (character.Target == null || character.Target.Race == character.Race)){
				ArtificialIntelligence.findNewEnemyTarget(character,false);
			}

            if (character.CanMove() && character.Target == null)
            {//no targert -> find new one
                ArtificialIntelligence.findNewTarget(character);
				//Debug.Log("no target -> find new target");
            }

            if (character.CanMove() && character.Target != null)
            {//have target -> interact with it
                //Debug.Log ("CanMove and have target");
                // ... set the destination of the nav mesh agent to the target.

				if(navMeshAgent.isActiveAndEnabled && navMeshAgent.isOnNavMesh){
					navMeshAgent.SetDestination(character.Target.MyTransform.position);
				}
				else{
					//reset
					Debug.Log("SetDestination() failed");
					transform.position = Model.RandomPoint(0);
						new Vector3(Random.Range(-10.0F, 10.0F), 0.0f, Random.Range(-10.0F, 10.0F));
					GetComponent<Rigidbody>().velocity = Vector3.zero;
					transform.rotation = new Quaternion(0, 180, 0, 0);
					ArtificialIntelligence.findNewTarget(this.character);
					Debug.Log("assigned new target");
				}
				
				shootingTimer += Time.deltaTime;

                if (character.closeEnough(character.Target))
                {//close
                    timeSinceLastEvent = 0f;
                    if (character.Target.Race == character.Race)
                    {//Friendly Target
                        //Debug.Log ("am close enough to pray");
                        navMeshAgent.speed = 0f;
                        character.Mode = PlayingMode.Praying;
                        if (character.Target.Mode == PlayingMode.Praying && navMeshAgent.isOnNavMesh)
                        {
                            //Debug.Log ("am praying");
                            ScoreManager.CreateWonder(character.Race,character.PrayingAbility);
                            character.evolvePray(Time.deltaTime);//Evolution
                            character.TimeSinceLastPray = 0f;
							if (!this.createWonderIndicator.isPlaying){
								this.createWonderIndicator.Play();
							}
							if (shootingTimer > character.TimeBetweenBullets){
								if (this.character.Race == Race.Religionist){
									Instantiate( WonderEnergyReligioner, this.transform.position, this.transform.rotation);
								}
								else{
									Instantiate( wonderEnergyDarwinist, this.transform.position, this.transform.rotation);
								}
								shootingTimer = 0.0f;
							}
							animator.SetBool("IsPraying", true);
                        }
						else{
							animator.SetBool("IsPraying", false);
							if (this.createWonderIndicator.isPlaying){
								this.createWonderIndicator.Stop(true);
							}
						}
                    }
                    else
                    {//Enemy Target
                        character.Mode = PlayingMode.Attacking;
                        navMeshAgent.speed = character.WalkingSpeed;
                        if (shootingTimer >= character.TimeBetweenBullets)
                        {
                            Shoot();
                        }
                    }
                }
                else
                {// I am far away from my target so I run there
                    character.Mode = PlayingMode.Running;
                    navMeshAgent.speed = character.RunningSpeed;
                    character.evolveSpeed(Time.deltaTime);//Evolution
                }


            }
            else
            { //I either cannot move or still have no target for some reason (happens if there are too few players and maybe at startup of the game)
                character.Mode = PlayingMode.Idle;
                //this.animator.SetTrigger ("Idle");
                //Don't evolve
            }
        }
    }

    void Shoot()
    {
        // Reset the timer.
        shootingTimer = 0f;
        this.shootController.Shoot(character);
    }

    new public void Fall()
    {
        base.Fall();
        this.navMeshAgent.speed = 0f;
    }

}