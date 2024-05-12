using TMPro;
using UnityEngine;

enum PlayerState
{
    White,
    Red,
    Green,
    Blue
}

public class PlayerScript : MonoBehaviour
{

    [SerializeField] Rigidbody rb;
    [SerializeField] GameObject shield;
    [SerializeField] GameObject multiplier;
    [SerializeField] GameManagerScript gameManagerS;

    float speed;
    [SerializeField] TextMeshProUGUI redScoreText;
    [SerializeField] TextMeshProUGUI greenScoreText;
    [SerializeField] TextMeshProUGUI blueScoreText;
    [SerializeField] TextMeshProUGUI scoreText;

    int redScore = 0;
    int greenScore = 0;
    int blueScore = 0;
    int score = 0;

    [SerializeField] Material defaultMat;
    [SerializeField] Material redMat;
    [SerializeField] Material greenMat;
    [SerializeField] Material blueMat;


    PlayerState currentState;

    int scoreMultiplier;

    bool gamePaused = false;

    public AudioSource orbCollectedAudio;
    public AudioSource errorAudio;
    public AudioSource transformAudio;
    public AudioSource transformLossAudio;
    public AudioSource obstacleHitAudio;
    public AudioSource nukeAudio;
    public AudioSource shieldAudio;
    public AudioSource multiplyAudio;
    // Start is called before the first frame update
    void Start()
    {
        currentState = PlayerState.White;

        speed = 5f;
        HideScores();

        setMultiplier(false);
    }

    public void HideScores()
    {
        scoreText.text = "";
        redScoreText.text = "";
        greenScoreText.text = "";
        blueScoreText.text = "";
    }

    public void ShowScore()
    {
        scoreText.text = score.ToString();
    }

    public void Stop()
    {
        gamePaused = true;
    }

    public void Resume()
    {
        gamePaused = false;
    }


