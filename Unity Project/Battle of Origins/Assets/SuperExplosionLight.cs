using UnityEngine;
using System.Collections;

public class SuperExplosionLight : MonoBehaviour {

    bool exploding;
    Light light;
	// Use this for initialization
	void Start () {
        exploding = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (exploding)
        {
            light.intensity -= 0.025f;
            if (light.intensity <= 0)
            {
                exploding = false;
            }
        }
	}

    public void flashLight(Vector3 superExplosionOrigin)
    {
        transform.position = superExplosionOrigin;
        light = GetComponent<Light>();
        light.intensity = 8;
        light.enabled = true;
        exploding = true;
    }
}
