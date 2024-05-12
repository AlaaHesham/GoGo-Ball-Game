using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameManagerScript : MonoBehaviour
{

    public GameObject obstaclePrefab;
    public GameObject redPrefab;
    public GameObject greenPrefab;
    public GameObject bluePrefab;
    public GameObject player;

    [SerializeField] TextMeshProUGUI gameOverText;
    [SerializeField] TextMeshProUGUI gameState;

    [SerializeField] GameObject startBtn;
    [SerializeField] GameObject quitBtn;
    [SerializeField] GameObject optionsBtn;


    [SerializeField] GameObject restartBtn;
    [SerializeField] GameObject resumeBtn;
    [SerializeField] GameObject mainMenuBtn;

    [SerializeField] GameObject optionsPanel;
    [SerializeField] TextMeshProUGUI panelText;
    public Toggle muteToggle;

    string guide =
        "Collect orbs to increase your score! \n\n" +
        "* Player Movement: Use A and D for left and right, respectively.\n" +
        "* If you collected 5 orbs of a color, you can transform to this color\n" +
        "* Transform to Red, Green and Blue using J, K and L, respectively\n " +
        "* Each Orb has a special power that can be activated using Space Bar while in the corresponding form:\n" +
        "* Red: Nuke! Destroy all obstacles ahead \n" +
        "* Green: Multiply! Get 5x multiplier for your score \n"+
        "* Blue: Shield! Protect yourself from one obstacle\n\n"+
        "You can use ESC to pause game at any time";

    string credits =
        "Credits \n\n" + 
        "Developed By \n"+
        "  Alaa Hesham\n"+
        "Music\n" +
        "  Intro: PURPLE PLANET\n"+
        "  Orb Collection,\n  Transformation,\n" +
        "  Power-ups:\n    freesound\n" +
        "  Background: pixabay\n"
        ;

    public AudioSource introAudio;
    public AudioSource backgroundAudio;

    bool paused = false;
    public bool gameStarted = false;

    Vector3[] lanesLocations = new Vector3[3];


    // Start is called before the first frame update
    void Start()
    {
        lanesLocations[0] = new Vector3(-3, 1, 100); // left lane
        lanesLocations[1] = new Vector3(0, 1, 100); // center lane
        lanesLocations[2] = new Vector3(3, 1, 100); // right lane

        player.GetComponent<PlayerScript>().Stop();

        gameOverText.text = "";
        panelText.text = guide;
        introAudio.Play();

        muteToggle.onValueChanged.AddListener(delegate {
            ToggleValueChanged(muteToggle);
        });
    }

    void ToggleValueChanged(Toggle change)
    {
        if (muteToggle.isOn)
        {
            introAudio.Stop();

        }
        else
        {
            introAudio.Play();

        }
    }

    // Update is called once per frame
    void Update()

    {
        
        if (gameStarted)
        {
            
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!paused)
                {
                    gameState.text = "Paused";
                    restartBtn.SetActive(true);
                    resumeBtn.SetActive(true);
                    mainMenuBtn.SetActive(true);
                    PauseGame();
                }
                else
                {
                    ResumeGame();
                }
            }
        }
    }

    void PauseGame()
    {
        paused = true;
        backgroundAudio.Pause();
        if (!muteToggle.isOn)
            introAudio.Play();
        // stop generation 
        CancelInvoke();

        // stop movement of orbs and objects

        GameObject[] currentObstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        GameObject[] currentRed = GameObject.FindGameObjectsWithTag("redOrb");
        GameObject[] currentGreen = GameObject.FindGameObjectsWithTag("greenOrb");
        GameObject[] currentBlue = GameObject.FindGameObjectsWithTag("blueOrb");

        foreach (GameObject o in currentObstacles)
        {
            o.GetComponent<MovementScript>().StopMovement();
        }

        foreach (GameObject o in currentRed)
        {
            o.GetComponent<MovementScript>().StopMovement();
        }

        foreach (GameObject o in currentGreen)
        {
            o.GetComponent<MovementScript>().StopMovement();
        }

        foreach (GameObject o in currentBlue)
        {
            o.GetComponent<MovementScript>().StopMovement();
        }

        //stop player movement
        player.GetComponent<PlayerScript>().Stop();
        player.GetComponent<PlayerScript>().HideScores();
    }

    void ResumeGamePart1()
    {
        paused = false;

        if (!muteToggle.isOn)
            backgroundAudio.Play();

        introAudio.Stop();

        gameState.text = "";
        restartBtn.SetActive(false);
        resumeBtn.SetActive(false);
        mainMenuBtn.SetActive(false);

        //regenerate 
        InvokeRepeating("Generate", 1f, 1.2f);

        //resume player movement
        player.GetComponent<PlayerScript>().Resume();
    }
    public void ResumeGame()
    {
        ResumeGamePart1();

        // resume movement of orbs and objects

        GameObject[] currentObstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        GameObject[] currentRed = GameObject.FindGameObjectsWithTag("redOrb");
        GameObject[] currentGreen = GameObject.FindGameObjectsWithTag("greenOrb");
        GameObject[] currentBlue = GameObject.FindGameObjectsWithTag("blueOrb");

        foreach (GameObject o in currentObstacles)
        {
            o.GetComponent<MovementScript>().ResumeMovement();
        }

        foreach (GameObject o in currentRed)
        {
            o.GetComponent<MovementScript>().ResumeMovement();
        }

        foreach (GameObject o in currentGreen)
        {
            o.GetComponent<MovementScript>().ResumeMovement();
        }

        foreach (GameObject o in currentBlue)
        {
            o.GetComponent<MovementScript>().ResumeMovement();
        }
    }

    void Generate()
    {
        //Instantiate(obstaclePrefab, new Vector3(-3, 1, 80), Quaternion.identity);

        int[] locations = new int[3]; // 0 -> empty, 1-> orb , 2 -> obstacle
        int obstacleCounter = 0;
        int emptyCounter = 0;

        for(int i = 0; i < 3; i++) 
        {
            int random = Random.Range(0, 3); // max is execlusive!
            if (random == 0) // empty 
            {
                emptyCounter++;
                if (emptyCounter > 2) //replace with orb
                {
                    locations[i] = 1;
                }
                else
                {
                    locations[i] = random;
                }
            }
            else if (random == 1) // orb
            {
                locations[i] = random;
            }
            else // obstacle
            {
                obstacleCounter++;
                if(obstacleCounter > 2) // we can't put anymore obstacles, so it should be either empty or an orb
                {
                    locations[i] = Random.Range(0,2); // put 0 -> empty or 1 -> orb
                }
                else // it's ok to add an obstacle
                {
                    locations[i] = random;
                }
            }
        }

        //creating the orbs and obstacles
        for(int i = 0; i<locations.Length; i++)
        {
            switch (locations[i])
            {
                case 0: //empty -> don't put anything
                    break;
                case 1: //put an orb.. but which type?
                    GenerateOrb(i);
                    break;
                case 2: //put obstacle
                    //Instantiate(obstaclePrefab, lanesLocations[i], Quaternion.identity);
                    GenerateOpstacle(i);
                    break;
            }
        }


    }

    void GenerateOpstacle(int i)
    {
        GameObject obstacle = ObstaclesPool.SharedInstance.GetPooledObject();
        if (obstacle != null)
        {
            obstacle.transform.position = lanesLocations[i];
            obstacle.transform.rotation = Quaternion.identity;
            obstacle.SetActive(true);
        }
        
    }

    void GenerateOrb(int i)
    {
        int orbType = Random.Range(0, 3); // 0 -> Red, 1 -> Green, 2 -> Blue
        switch (orbType)
        {
            case 0: // red
                //Instantiate(redPrefab, lanesLocations[i], Quaternion.identity);
                GameObject rOrb = RedPool.SharedInstance.GetPooledObject();
                if (rOrb != null)
                {
                    rOrb.transform.position = lanesLocations[i];
                    rOrb.transform.rotation = Quaternion.identity;
                    rOrb.SetActive(true);
                }
                break;
            case 1: // green
                //Instantiate(greenPrefab, lanesLocations[i], Quaternion.identity);
                GameObject gOrb = GreenPool.SharedInstance.GetPooledObject();
                if (gOrb != null)
                {
                    gOrb.transform.position = lanesLocations[i];
                    gOrb.transform.rotation = Quaternion.identity;
                    gOrb.SetActive(true);
                }
                break;
            default: // blue
                //Instantiate(bluePrefab, lanesLocations[i], Quaternion.identity);
                GameObject bOrb = BluePool.SharedInstance.GetPooledObject();
                if (bOrb != null)
                {
                    bOrb.transform.position = lanesLocations[i];
                    bOrb.transform.rotation = Quaternion.identity;
                    bOrb.SetActive(true);
                }
                break;
        }
    }


    public void GameOver()
    {
        gameOverText.text = "Game Over";
        CancelInvoke();
        player.SetActive(false);
        mainMenuBtn.SetActive(true);
        restartBtn.SetActive(true);
        PauseGame();
        player.GetComponent<PlayerScript>().ShowScore();
    }

    public void GameStarted()
    {
        gameStarted = true;
        startBtn.SetActive(false);
        quitBtn.SetActive(false);
        optionsBtn.SetActive(false);
        InvokeRepeating("Generate", 1f, 1.2f);
        player.GetComponent<PlayerScript>().Resume();
        introAudio.Stop();
        if (!muteToggle.isOn)
            backgroundAudio.Play();
    }

    public void QuitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    void DestroyScene()
    {
        GameObject[] currentObstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        GameObject[] currentRed = GameObject.FindGameObjectsWithTag("redOrb");
        GameObject[] currentGreen = GameObject.FindGameObjectsWithTag("greenOrb");
        GameObject[] currentBlue = GameObject.FindGameObjectsWithTag("blueOrb");

        foreach (GameObject o in currentObstacles)
        {
            o.GetComponent<MovementScript>().ResumeMovement();
            o.SetActive(false);
        }

        foreach (GameObject o in currentRed)
        {
            o.GetComponent<MovementScript>().ResumeMovement();
            o.SetActive(false);
        }

        foreach (GameObject o in currentGreen)
        {
            o.GetComponent<MovementScript>().ResumeMovement();
            o.SetActive(false);
        }

        foreach (GameObject o in currentBlue)
        {
            o.GetComponent<MovementScript>().ResumeMovement();
            o.SetActive(false);
        }
    }

    public void RestartGame()
    {
        gameOverText.text = "";
        player.SetActive(true);
        player.GetComponent<PlayerScript>().PlayerRestart();
        ResumeGamePart1();
        DestroyScene();
    }

    public void ShowMainMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ShowOptionsPanel()
    {
        optionsPanel.SetActive(true);

    }

    public void HideOptionsPanel()
    {
        optionsPanel.SetActive(false);
    }

    public void ShowCredits()
    {
        panelText.text = credits;
    }

    public void ShowGuide()
    {
        panelText.text = guide;
    }

}