    // Update is called once per frame
    private void Update()
    {
        if (!gamePaused)
        {
            //moving player
            if (!(Input.GetAxis("Horizontal") > 0 && transform.position.x > 3.5)
                && !(Input.GetAxis("Horizontal") < 0 && transform.position.x < -3.5))
                transform.position += new Vector3(Input.GetAxis("Horizontal") * speed, 0, 0) * Time.deltaTime;

            // updating screen
            UpdateScreen();

            //transformig player
            PlayerTrans();

            //power ups
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (currentState == PlayerState.Red && redScore > 0)
                    RedPowerUp();
                else if (currentState == PlayerState.Green && greenScore > 0)
                    GreenPowerUp();
                else if (currentState == PlayerState.Blue && blueScore > 0)
                    BluePowerUp();
                else
                {
                    if (!gameManagerS.muteToggle.isOn)
                        errorAudio.Play();
                }
            }

            //deactive green power if form change
            if (scoreMultiplier == 5 && (currentState != PlayerState.Green && currentState != PlayerState.White))
            {
                setMultiplier(false);
                if (!gameManagerS.muteToggle.isOn)
                    transformLossAudio.Play();
            }

            //deactive shield power if form change
            if (shield.activeSelf && (currentState != PlayerState.Blue && currentState != PlayerState.White))
            {
                shield.SetActive(false);
                if (!gameManagerS.muteToggle.isOn)
                    transformLossAudio.Play();
            }

            //revert back
            if ((currentState == PlayerState.Red && redScore == 0) || (currentState == PlayerState.Green && greenScore == 0) || (currentState == PlayerState.Blue && blueScore == 0))
            {
                RevertToWhite();
                if (!gameManagerS.muteToggle.isOn)
                    transformLossAudio.Play();
            }
        }
    }

    void setMultiplier(bool activate)
    {
        if (activate)
        {
            multiplier.SetActive(true);
            scoreMultiplier = 5;
        }
        else
        {
            multiplier.SetActive(false);
            scoreMultiplier = 1;
        }
    }

    void UpdateScreen()
    {
        redScoreText.text = "Red " + redScore;
        greenScoreText.text = "Green " + greenScore;
        blueScoreText.text = "Blue " + blueScore;
        scoreText.text = score.ToString();
    }
    void PlayerTrans()
    {
        if (Input.GetKeyDown(KeyCode.J)) //red
        {
            if (redScore == 5 && currentState != PlayerState.Red)
            {
                redScore--;
                currentState = PlayerState.Red;
                GetComponent<Renderer>().material = redMat;
                if (!gameManagerS.muteToggle.isOn)
                    transformAudio.Play();
            }
            else
            {
                if (!gameManagerS.muteToggle.isOn)
                    errorAudio.Play();
            }
        }
        if (Input.GetKeyDown(KeyCode.K)) //green
        {
            if (greenScore == 5 && currentState != PlayerState.Green)
            {
                greenScore--;
                currentState = PlayerState.Green;
                GetComponent<Renderer>().material = greenMat;
                if (!gameManagerS.muteToggle.isOn)
                    transformAudio.Play();

            }
            else
            {
                if (!gameManagerS.muteToggle.isOn)
                    errorAudio.Play();
            }
        }
        if (Input.GetKeyDown(KeyCode.L)) //blue
        {
            if (blueScore == 5 && currentState != PlayerState.Blue)
            {
                blueScore--;
                currentState = PlayerState.Blue;
                GetComponent<Renderer>().material = blueMat;
                if (!gameManagerS.muteToggle.isOn)
                    transformAudio.Play();
            }
            else
            {
                if (!gameManagerS.muteToggle.isOn)
                    errorAudio.Play();
            }
        }
    }

    void RedPowerUp() //Nuke
    {
        if (!gameManagerS.muteToggle.isOn)
            nukeAudio.Play();
        redScore--;
        foreach (GameObject obstacle in GameObject.FindGameObjectsWithTag("Obstacle"))
        {
            //Destroy(obstacle);
            obstacle.SetActive(false);
        }

    }
    void GreenPowerUp() //Multiply
    {

        if (scoreMultiplier == 1)
        {
            greenScore--;
            setMultiplier(true);
            if (!gameManagerS.muteToggle.isOn)
                multiplyAudio.Play();
        }
        else
        {
            if (!gameManagerS.muteToggle.isOn)
                errorAudio.Play();
        }
    }

    void BluePowerUp()
    {

        if (!shield.activeSelf)
        {
            blueScore--;
            shield.SetActive(true);
            if (!gameManagerS.muteToggle.isOn)
                shieldAudio.Play();
        }
        else
        {
            if (!gameManagerS.muteToggle.isOn)
                errorAudio.Play();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!gameManagerS.muteToggle.isOn)
            orbCollectedAudio.Play();

        switch (other.tag)
        {
            case "redOrb":
                if (redScore < 5 && currentState != PlayerState.Red)
                    redScore++;
                if (scoreMultiplier == 5 && redScore < 5)
                    redScore++;
                if (currentState == PlayerState.Red)
                    score++;
                break;
            case "greenOrb":
                if (greenScore < 5 && currentState != PlayerState.Green)
                    greenScore++;
                if (currentState == PlayerState.Green)
                    score += scoreMultiplier;
                break;
            case "blueOrb":
                if (blueScore < 5 && currentState != PlayerState.Blue)
                    blueScore++;
                if (scoreMultiplier == 5 && blueScore < 5)
                    blueScore++;
                if (currentState == PlayerState.Blue)
                    score++;
                break;
        }
        score += scoreMultiplier;
        if (scoreMultiplier == 5)
        {
            setMultiplier(false);
        }


        other.gameObject.SetActive(false);
        //Destroy(other.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Obstacle"))
        {
            if (!gameManagerS.muteToggle.isOn)
                obstacleHitAudio.Play();
            //Destroy(collision.gameObject);
            if (shield.activeSelf)
            {
                //Destroy(collision.gameObject);
                collision.gameObject.SetActive(false);
                shield.SetActive(false);

            }
            else
            {
                if (currentState != PlayerState.White)
                {
                    //Destroy(collision.gameObject);
                    collision.gameObject.SetActive(false);
                    RevertToWhite();
                    if (scoreMultiplier == 5)
                    {
                        setMultiplier(false);
                    }
                }
                else
                {
                    gameManagerS.GameOver();
                }
            }

        }
    }

    void RevertToWhite()
    {
        currentState = PlayerState.White;
        GetComponent<Renderer>().material = defaultMat;
    }

    public void PlayerRestart()
    {
        transform.position = new Vector3(0, 0.52f, 0);
        score = 0;
        redScore = 0;
        greenScore = 0;
        blueScore = 0;
        currentState = PlayerState.White;
        GetComponent<Renderer>().material = defaultMat;
        shield.SetActive(false);
        setMultiplier(false);
    }

}
