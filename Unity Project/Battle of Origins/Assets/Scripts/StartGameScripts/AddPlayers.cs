using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AddPlayers : MonoBehaviour
{

    int nofPlayers;
    float nofRelAI;
    float nofDarAI;
    int[] playerEnabled;
    bool useKeyboard;
    Text[] playerTexts;
    Text nofRelAIText;
    Text nofDarAIText;
    Text keyboardText;
    Text CountdownText;
    bool[] ready;
    float startingDelay = 5f;
    int maxAInum = 50;
    float startingTimer;
    public GameObject DarwinistPrefab;
    public GameObject ReligionistPrefab;
    Vector3[] positions;
    GameObject[] previewCharacters;
    string[] texts;

    Text[] joinText;

    //for inputManaging
    string[] inputPrefixes;
    int nofControllers;
    bool[] hasAlready;

    //Audio
    AudioSource countDownSound;
    AudioManager audioMan;

	//Loading
	Image loadingControlsImage;
	Image loadingBackgroundImage;

    // Use this for initialization
    void Start()
    {
        //for inputManaging
        inputPrefixes = new string[] { "", "", "", "" };
        nofControllers = 0;
        hasAlready = new bool[8];

        playerEnabled = new int[] { 0, 0, 0, 0 };//{ 0, 0, 0, 0 };//=-> disabled, 1 -> darwinist -> 2 religionist
        useKeyboard = false;
        nofPlayers = 0;
        playerTexts = new Text[4];
        ready = new bool[4];
        joinText = new Text[4];

        joinText[0] = GameObject.Find("Join1").GetComponent<Text>();
        joinText[1] = GameObject.Find("Join2").GetComponent<Text>();
        joinText[2] = GameObject.Find("Join3").GetComponent<Text>();
        joinText[3] = GameObject.Find("Join4").GetComponent<Text>();
        playerTexts[0] = GameObject.Find("Player1").GetComponent<Text>();
        playerTexts[1] = GameObject.Find("Player2").GetComponent<Text>();
        playerTexts[2] = GameObject.Find("Player3").GetComponent<Text>();
        playerTexts[3] = GameObject.Find("Player4").GetComponent<Text>();
        nofDarAIText = GameObject.Find("nofDarwinistAI").GetComponent<Text>();
        nofRelAIText = GameObject.Find("nofReligionistAI").GetComponent<Text>();
        keyboardText = GameObject.Find("Keyboard").GetComponent<Text>();
        CountdownText = GameObject.Find("Countdown").GetComponent<Text>();

        positions = new Vector3[4];
        positions[0] = new Vector3(-1f, 2.25f, 0f);
        positions[1] = new Vector3(3f, 2.25f, 0f);
        positions[2] = new Vector3(-1f, -2.5f, 0f);
        positions[3] = new Vector3(3f, -2.5f, 0f);

        previewCharacters = new GameObject[4];

        texts = new string[] { "Disabled", "Darwinist", "Religionist" };
        nofDarAI = 10f;
        nofRelAI = 10f;

        countDownSound = GetComponent<AudioSource>();

        audioMan = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        audioMan.PlayMainSoundtrack();

		loadingControlsImage = GameObject.Find ("LoadingImage").GetComponent<Image> ();
		loadingBackgroundImage = GameObject.Find ("LoadingImageBackground").GetComponent<Image> ();
    }

    // Update is called once per frame
    void Update()
    {
        //for inputManaging (detect new devices)
        FindNewControllers();

        //Keyboard input
        if (Input.GetButtonDown("KeyboardPray") && ! ready[0])
        {

            if (!useKeyboard)
            {
                //insert keyboard player, shift others down
                if (playerEnabled[3] == 0)
                {
                    nofPlayers++;
                }
                for (int i = 3; i >= 1; i--)
                {
                    playerEnabled[i] = playerEnabled[i - 1];
                    ready[i] = ready[i - 1];
                }
                playerEnabled[0] = 1;
                ready[0] = false;
                useKeyboard = true;
            }
            else
            {
                //remove keyboard player, shift others up
                playerEnabled[0] = (playerEnabled[0] + 1) % 3;
                if (playerEnabled[0] == 0)
                {
                    nofPlayers--;
                    useKeyboard = false;
                    for (int i = 0; i < 3; i++)
                    {
                        playerEnabled[i] = playerEnabled[i + 1];
                        ready[i] = ready[i + 1];
                    }
                    playerEnabled[3] = 0;
                    ready[3] = false;
                }
                else if (playerEnabled[0] == 1)
                {
                    nofPlayers++;
                }
            }
        }

        if (useKeyboard && Input.GetButtonDown("KeyboardFire1"))
        {
            ready[0] = !ready[0];
        }

        //addPlayer
        ChangeNofDarAI();
        ChangeNofRelAI();

        //controller input
        int iCor = useKeyboard ? 1 : 0;
        for (int i = 0; i < Mathf.Min(4 - iCor, nofControllers); i++)
        {//accept 3 controllers when keyboard is selected
            if (Input.GetButtonDown(inputPrefixes[i] + "Pray") && !ready[i + iCor]/* && (nofPlayers > 1 ||  playerEnabled[i+iCor] == 0)*/)
            {
                playerEnabled[i + iCor] = (playerEnabled[i + iCor] + 1) % 3;
                if (playerEnabled[i + iCor] == 0)
                {
                    nofPlayers--;
                }
                else if (playerEnabled[i + iCor] == 1)
                {
                    nofPlayers++;
                }
            }
            if (playerEnabled[i + iCor] != 0 && Input.GetButtonDown(inputPrefixes[i] + "Fire1"))
            {
                ready[i + iCor] = !ready[i + iCor];
            }
        }

        //update UI
        updateDisplay();
        updatePreviews();

        //check if game can be started
        CanStartGame();
    }

    void FindNewControllers()
    {
        for (int i = 0; i < 8; i++)
        {
            if (Input.GetButtonDown("C" + (i + 1) + "Pray") || Input.GetButtonDown("C" + (i + 1) + "Fire1"))
            {
                if (nofControllers < 4 && !hasAlready[i])
                {
                    inputPrefixes[nofControllers] = "C" + (i + 1);
                    nofControllers++;
                    hasAlready[i] = true;
                    Debug.Log("Controller " + (i + 1) + " added as Joystick " + nofControllers);
                }
            }
        }
    }

    void updateDisplay()
    {
        if (useKeyboard)
        {
            keyboardText.text = "Keyboard";
        }
        else
        {
            keyboardText.text = "";//joystick is default
        }

        for (int i = 0; i < 4; i++)
        {
            string text;
            if (ready[i])
            {
                text = "Ready";
                joinText[i].text = "";
            }
            else
            {
                text = texts[playerEnabled[i]];
                if (playerEnabled[i] == 0)
                {
                    joinText[i].text = "Choose Team by pressing B/" + "\u20dd";
                }
                else
                {
                    joinText[i].text = "Press RB/R1 when Ready"; ;
                }
            }
            playerTexts[i].text = text;


        }

        nofDarAIText.text = "Darwinist NPCs: " + ((int)nofDarAI);
        nofRelAIText.text = "Religionist NPCs: " + ((int)nofRelAI);
    }

    void updatePreviews()
    {
        for (int i = 0; i < 4; i++)
        {
            if (playerEnabled[i] == 0 && previewCharacters[i] != null)
            {
                GameObject.Destroy(previewCharacters[i]);
            }
            else if (playerEnabled[i] == 1 && (previewCharacters[i] == null || !previewCharacters[i].tag.Equals(texts[playerEnabled[i]])))
            {
                if (previewCharacters[i] != null)
                {
                    Destroy(previewCharacters[i]);
                }
                previewCharacters[i] = Instantiate(DarwinistPrefab, positions[i], Quaternion.Euler(0, 0, 0)) as GameObject;
                audioMan.PlayDarwPicked();
            }
            else if (playerEnabled[i] == 2 && (previewCharacters[i] == null || !previewCharacters[i].tag.Equals(texts[playerEnabled[i]])))
            {
                if (previewCharacters[i] != null)
                {
                    Destroy(previewCharacters[i]);
                }
                previewCharacters[i] = Instantiate(ReligionistPrefab, positions[i], Quaternion.Euler(0, 180, 0)) as GameObject;
                audioMan.PlayRelPicked();
            }
        }
    }

    void ChangeNofRelAI()
    {
        if (!ready[0])
        {
            float addAI = 0;
            if (useKeyboard)
            {
                addAI = (float)Input.GetAxisRaw("KeyboardVertical2") / 3f;
            }
            else if (nofControllers > 0)
            {
                addAI = Input.GetAxisRaw(inputPrefixes[0] + "Vertical2") / 3f;
            }

            nofRelAI += addAI;

            if (nofRelAI > maxAInum)
            {
                nofRelAI = maxAInum;
            }
            else if (nofRelAI < 0)
            {
                nofRelAI = 0f;
            }
        }
    }

    void ChangeNofDarAI()
    {
        if (!ready[0])
        {
            float addAI = 0;
            if (useKeyboard)
            {
                addAI = (float)Input.GetAxisRaw("KeyboardVertical1") / 3f;
            }
            else if (nofControllers > 0)
            {
                addAI = Input.GetAxisRaw(inputPrefixes[0] + "Vertical1") / 3f;
            }

            nofDarAI += addAI;

            if (nofDarAI > maxAInum)
            {
                nofDarAI = maxAInum;
            }
            else if (nofDarAI < 0)
            {
                nofDarAI = 0f;
            }
        }
    }

    void CanStartGame()
    {
        if (nofPlayers > 0)
        {
            bool allPlayersReady = true;
            for (int i = 0; i < 4; i++)
            {
                if (playerEnabled[i] != 0 && !ready[i])
                {
                    allPlayersReady = false;
                }
            }
            if (allPlayersReady)
            {
				loadingBackgroundImage.enabled = true;
				loadingControlsImage.enabled = true;
                if (!countDownSound.isPlaying)
                {
                    countDownSound.Play();
                }

                startingTimer -= Time.deltaTime;
                if (startingTimer < 0)
                {
                    CountdownText.text = "Loading Game...";
                }
                else
                {
                    CountdownText.text = "Start Game in: " + (int)(startingTimer + 1);
                }
            }
            else
            {
				loadingBackgroundImage.enabled = false;
				loadingControlsImage.enabled = false;
                if (countDownSound.isPlaying)
                {
                    countDownSound.Stop();
                }

                startingTimer = startingDelay;
                CountdownText.text = "";
            }
            if (startingTimer < 0)
            {
                startGame();
                startingTimer = 100;
                //avoid to start game very often in case the game is not properly started
            }
        }
    }

    void startGame()
    {
        //Debug.Log("Start Game");
        //tel other scene that the game was started through the startGame scene
        Model.fromStartGame = true;

        //set the joystick to player mapping for other scene
        if (useKeyboard)
        {
            Model.inputPrefixes = new string[] {
				"Keyboard",
				inputPrefixes [0],
				inputPrefixes [1],
				inputPrefixes [2]
			};
        }
        else
        {
            Model.inputPrefixes = inputPrefixes;
        }

        //set initial races of players for other scene
        Model.initialRacesOfPlayers = new Race[nofPlayers];

        Model.initialnofDarwinistAI = (int)nofDarAI;
        Model.initialnofReligionistAI = (int)nofRelAI;

        int count = 0;
        for (int i = 0; i < 4; i++)
        {
            if (playerEnabled[i] != 0)
            {
                audioMan.StopMainSountrack();
                Model.initialRacesOfPlayers[count] = playerEnabled[i] == 1 ? Race.Darwinist : Race.Religionist;
                count++;
            }
        }

        //start other scene
        Application.LoadLevel("BattleOfOriginsScene01");
    }


}
