using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    /* OVERVIEW
     * This file intended to be read first. 
    * This is the main file you would want to edit in order to tweak the game settings .
    * The variables pertaining to the game settings are in the first section, accompanied by descriptions 
    * Note that all of these variables are public so you can edit them from inside the editor 
    * This file is intended to be read first, followed by PlayerController.cs, then Enemy.cs
    * Each file has one instance of an RNG that is noted where it occurs. The numbering of these instances is based on the given reading order 
    */

    //=============== Section ========================//
    /* These are the only values you should need to change in order to tweak the game settings
     * They are also editable inside the editor
     */
    public int TilesPerAxis = 4; // this # squared = the total possible # of spawn locations for the enemies
    public int TotalSpawns = 15; // the amt of enemies spawned before game loop ends
    public float SpawnDelay = 3f; // time between spawns
    public float AverageGrowthRate = .2f; // each enemy has a randomly generated growth rate, normalized around this value
    public float LampPower = 1f; // attack power of lamp - enemies start with 5 health
    public float CriticalHitThreshold = .5f; // range: 0 -1. RNG every time player hits space, if value is greater than this threshold, its a crit (2x dmg)
    public float LampRefreshRate = 1f; //range 1-5. The higher this value is this, the quicker the hitbox for the lamp attack will be on screen
    public float StunTimer = 1.5f; // every time an enemy is hit is ceases growing for a period for of time
    public float PlayerSpeed = 20f; // range 5 - 100
    public Color CriticalHitColor;  // color that lamp/aura turns when a crit is achieved
    public Color DefaultLampColor; // default color of lamp/aura
    //=============== End section =====================//

    //=============== Section ========================//
    /* Some data structures and references that are used for spawning the Flowers in a grid-like fashion 
     * Although they are public, they really only need to be set once and shouldn't need to be changed 
     */
    public GameObject EnemyPrefab;
    public Text StatsDisplay;
    public GameObject GameOverPanel;
    List<int> CurrentSpawns = new List<int>();
    public float xGuiOffset = 0.2f;
    public float ActorBoundaryInset = 0.02f;
    float distanceFromCamera = 10;
    //=============== End section =====================//

    //=============== Game state variables ========================//
    int score = 0;
    bool gameOver = false;
    //=============================================================//

    void Start()
    {
        /*
         * The way the game loop works is that it starts a coroutine which gradually spawns all the enemies until the game is over . 
         * In order to restart the game , the game manager object is just destroyed and re instantiated
         */
        StartCoroutine(SpawnGradual());
    }

    void Update()
    {
        UpdateGui();

        if (gameOver)
        {
            /*
             * End of game logic
             */
            GameOverPanel.SetActive(true);
            if (Input.GetKeyDown(KeyCode.B))
            {
                GameObject newGm = Instantiate(gameObject, transform.position, transform.rotation);
                newGm.name = "GameManager";
                GameOverPanel.SetActive(false);
                Destroy(gameObject);
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                SceneManager.LoadScene("Credits");
            }

        }
    }

    IEnumerator SpawnGradual()
    {
        /*
         * This is the coroutine that spawns all of the enemies .
         * As you can see the amount of enemies that are spawned are determined by a public variable .
         */
        int i = 0;
        while (i < TotalSpawns)
        {
            yield return SpawnEnemy();
            i++;
        }
        yield return new WaitForSeconds(SpawnDelay);
        gameOver = true;
    }
    IEnumerator SpawnEnemy()
    {
        int totalTiles = TilesPerAxis * TilesPerAxis;
        int i = Random.Range(0, totalTiles);
        if (CurrentSpawns.Count != totalTiles)
        {
            /* RNG instance #1 - the spawn location is selected randomly from a tile in the grid via a uniform distribution
             */
            while (CurrentSpawns.Contains(i)) i = Random.Range(0, totalTiles);
            CurrentSpawns.Add(i);

        }

        // This code spawns the Flowers in a tiled grid 
        float xSpread = (1f - xGuiOffset) / TilesPerAxis;
        float ySpread = 1f / TilesPerAxis;
        float xOffset = xGuiOffset + xSpread / 2;
        float yOffset = ySpread / 2;
        float xSpawn = xOffset + xSpread * (i % TilesPerAxis);
        float ySpawn = yOffset + ySpread * Mathf.Floor(i / TilesPerAxis);
        Vector3 spawnPos = Camera.main.ViewportToWorldPoint(new Vector3(xSpawn, ySpawn, distanceFromCamera));
        GameObject thing = GameObject.Instantiate(EnemyPrefab, spawnPos, Quaternion.identity);
        thing.GetComponent<Enemy>().GameManager = this;
        yield return new WaitForSeconds(SpawnDelay);

    }


    //=============== Section ========================//
    /* The scoring system is fairly simple but could be easily extended 
     */
    public void AddToScore()
    {
        /*
         * called when a player eliminates an enemy
         */
        score += 20;
    }

    public void AddToDamage()
    {
        /*
         * called when an enemy grows to completion
         */
        score -= 10;
    }
    //=============== End section =====================//


    void UpdateGui()
    {
        /* How the score is updated on screen
         */
        string StatsString = $"{score}";
        StatsDisplay.text = StatsString;
    }

}
