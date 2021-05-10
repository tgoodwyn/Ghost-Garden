using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    /* OVERVIEW
    * This file intended to be read second. 
    * This file really shouldn't need to be edited . 
    * It controls the player movement and attacks. 
    * Note that it has a reference to the game manager , which is how it uses the game setting variables set in the editor 
    * This file has one instance of an RNG that is in the CheckCrit function
    */
    float PlayerSpeed = 20f;

    public GameObject LampLight;
    float CooldownRate = 1f;
    bool lightReady = true;
    float cooldownTimer = 0f;
    GameManager gm;
    float hor;
    float ver;
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        PlayerSpeed = gm.PlayerSpeed;
        PlayerSpeed = PlayerSpeed / 100;
        CooldownRate = gm.LampRefreshRate;
    }

    void Update()
    {
        Move();
        CastLight();
    }

    private void CastLight()
    {
        if (lightReady && Input.GetKeyDown(KeyCode.Space))
        {
            LampLight.SetActive(true);
            StartCoroutine(CheckCrit());
            cooldownTimer = 0f;
            lightReady = false;
        }

        if (!lightReady)
        {
            cooldownTimer += CooldownRate * Time.deltaTime;
            if (cooldownTimer >= 1)
            {
                LampLight.SetActive(false);
                lightReady = true;
            }
        }
    }

    private void Move()
    {
        hor = Input.GetAxisRaw("Horizontal");
        ver = Input.GetAxisRaw("Vertical");
        if (hor != 0 && hor != transform.localScale.x)
        {
            transform.localScale = Vector3.Scale(transform.localScale, new Vector3(-1, 1, 1));
        }

        float xDelta = hor * PlayerSpeed * Time.deltaTime;
        float yDelta = ver * PlayerSpeed * Time.deltaTime;
        Vector3 currentPos = Camera.main.WorldToViewportPoint(transform.position);
        currentPos.x = Mathf.Clamp(currentPos.x + xDelta, 0.05f, .95f);
        currentPos.y = Mathf.Clamp(currentPos.y + yDelta, 0.05f, .95f);
        Vector3 finalPos = Camera.main.ViewportToWorldPoint(currentPos);
        transform.position = finalPos;
    }

    IEnumerator CheckCrit()
    {
        /* 
         * RNG instance #3
         * Uses a uniform distribution to generate a value between 0 and 1.
         * If that value is greater than the game setting "CriticalHitThreshold,
         * then a critical hit is registered
         */

        float r = UnityEngine.Random.Range(0, 1f);
        Color prevColor;
        if (r >= gm.CriticalHitThreshold)
        {
            GameObject body = transform.Find("Body").gameObject;
            SpriteRenderer[] rays = body.GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer ray in rays)
            {
                prevColor = ray.color;
                ray.color = gm.CriticalHitColor;
            }
            gm.LampPower *= 2;
            yield return new WaitForSeconds(.7f);
            gm.LampPower /= 2;
            foreach (SpriteRenderer ray in rays)
            {
                ray.color = gm.DefaultLampColor;
            }
        }
        else
        {
            yield return null;
        }
    }

}
