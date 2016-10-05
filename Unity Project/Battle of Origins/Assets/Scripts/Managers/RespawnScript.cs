using UnityEngine;
using System.Collections;

public class RespawnScript : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /*
    void OnParticleCollision(GameObject other)
    {
        Debug.Log("respawning " + other.tag);
        other.transform.position = new Vector3(Random.Range(-10.0F, 10.0F), 0.0f, Random.Range(-10.0F, 10.0F));
        other.GetComponent<Rigidbody>().velocity = Vector3.zero;
        other.transform.rotation = new Quaternion(0, 180, 0, 0);
        EnemyMovement em = other.GetComponent<EnemyMovement>();
        if (em)
        {
            ArtificialIntelligence.findNewTarget(em.Character);
        }
    }
    */
    //*
    void OnTriggerEnter(Collider other)
    {

        //Ignore Collider with trigger enabled. This is only for the wonder
        if (other.isTrigger)
        {
            return;
        }
       // Debug.Log("Touched Water. Respawning " + other.tag);
		other.gameObject.transform.position = Model.RandomPoint (0);
			//new Vector3(Random.Range(-10.0F, 10.0F), 0.0f, Random.Range(-10.0F, 10.0F));
        other.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        other.gameObject.transform.rotation = new Quaternion(0, 180, 0, 0);
        EnemyMovement em = other.gameObject.GetComponent<EnemyMovement>();
        if (em)
        {
            ArtificialIntelligence.findNewTarget(em.Character);
        }
    }
    //*/
}
