using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    //public float speedBoss = 0;
    private PlayerController playerControllerScript;
    private int enlargeThreshold=80;
    private int speedUpThreshold=60;

    //Particles
    public ParticleSystem explosionParticle;
    public ParticleSystem enlargeParticle;

    //boss' components
    private Animator bossAnim;
    private Rigidbody bossRb;
    private AudioSource bossAudio;

    //audio
    public AudioClip laughSound;
    public AudioClip gameOverSound;

    //bool to check if sound was played
    private bool isLaughSoundPlayed = false;
    private bool isGameOverSoundPlayed = false;
    private bool isBossRotated = false;

    // Start is called before the first frame update
    void Start()
    {
        playerControllerScript = GameObject.Find("Player").GetComponent<PlayerController>();

        //boss' components
        bossAnim = GetComponent<Animator>();
        bossRb = GetComponent<Rigidbody>();
        bossAudio = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
        if (!playerControllerScript.finish && playerControllerScript.expPoints!= speedUpThreshold&& playerControllerScript.expPoints < speedUpThreshold)
        {
            Vector3 offset = new Vector3(-25f, 0, 0);
            transform.position = new Vector3((GameObject.Find("Player").GetComponent<Transform>().position.x), 0, 0) + offset;

        }


        if (playerControllerScript.gameOver == false)
        {

            //speeding up the Boss
            if (playerControllerScript.expPoints > speedUpThreshold ||playerControllerScript.expPoints == speedUpThreshold)
            {
                Vector3 offset = new Vector3(-3.5f, 0, 0);
                transform.position = new Vector3((GameObject.Find("Player").GetComponent<Transform>().position.x), 0, 0) + offset;


                if (playerControllerScript.expPoints == speedUpThreshold)
                {
                    // transform.localScale *= 1.5f;

                    //transform.Translate(Vector3.right * Time.deltaTime * speedBoss,Space.World);

                    //StartCoroutine(BossSpeedUpCountdownRoutine());
                    //bossAnim.SetFloat("speedMultiplier", 1.5f);
                    explosionParticle.gameObject.SetActive(true);

                    if (!isLaughSoundPlayed)
                    {
                        bossAudio.PlayOneShot(laughSound, 1f); //audio doesn't work
                        isLaughSoundPlayed = true;
                    }



                } 
            }
            //if (playerControllerScript.expPoints > speedUpTreshold&& playerControllerScript.expPoints!= speedUpTreshold)
           // {
            //    transform.Translate(Vector3.right * Time.deltaTime * speedBoss, Space.World);
           // }
            //enlargeing the boss
           // if (playerControllerScript.expPoints == enlargeTreshold) //enlarging the Boss
          //  {
                //transform.localScale *= 2;
                //
              //  if (!isEnlargeSoundSoundPlayed)
              //  {
                    //bossAudio.PlayOneShot(enlargeSound, 1f); //audio doesn't work
            //        isEnlargeSoundSoundPlayed = true;
             //   }

              //  enlargeParticle.Play();
         //   }
        }
        else if (playerControllerScript.gameOver == true)
        {

            bossAnim.SetTrigger("victoryTrigger");
            if (!isGameOverSoundPlayed)
            {
                bossAudio.PlayOneShot(gameOverSound, 1f); //audio doesn't work
                isGameOverSoundPlayed = true;
            }
        }
        if (playerControllerScript.expPoints == 100)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            playerControllerScript.gameOver = true;
            collision.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.right, ForceMode.Impulse);
            bossAnim.SetTrigger("victoryTrigger");

        }
    }

   // IEnumerator BossSpeedUpCountdownRoutine()
   // {
       // yield return new WaitForSeconds(2);
        //transform.localScale = 1.5f;
        //explosionParticle.gameObject.SetActive(true);
       // bossAnim.SetFloat("speedMultiplier", 1.0f);
        //speedBoss = 1f;

   // }
}
