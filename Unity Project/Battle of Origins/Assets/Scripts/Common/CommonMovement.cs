/*
 * Force	        - Add a continuous force to the rigidbody, using its mass.
 * Acceleration	    - Add a continuous acceleration to the rigidbody, ignoring its mass.
 * Impulse	        - Add an instant force impulse to the rigidbody, using its mass.
 * VelocityChange	- Add an instant velocity change to the rigidbody, ignoring its mass. 
 */

using UnityEngine;
using System.Collections;

public class CommonMovement : MonoBehaviour
{

	public Material Red;

    protected Animator animator;
    protected float fallingTimer, immuneTimer;
    protected Character character;
    //int forceSteps = 0;
    protected NavMeshAgent navMeshAgent;               // Reference to the nav mesh agent.
	protected ParticleSystem createWonderIndicator;
    AudioManager audioManager;
	Light immuneLight;
	bool isImmunAnimationRunning;

    protected void Awake()
    {
        this.animator = this.GetComponent<Animator>();
        this.navMeshAgent = this.GetComponent<NavMeshAgent>();
		this.createWonderIndicator = GetComponentInChildren<ParticleSystem> ();
		this.createWonderIndicator.Stop ();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
		immuneLight = this.GetComponentInChildren<Light> ();
		isImmunAnimationRunning = false;
    }

    // Update is called once per frame
    protected void Update()
    {
    }

    protected void FixedUpdate()
    {
		if (character.IsImmune) {
			if (!isImmunAnimationRunning) {
				StartCoroutine( immuneAnimation ());
			}
		} else {
			if(isImmunAnimationRunning){
				StopImmuneAnimation();
			}
		}
    }

    public void Fall()
    {
		this.createWonderIndicator.Stop(true);
        this.character.Mode = PlayingMode.Stunned;
        character.evolveResistance();//Evolution
        this.fallingTimer = 0f;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

        if (character.Race == Race.Religionist)
         {
             audioManager.PlayReligPain();
             //current falling animation: 1.067s
            // animator.speed = 1 / Mathf.Pow(expForce.magnitude, 0.1f);
         }
         else
         {
             audioManager.PlayDarwPain();
            // animator.speed = 1 / Mathf.Pow(expForce.magnitude, 0.1f);
         }
         
        this.animator.SetTrigger("Fall");
    }

    public Character Character
    {
        get
        {
            return character;
        }
        set
        {
            character = value;
        }
    }

	public void StopImmuneAnimation(){
		StopCoroutine(immuneAnimation());
		isImmunAnimationRunning = false;
	}

    protected IEnumerator immuneAnimation()
    {
		isImmunAnimationRunning = true;
        //TODO: Reset Animation speed
        //http://forum.unity3d.com/threads/mecanim-change-animation-speed-of-specific-animation-or-layers.160395/page-2
        this.GetComponent<Animator>().speed = 1f;

        if (character.Race == Race.Darwinist)
        {
            //TODO: Different immunity animation
        }
        else
        {

        }

        while (character.IsImmune)
        {
            if (this.immuneLight != null)
            {
				this.immuneLight.enabled = !this.immuneLight.enabled;
            }
            yield return new WaitForSeconds(0.2f);
        }

        if (immuneLight != null)
        {
			this.immuneLight.enabled = false;
        }
    }
}
