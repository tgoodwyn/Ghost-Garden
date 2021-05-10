using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampAttack : MonoBehaviour
{

    GameManager gm;
    public float ExplosionDelay = 1f;

    //float hitStrength;
    // Start is called before the first frame update
    void Start()
    {
        //gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        ////hitStrength = gm.AttackPower;
        //StartCoroutine(Stun());
        ////StartCoroutine(DestructSequence());
        //StartCoroutine(DestructSequence());
        //Debug.Log("hello");


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //private void OnTriggerEnter2D(Collider2D other)
    //{
    //    if (other.gameObject.tag == "enemy")
    //    {
    //        Flower f = other.gameObject.GetComponent<Flower>();
    //        StartCoroutine(f.Stun());
    //        StartCoroutine(Stun());
    //    }
    //    Debug.Log("hit");
    //}

    public IEnumerator Stun()
    {
       
        //yield return null;
        //yield return new WaitForSeconds(GameManager.StunTimer);
        yield return new WaitForSeconds(0.2f);
        Debug.Log("unstun");
    }

    IEnumerator DestructSequence()
    {
        yield return new WaitForSeconds(ExplosionDelay);
        
    }
}
