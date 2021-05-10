using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    /* OVERVIEW
     * This file intended to be read third. 
     * This file really shouldn't need to be edited . 
     * It controls the enemy behavior. 
     * Note that it has a reference to the game manager , which is how it uses the game setting variables set in the editor 
     * This file has one instance of an RNG that is in the Start function, where the growth rate of the new enemy  instance is initialized
     */

    //=============== Section ========================//

    public GameManager GameManager;
    public float MaxHealth = 5f;
    float GrowthRate = .4f;
    float ExplosionDelay = 1f;
    bool fullyGrown = false;
    Transform flowerBody;
    SpriteRenderer flowerFrame;
    float health;
    bool stunned = false;
    //=============== Section ========================//

    void Start()
    {
        //=========== references initialized====================//
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        flowerBody = transform.Find("Flower Body");
        flowerFrame = transform.Find("Flower Frame").GetComponent<SpriteRenderer>();
        //======================================================//

        //============== state variables initialized ===========//
        /* 
         * RNG instance #3
         * The growth rate of a new enemy is randomly generated using a normal distribution, so should be around the average value set in the game manager
         */
        GrowthRate = GameManager.AverageGrowthRate;
        health = 0;
        GrowthRate = Probabilities.RandomNormalVariable(GrowthRate, GrowthRate / 2, 10, 10);
        GrowthRate = Mathf.Clamp(GrowthRate, .02f, 10);
        //=======================================================//
    }

    void Update()
    {
        /*
         * Three main actions every loop:
         * 1) check if enemy has been hit
         * 2) if the enemy is not currently stunned, update its health, then update its transform.scale to match the health
         * 3) if the enemy has reached full scale, initiate a destruct sequence that terminates in destroying the gameObject and reducing the player's score
         */
        if (health < 0)
        {
            GameManager.AddToScore();
            Destroy(gameObject);
        }

        if (!fullyGrown && !stunned)
        {
            Grow();
        }

        if (flowerBody.localScale.y >= 1)
        {
            fullyGrown = true;
            flowerFrame.color = Color.black;
            StartCoroutine(DestructSequence());
        }
    }


    //=========================== Support functions ===============//
    private void MatchScaleToHealth()
    {
        float HealthToScale = health / MaxHealth;
        flowerBody.localScale = new Vector3(1, HealthToScale, 1);

    }

    private void Grow()
    {
        float HealthToScale = health / MaxHealth;
        flowerBody.localScale = new Vector3(1, HealthToScale + (Time.deltaTime * GrowthRate), 1);
        health = flowerBody.localScale.y * MaxHealth;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "light")
        {
            if (!fullyGrown) StartCoroutine(Stun());
        }
    }
    IEnumerator DestructSequence()
    {
        yield return new WaitForSeconds(ExplosionDelay);
        GameManager.AddToDamage();
        Destroy(gameObject);
    }

    public IEnumerator Stun()
    {
        stunned = true;
        health -= GameManager.LampPower;
        MatchScaleToHealth();
        yield return new WaitForSeconds(GameManager.StunTimer);
        stunned = false;
    }

    //===============================================//


}
