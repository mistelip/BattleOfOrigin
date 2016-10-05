using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerSpawner : MonoBehaviour
{

	public bool doEvolution;
	public GameObject[] DarwinistEvolutionMeshes;
	public GameObject[] ReligionistEvolutionMeshes;

    public GameObject religonerPrefabAI;
    public GameObject darwinistPrefabAI;
    public GameObject religonerPrefabPlayer;
    public GameObject darwinistPrefabPlayer;
    Vector3 center = new Vector3(0f, 0f, 0f); //initial position of player
    public float spawningRange = 5f;
    public int nofDarwinistAI;
    public int nofReligonistAI;
    public int nofDarwinistPlayers;
    public int nofReligionistPlayers;
    public bool useKeyboardForPlayerOne;
    public bool friendlyFire;
    public bool beBrainless;
    public List<GameObject> obstaclePrefabs;
    public int nofObstacles = 5;

    //super explosion
    float superExplosionRadius = 100;
    bool slowMotioning;
    float slowMotionTimer;
    float slowMotionDuration = 2f; //times 2.5 for real duration in seconds

    // Use this for initialization
    void Start()
    {
        Model.reset();
		Model.doEvolution = doEvolution;
		//TODO: activate once the meshes are ready
		/*
		if (Model.doEvolution) {
			//Load Darwinist Meshes
			Model.evolutionMeshesD = new Mesh[DarwinistEvolutionMeshes.Length];
			for (int i=0; i<DarwinistEvolutionMeshes.Length; i++) {
				GameObject temp = Instantiate (DarwinistEvolutionMeshes [i]) as GameObject;
				Model.evolutionMeshesD [i] = temp.GetComponentInChildren<MeshFilter> ().mesh;
				Destroy (temp);
			}
			//Load Religionist Meshes
			Model.evolutionMeshesR = new Mesh[ReligionistEvolutionMeshes.Length];
			for (int i=0; i<ReligionistEvolutionMeshes.Length; i++) {
				GameObject temp = Instantiate (ReligionistEvolutionMeshes [i]) as GameObject;
				Model.evolutionMeshesR [i] = temp.GetComponentInChildren<MeshFilter> ().mesh;
				Destroy (temp);
			}
		}
		*/

        Model.initializeObstacles();
        Model.controlsEnabled = false;
        Model.center = center;
        Model.spawningRange = spawningRange;
        ScoreManager.friendlyFire = this.friendlyFire;
        ArtificialIntelligence.beBrainless = this.beBrainless;

        //super explosion
        slowMotioning = false;
        slowMotionTimer = 0;

        //spawnObstacles
        SpawnObstacles(nofObstacles);


        //Logs for input devices
        /*
        string[] devices = Input.GetJoystickNames ();
        foreach (string s in devices) {
            Debug.Log (s);
            Debug.Log ("preconfigured? " + Input.IsJoystickPreconfigured (s));
        }
        //*/

        Model.religonerPrefabAI = religonerPrefabAI;
        Model.religonerPrefabPlayer = religonerPrefabPlayer;
        Model.darwinistPrefabAI = darwinistPrefabAI;
        Model.darwinistPrefabPlayer = darwinistPrefabPlayer;

        //make sure there are at most 4 players
        if (nofDarwinistPlayers > 4)
        {
            nofDarwinistPlayers = 4;
        }
        if (nofDarwinistPlayers + nofReligionistPlayers > 4)
        {
            nofReligionistPlayers = 4 - nofDarwinistPlayers;
        }


        string[] inputPrefixes;
        Race[] races;
        if (Model.fromStartGame)
        {
            Debug.Log("FromStartGame");


            inputPrefixes = Model.inputPrefixes;
            races = Model.initialRacesOfPlayers;

            nofDarwinistPlayers = 0;
            nofReligionistPlayers = 0;
            for (int i = 0; i < races.Length; i++)
            {
                if (races[i] == Race.Darwinist)
                {
                    nofDarwinistPlayers++;
                }
                else
                {
                    nofReligionistPlayers++;
                }
            }
            nofDarwinistAI = Model.initialnofDarwinistAI;
            nofReligonistAI = Model.initialnofReligionistAI;

        }
        else
        {
            Debug.Log("notFromStartGame");
            if (useKeyboardForPlayerOne)
            {
                inputPrefixes = new string[] { "Keyboard", "C1", "C2", "C3" };
            }
            else
            {
                inputPrefixes = new string[] { "C1", "C2", "C3", "C4" };
            }

            races = new Race[nofReligionistPlayers + nofDarwinistPlayers];
            int count = 0;
            for (int i = 0; i < nofDarwinistPlayers; i++)
            {
                races[count] = Race.Darwinist;
                count++;
            }
            for (int i = 0; i < nofReligionistPlayers; i++)
            {
                races[count] = Race.Religionist;
                count++;
            }
        }

        // set number of Players
        ScoreManager.setNumPlayers(nofReligionistPlayers + nofReligonistAI, nofDarwinistPlayers + nofDarwinistAI, nofReligionistPlayers + nofReligonistAI + nofDarwinistPlayers + nofDarwinistAI);

        Model.players = new Character[nofDarwinistPlayers + nofReligionistPlayers];
        Model.nofPlayers = nofDarwinistPlayers + nofReligionistPlayers + nofDarwinistAI + nofReligonistAI;

        //spawn players
        for (int i = 0; i < races.Length; i++)
        {
            GameObject d = Instantiate(races[i] == Race.Darwinist ? darwinistPrefabPlayer : religonerPrefabPlayer, Model.RandomPoint(0), Quaternion.identity) as GameObject;
            createCharacter(d, PlayerType.Player, races[i], inputPrefixes[i]);
        }

        //spawn NPCs
        for (int i = 0; i < nofDarwinistAI; i++)
        {
            GameObject d = Instantiate(darwinistPrefabAI, Model.RandomPoint(0), Quaternion.identity) as GameObject; 
			createCharacter(d, PlayerType.NPC, Race.Darwinist, null);
        }
        for (int i = 0; i < nofReligonistAI; i++)
        {
            GameObject d = Instantiate(religonerPrefabAI, Model.RandomPoint(0), Quaternion.identity) as GameObject;
            createCharacter(d, PlayerType.NPC, Race.Religionist, null);
        }

        //start the game:
        ArtificialIntelligence.initializeTargets();

    }

    private void createCharacter(GameObject go, PlayerType pt, Race r, string inputPrefix)
    {
        Character c = new Character(go, pt, r, inputPrefix);
        c.WonderScript = go.GetComponent<CommonWonder>();
        c.WonderScript.Character = c;
        go.GetComponent<CommonMovement>().Character = c;
        if (pt == PlayerType.Player)
        {
            go.GetComponent<PlayerPray>().Character = c;
            go.GetComponentInChildren<PlayerShoot>().Character = c;
            BodyPartsGrowing bodyPartsGrowing = go.GetComponent<BodyPartsGrowing>();
            if (bodyPartsGrowing)
            {
                bodyPartsGrowing.Character = c;
            }//TODO: remove as soon as Darwinist also has growing script

        }

		if (Model.doEvolution) {
			c.updateMesh();
		}

        Model.add(c);
    }

    public static void ChangeTeam(Character c)
    {
        c.Me.GetComponent<CommonMovement>().StopImmuneAnimation();
        switch (c.Type)
        {
            case PlayerType.Player:
                //PlayerSpawner.ChangeTeamPlayer (c);
                PlayerSpawner.ChangePlayerIntoNextNPC(c);
                break;
            case PlayerType.NPC:
                PlayerSpawner.ChangeTeamNPC(c);
                break;
        }
    }

    private static void ChangeTeamNPC(Character c)
    {
        GameObject newNPC;
        switch (c.Race)
        {
            case Race.Darwinist:
                newNPC = Instantiate(Model.religonerPrefabAI, c.MyTransform.position, c.MyTransform.rotation) as GameObject;
                c.Race = Race.Religionist;
                ScoreManager.ConvertedDarwToRel();
                break;
            default:
                c.Race = Race.Darwinist;
                newNPC = Instantiate(Model.darwinistPrefabAI, c.MyTransform.position, c.MyTransform.rotation) as GameObject;
                ScoreManager.ConvertedRelToDarw();
                break;
        }
        initializeNPC(newNPC, c);
    }

    private void SpawnObstacles(int number)
    {
        for (int i = 0; i < number; i++)
        {
            int obstacleNum = Random.Range(0, obstaclePrefabs.Count);
            Vector3 position = Model.RandomPoint(6f);
            if (position != Model.center)
            {
                Quaternion rot = Quaternion.Euler(0, Random.Range(0, 360), 0);

                GameObject obstacleGO = Instantiate(obstaclePrefabs[obstacleNum], position, rot) as GameObject;
                Obstacle obst = Model.CreateObstacle(obstacleGO);
                Model.obstacles.Add(obst);
            }
        }
    }

    private static void ChangeTeamPlayer(Character c)
    {
        Debug.Log("human player changes team");
        GameObject newPlayer;
        switch (c.Race)
        {
            case Race.Darwinist:
                newPlayer = Instantiate(Model.religonerPrefabPlayer, c.MyTransform.position, c.MyTransform.rotation) as GameObject;
                c.Race = Race.Religionist;
                ScoreManager.ConvertedDarwToRel();
                break;
            default:
                c.Race = Race.Darwinist;
                newPlayer = Instantiate(Model.darwinistPrefabPlayer, c.MyTransform.position, c.MyTransform.rotation) as GameObject;
                ScoreManager.ConvertedRelToDarw();
                break;
        }
        initializePlayer(newPlayer, c);
    }

    private static void ChangePlayerIntoNextNPC(Character c)
    {
        Character npc = ArtificialIntelligence.getRandomNpcPlayer(c.Race);

        if (npc != null)
        {
            Debug.Log("human player takes control over NPC");
            GameObject newPlayer;
            GameObject newNPC;
            switch (c.Race)
            {
                case Race.Darwinist:
                    newPlayer = Instantiate(Model.darwinistPrefabPlayer, npc.MyTransform.position, npc.MyTransform.rotation) as GameObject;
                    newNPC = Instantiate(Model.religonerPrefabAI, c.MyTransform.position, c.MyTransform.rotation) as GameObject;
                    npc.Race = Race.Religionist;
                    ScoreManager.ConvertedDarwToRel();
                    break;
                default:
                    newPlayer = Instantiate(Model.religonerPrefabPlayer, npc.MyTransform.position, npc.MyTransform.rotation) as GameObject;
                    newNPC = Instantiate(Model.darwinistPrefabAI, c.MyTransform.position, c.MyTransform.rotation) as GameObject;
                    npc.Race = Race.Darwinist;
                    ScoreManager.ConvertedRelToDarw();
                    break;
            }

            //for player
            initializePlayer(newPlayer, c);

            //for NPC
            initializeNPC(newNPC, npc);
        }
        else
        {
            //no NPC available;
            PlayerSpawner.ChangeTeamPlayer(c);
        }
    }

    private static void initializePlayer(GameObject newPlayer, Character c)
    {
        Destroy(c.Me);
        c.Me = newPlayer;
        c.MyTransform = c.Me.GetComponent<Transform>();
        c.Me.GetComponent<CommonMovement>().Character = c;
        c.Me.GetComponent<PlayerPray>().Character = c;
        c.Me.GetComponentInChildren<PlayerShoot>().Character = c;
        c.WonderScript = c.Me.GetComponent<CommonWonder>();
        c.WonderScript.Character = c;

		c.updateMesh();

        BodyPartsGrowing bodyPartsGrowing = c.Me.GetComponent<BodyPartsGrowing>();
        if (bodyPartsGrowing)
        {
            bodyPartsGrowing.Character = c;
        }
    }

    private static void initializeNPC(GameObject newNPC, Character c)
    {
        Destroy(c.Me);
        c.Me = newNPC;
        c.MyTransform = c.Me.GetComponent<Transform>();
        c.Me.GetComponent<CommonMovement>().Character = c;
        c.WonderScript = c.Me.GetComponent<CommonWonder>();
        c.WonderScript.Character = c;

		c.updateMesh();
    }

    // Update is called once per frame
    void Update()
    {
        if (slowMotioning)
        {
            slowMotionTimer -= Time.deltaTime;
            if (slowMotionTimer <= 0)
            {
                slowMotioning = false;
                Time.timeScale = 1;
            }
        }
    }

    public void SuperExplosion(Vector3 superExplosionOrigin)
    {
        Debug.Log("SuperExpl Origin: " + superExplosionOrigin);
        superExplosionOrigin.y = 0;

        //Slow motion
        slowMotioning = true;
        slowMotionTimer = slowMotionDuration;
        Time.timeScale = 0.4f;

        //sound
        GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>().PlaySuperExplosion();

        //light
        GameObject.FindGameObjectWithTag("SuperExplosionLight").GetComponent<SuperExplosionLight>().flashLight(superExplosionOrigin + new Vector3(0,4f,0));

        //force
        Collider[] colliders = Physics.OverlapSphere(superExplosionOrigin, superExplosionRadius);
        foreach (Collider hit in colliders)
        {
            Rigidbody targetRB = hit.GetComponentInParent<Rigidbody>();
            if (targetRB)
            {
                CommonMovement commonMovement = hit.GetComponent<CommonMovement>();
                if (commonMovement)
                {
                    float minDistance = 4f;
                    if (Vector3.Distance(hit.transform.position, superExplosionOrigin) < minDistance)
                    {
                        Vector3 diretionV = hit.transform.position - superExplosionOrigin;
                        superExplosionOrigin = superExplosionOrigin - diretionV.normalized * minDistance;
                    }
                    commonMovement.Fall();
                    NavMeshAgent navMeshAgent = hit.GetComponent<NavMeshAgent>();
                    navMeshAgent.enabled = false;
                    float explosionStrength = 600;
                    targetRB.AddExplosionForce(explosionStrength, superExplosionOrigin, superExplosionRadius, 0.5f);
                }
                else
                {
                    Debug.Log("Bad");
                }
            }
        }
        //Camera
		Camera.main.GetComponent<CameraFollow> ().circulateIsland (slowMotionDuration,superExplosionOrigin);
    }

    public void EndGame()
    {
        Application.LoadLevel("SplashScreen");
    }
}
