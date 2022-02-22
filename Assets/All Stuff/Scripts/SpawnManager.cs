using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    //things to be spawn
    public GameObject[] groundObstacle;
    public GameObject[] flyingObstacle;
    public GameObject[] powerUp;
    public GameObject[] junkFood;
    public GameObject[] expPlatform;
    public GameObject[] backgrounds;

    //spawning positions
    private Vector3 spawnPosGroundObstcle = new Vector3(100, 0, 1.5f);


    private float x;    

    //time Delays
    private float starDelayFlyingObs = 0;
    private float starDelayGroundObs = 3; //dont need it
    private float starDelayPowerUp = 2;
    private float starDelayJunkFood = 7;
    private float starDelay = 9;

    //calling other script
    private PlayerController playerControllerScript;

    private float flyingForce = 450;

    // Start is called before the first frame update
    void Start()
    {
        playerControllerScript = GameObject.Find("Player").GetComponent<PlayerController>();

        

        Invoke("SpawnFlyingObstacle", starDelayFlyingObs);
        Invoke("SpawnGroundObstacle", starDelayGroundObs);
        Invoke("SpawnPowerUp", starDelayPowerUp);
        Invoke("SpawnJunkFood", starDelayJunkFood);

        ///spawnPosFood = new Vector3(25, heightFood, 0);
        
        
    }

    // Update is called once per frame
    void Update()
    {
        x = playerControllerScript.gameObject.transform.position.x + 40;
    }

    void SpawnGroundObstacle()
    {
        float starDelay = Random.Range(16, 22);
        int ind = Random.Range(0, groundObstacle.Length);
        if (ind > 0 && !playerControllerScript.finish)
        {
            //Instantiate obstacles on the ground
            Instantiate(groundObstacle[ind], spawnPosGroundObstcle, groundObstacle[ind].transform.rotation);
            Invoke("SpawnGroundObstacle", starDelay);

        }
    }
    void SpawnFlyingObstacle()
    {
        //float starDelay = Random.Range(9, 13);
        int ind = Random.Range(0, flyingObstacle.Length);
        if (!playerControllerScript.finish)
        {
            //Instantiate flying obstacles 
            GameObject fly = Instantiate(flyingObstacle[ind], new Vector3(x, Random.Range(7.5f, 8f), 1.5f), flyingObstacle[ind].transform.rotation);
            fly.GetComponent<Rigidbody>().AddTorque(new Vector3(0, 0, -1f) * flyingForce, ForceMode.Impulse);
            //fly.GetComponent<Rigidbody>().AddForce(Vector3.left * 100, ForceMode.Impulse);
            Invoke("SpawnFlyingObstacle", starDelay/1.9f);

        }
    }
    void SpawnExpPlatform()
    {
        int ind = Random.Range(0, expPlatform.Length);
        if (!playerControllerScript.finish)
        {
            Instantiate(expPlatform[ind], new Vector3(x, Random.Range(4.5f, 4.9f), 1.5f), expPlatform[ind].transform.rotation);
        }

    }
    void SpawnPowerUp()
    {
        //float starDelay = Random.Range(12, 18);
        int ind = Random.Range(0, powerUp.Length);
        if (!playerControllerScript.finish)
        {
            Instantiate(powerUp[ind], new Vector3(x, Random.Range(1f, 1.9f), 1.5f), powerUp[ind].transform.rotation);
            Invoke("SpawnPowerUp", starDelay);
            StartCoroutine("SpawnCountDown");
        }

    }

    void SpawnJunkFood()
    {
        //float starDelay = Random.Range(10, 20);
        int ind = Random.Range(0, junkFood.Length);
        if (!playerControllerScript.finish)
        {
            Instantiate(junkFood[ind], new Vector3(x, Random.Range(3.4f, 3.7f), 1.5f), junkFood[ind].transform.rotation);
            Invoke("SpawnJunkFood", starDelay*3);
        }
    }


    IEnumerator SpawnCountDown()
    {
        yield return new WaitForSeconds(2);
        SpawnExpPlatform();
    }
}
