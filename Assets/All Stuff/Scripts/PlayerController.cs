using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    //Game Over Text
    public TextMeshProUGUI gameOverText;
    public Button restartButton;
    public Button exitButton;

    //player's components 
    private Rigidbody playerRb;
    private Animator playerAnim;
    public AudioSource playerAudio;

    //boss
    private Animator bossAnim;
    private GameObject boss;

    //audio
    private AudioSource MainAudio;
    public AudioClip startSound;
    public AudioClip[] jumpSound;
    public AudioClip[] diveSound;
    public AudioClip deathSound;
    public AudioClip[] junkSound;
    public AudioClip[] powerSound;
    public AudioClip[] expSound;
    public AudioClip bossSound;

    public AudioClip victorySound;
    public AudioClip finishSound;

    //forces
    public float jumpForce;
    public float gravityModifier;

    //bools
    public bool isOnGround=true;
    private bool startGame = false;
    public bool gameOver;
    public bool hasPowerup;
    //private bool isFat = false;
    public bool finish;// whether game is finished/over

    //experience points
    public int expPoints;
    public TextMeshProUGUI expText;

    //treshold of exp when the game speeds up
    private int tresholdSpeedUp=60;

    //Particles
    public ParticleSystem fireworksParticle;
    public ParticleSystem crashParticle;
    public ParticleSystem powerUpParticle;

    //calling other scripts
    private MoveLeft MoveLeft;
    private SpawnManager spawnManagerScript;


    //reaching home
    private int tresholdHome = 100;
    //public GameObject home;

    private float pitch = 3f;

    //speed 
    private float speedPlayer;

    private float leftBorder = 0;
    private float rightBorder = 18f;

    //bool variables to detect if a sound was played
    private bool isBossSoundPlayed = false;
    private bool isFinishSoundPlayed = false;
    private bool isRotated = false;

    private float junkSlow = 100;
    private float forceBoost = 200;


    // Start is called before the first frame update
    void Start()
    {
        
        //accessing player's components
        playerRb = GetComponent<Rigidbody>();
        playerAnim = GetComponent<Animator>();
        playerAudio = GetComponent<AudioSource>();

        //accessing others' components
        //MoveLeft = GameObject.Find("Background").GetComponent<MoveLeft>();
        MainAudio = GameObject.Find("Main Camera").GetComponent<AudioSource>();
        bossAnim = GameObject.Find("Boss").GetComponent<Animator>();

        ///boss = GameObject.Find("Boss");

        //setting variables
        Physics.gravity *= gravityModifier;
        gameOver = false;
        expPoints = 0;
        finish = false;
        //isFat = false;
        //hasPowerup = false;
        speedPlayer = 1f;

    }

    

    // Update is called once per frame
    void Update()
    {

        if (hasPowerup&& jumpForce == 400)
        {
            jumpForce += forceBoost;
            powerUpParticle.gameObject.SetActive(true);
        }
        if (gameOver || (expPoints == tresholdHome))
        {
            finish = true;
        }

        if (Input.GetKey("escape"))
        {
            ExitGame();
        }

        //playing the sound and start running
        if (!startGame)
        {
            playerAudio.PlayOneShot(startSound, pitch);
            startGame = true;

        }

        if (!gameOver)
        {
            MovePlayer();
        }
        if (gameObject.transform.position.x<leftBorder)
        {
            gameOver = true;
            Death();
        }
        

        //if(!gameOver && expPoints == tresholdSpeedUp)
        //{
            
            //if (!isBossSoundPlayed)
           // {
             //   playerAudio.PlayOneShot(bossSound, pitch); //audio doesnt work
           //     isBossSoundPlayed = true;
            //}
       // }
        else if (expPoints == tresholdHome)
        {

            Win();
            Vector3 PosHome = new Vector3(transform.position.x + 19, 0, 6.7f);
            GameObject home= GameObject.Find("Home");
            
            home.transform.position = PosHome;
            //Instantiate(home, PosHome, home.transform.rotation);
        }



    }


    private void OnCollisionEnter(Collision collision)
    {
        //check whether the Player is on the ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;
        }
        //collision with obstacle
        else if (collision.gameObject.CompareTag("Obstacle"))
        {
            Death();

        }
        //getting speed boost
        else if (collision.gameObject.CompareTag("PowerUp"))
        {

            StartCoroutine(PowerUpCountdownRoutine());

            GenerateSound(powerSound);
            Debug.Log("Power Up!");
            ///MoveLeft.speed += 5;
            hasPowerup = true;
            Destroy(collision.gameObject);

            //player
            //playerAnim.SetFloat("speedMultiplier", 1.8f);


          //  powerUpParticle.gameObject.SetActive(true);


            jumpForce += forceBoost;

            Destroy(collision.gameObject);
            
        }
        else if (collision.gameObject.CompareTag("JunkFood"))
        {
            
            StartCoroutine(JunkFoodCountdownRoutine());
           // isFat = true;
            GenerateSound(junkSound);
            Debug.Log("Oh no, I will get fat!");

            //player
            //jumpForce -= junkSlow;
            transform.localScale *= 1.5f;
            //playerAnim.SetFloat("speedMultiplier", 0.8f);


            Destroy(collision.gameObject);



        }

        //getting experience points
        else if (collision.gameObject.CompareTag("Experience"))
        {
            if (expPoints != 40 && expPoints != 80)
            {
                GenerateSound(expSound);
            }
            
            Debug.Log("Experience!");
            Destroy(collision.gameObject);
            UpdateExp(20);
        }

        //collision with boss
        if (collision.gameObject.CompareTag("Boss"))
        {
            Death();
        }

    }

    //setting how long powerup will last
    IEnumerator PowerUpCountdownRoutine()
    {
        yield return new WaitForSeconds(6);
        hasPowerup = false;

        jumpForce -= forceBoost;
        powerUpParticle.gameObject.SetActive(false);

        //playerAnim.SetFloat("speedMultiplier", 1.0f);
    }
    IEnumerator JunkFoodCountdownRoutine()
    {
        yield return new WaitForSeconds(3);
        //isFat = false;
        //jumpForce += junkSlow;
        transform.localScale /= 1.5f;
        //playerAnim.SetFloat("speedMultiplier", 1.0f);

    }

    IEnumerator SpeedUpCountdownRoutine()
    {
        yield return new WaitForSeconds(1.5f);
        if (!isBossSoundPlayed)
        {
            playerAudio.PlayOneShot(bossSound, pitch); //audio doesnt work
            isBossSoundPlayed = true;
        }
        speedPlayer = 1f;
        


    }
    void MovePlayer()
    {
        //jump
        if (Input.GetKeyDown(KeyCode.Space) && isOnGround)
        {
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isOnGround = false;
            //playerAnim.SetTrigger("jumpTrigger");
            GenerateSound(jumpSound);

        }

        //divie down
        if (Input.GetKeyDown(KeyCode.DownArrow) && isOnGround)
        {
            GenerateSound(diveSound);
            playerAnim.SetTrigger("diveTrigger");
        }

        //move to right
        if (Input.GetKeyDown(KeyCode.RightArrow) && isOnGround)// && gameObject.transform.position.x < rightBorder && hasPowerup)
        {
            //Debug.Log("pushed");
            float a = 300f;
            playerRb.AddForce(Vector3.right * a, ForceMode.Acceleration);
        }

        //move to left
        if (Input.GetKeyDown(KeyCode.LeftArrow) && isOnGround)// && gameObject.transform.position.x > leftBorder && hasPowerup)
        {
            float a = 300f;
            playerRb.AddForce(Vector3.left * a, ForceMode.Acceleration);
        }


        if (!finish)
        {
            transform.Translate( Vector3.right*speedPlayer*Time.deltaTime,Space.World);
            if(expPoints == tresholdSpeedUp)
            {
                speedPlayer = 2f;
                transform.Translate(Vector3.right * speedPlayer * Time.deltaTime, Space.World);
                StartCoroutine(SpeedUpCountdownRoutine());
            }
        }

    }
    void GenerateSound(AudioClip[] sound)
    {
        int ind = Random.Range(0, sound.Length);
        playerAudio.PlayOneShot(sound[ind], pitch);
    }

    private void UpdateExp(int expToAdd)
    {
        expPoints += expToAdd;
        expText.text = "Exp:" + expPoints;
    }

    public void GameOver()
    {
        gameOverText.gameObject.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitGame()
    {
        Debug.Log("Exit Game");
        Application.Quit();
    }

    private void Death()
    {
        gameOver = true;
        GameOver();
        Debug.Log("Game Over!");
        playerAnim.SetTrigger("deathTrigger");
        playerAudio.PlayOneShot(deathSound, pitch);
        restartButton.gameObject.SetActive(true);
        exitButton.gameObject.SetActive(true);
        if (null != crashParticle)
        {

            crashParticle.gameObject.SetActive(true);
        }

    }
   
    private void Win()
    {

       // Debug.Log("won");
       // playerRb.transform.Rotate(0, 180, 0);
       // playerAudio.PlayOneShot(victorySound, pitch);
      //  if(null != fireworksParticle)
      //  {

       //     fireworksParticle.gameObject.SetActive(true);

       // }
        //playerAnim.SetTrigger("joyTrigger");



        if (!isRotated)
        {
            Debug.Log("won");
            playerRb.transform.Rotate(0, 90, 0);
            isRotated = true;
        }
        transform.position = new Vector3(transform.position.x, 0.53f, 1.5f);
       // GameObject camera= GameObject.Find("Main Camera");
       // camera.transform.position += new Vector3(10,0,0);


        speedPlayer = 0;
        ///MainAudio.enabled = false;

        if (!isFinishSoundPlayed)
        {
            playerAudio.PlayOneShot(finishSound, pitch); //audio doesnt work
            isFinishSoundPlayed = true;
        }
        playerAnim.SetTrigger("joyTrigger");

        fireworksParticle.gameObject.SetActive(true);


    }

}
