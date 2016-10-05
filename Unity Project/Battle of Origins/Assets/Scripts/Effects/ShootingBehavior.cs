using UnityEngine;
using System.Collections;

public class ShootingBehavior : MonoBehaviour
{
    //ParticleCollisionEvent[] collisionEvents;
    float timer;
    float lifeSpan = 0.7f;
   
    float explosionRadius = 10f;
    AudioManager audioManager;
    Vector3 origin;
    Quaternion rotationWhenShooting;
    Character shooter;
	Transform throwObjectTransform;
	Transform throwObjectTransformChild;
	Vector3 randomRotation;
	ParticleSystem shot;

    void Start()
    {
        this.timer = 0;
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
		randomRotation = new Vector3 (Random.Range (0.0f, 1.0f), Random.Range (0.0f, 1.0f), Random.Range (0.0f, 1.0f));
		shot = this.gameObject.GetComponent<ParticleSystem> ();
		throwObjectTransformChild = GetComponentInChildren<MeshRenderer> ().GetComponentInParent<Transform> ();
		throwObjectTransform = throwObjectTransformChild.GetComponentInParent<Transform>();
    }

    void FixedUpdate()
    {
        timer += Time.deltaTime;    
        if (timer > lifeSpan ) 
        {
            Destroy(this.gameObject);
        }
		throwObjectTransformChild.Rotate(randomRotation * 7.0f);
    }

	void Update()
	{
		ParticleSystem.Particle[] particleList = new ParticleSystem.Particle[shot.particleCount];
		var numberOfParicles = shot.GetParticles (particleList);
		if (numberOfParicles > 0)
		{
			throwObjectTransform.position = shot.transform.position + 
				(shot.transform.rotation * (particleList [0].position + (particleList[0].position.normalized * 2.0f)));
		}		
	}

    void OnParticleCollision(GameObject other)
    {
        Vector3 enemyPosition = other.transform.position;
        Vector3 shootingDirection = enemyPosition - this.transform.position;
        Vector3 explosionOrigin = enemyPosition - (shootingDirection.normalized*0.1f);
        //explosionOrigin.y = 1;
       // GameObject.FindGameObjectWithTag("ExplosionOrigin").transform.position = explosionOrigin;

        explode(explosionOrigin);


    }

    private void explode(Vector3 explosionOrigin)
    {
        Collider[] colliders = Physics.OverlapSphere(explosionOrigin, explosionRadius);
        string myTag = this.tag;
        audioManager.PlayExplosion();
  

        //Debug.Log("num of Colliders" + colliders.Length);
        foreach (Collider hit in colliders)
        {
            string targetTag = hit.tag;

            //No Friendly fire
            //*
            if (!ScoreManager.friendlyFire && myTag == targetTag)
            {
                //Debug.Log("Friendly Fire " + ScoreManager.friendlyFire);
                continue;
            }
            //*/

            Rigidbody targetRB = hit.GetComponentInParent<Rigidbody>();
            if (targetRB)
            {

                //notify hit character
                //works for both NPC and human player
                CommonMovement commonMovement = hit.GetComponent<CommonMovement>();
                if (commonMovement)
                {
                    //*
                    if (commonMovement.Character.IsImmune)
                    {
						if (commonMovement.Character.Mode == PlayingMode.Wonder)
						{
							targetRB.AddForce(this.transform.forward.x * 500, 0.0f, this.transform.forward.z * 500);

							if (shooter != null)
							{
								shooter.evolveShoot();//Evolution
							}
						}
                        //No hit if target is currently immune or already hit
                        return;
                    }
                    // */

                    //targetRB.AddRelativeForce(explosionForce,ForceMode.Acceleration);
                    /*  Debug.Log("myPos = " + this.transform.position
                          + "\ntargetPos = " + enemyPosition
                          + "\nHit = " + hit.transform.position
                          + "\nexplOr = " + explosionOrigin
                          + "\nforce = " + explosionForce + " (mag = " + explosionForce.magnitude + ")");
                     */

                    float minDistance = 1f;
                    if (Vector3.Distance(hit.transform.position, explosionOrigin) < minDistance)
                    {
                        // Debug.Log("Distance too short " + Vector3.Distance(hit.transform.position, explosionOrigin));
                        Vector3 diretionV = hit.transform.position - explosionOrigin;
                        explosionOrigin = explosionOrigin - diretionV.normalized * minDistance;
                        //explosionOrigin.y = 1;
                        // Debug.Log("New Distance  " + Vector3.Distance(hit.transform.position, explosionOrigin));
                        //GameObject.FindGameObjectWithTag("ExplosionOrigin2").transform.position = explosionOrigin;
                    }
                    // explosionOrigin.y = 1;

					if (shooter != null)
					{
						shooter.evolveShoot();//Evolution
					}

                    float explosionStrength = 600;
					if(commonMovement.Character.Mode == PlayingMode.Praying){
						explosionStrength += 200;
						//Debug.Log ("increased vulnerability during praying");
					}

					if(Model.doEvolution){
						explosionStrength *= shooter.AttackStrength;
					}

					commonMovement.Fall();
                    NavMeshAgent navMeshAgent = hit.GetComponent<NavMeshAgent>();
                    navMeshAgent.enabled = false;
                    targetRB.AddExplosionForce(explosionStrength, explosionOrigin, explosionRadius, 0.11f);

                }
                else
                {
                    Debug.Log("Bad");
                }

                //Score keeping
                if (targetTag == "Religionist")
                {
                    ScoreManager.HitReligioner();
                }
                else
                {
                    ScoreManager.HitDarwinist();
                }

                if (shooter != null)
                {
                    shooter.evolveShoot();//Evolution
                }
            }
        }
    }

	//we don't use that anymore. 
    //P: So should we like erase it?
    IEnumerator hitAnimation(Transform rbTransform)
    {
        rbTransform.localScale /= 2;
        yield return new WaitForSeconds(2);
        if (rbTransform != null)
        {
            rbTransform.localScale *= 2;
        }

    }

    //Properties

    public Character Shooter
    {
        get
        {
            return this.shooter;
        }
        set
        {
            shooter = value;
        }
    }

    public Quaternion RotationWhenShooting
    {
        get
        {
            return rotationWhenShooting;
        }
        set
        {
            rotationWhenShooting = value;
        }
    }

    public Vector3 Origin
    {
        get
        {
            return origin;
        }
        set
        {
            origin = value;
        }
    }
}
