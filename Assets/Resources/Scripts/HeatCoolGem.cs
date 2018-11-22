using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatCoolGem : MonoBehaviour {

    public bool heatCoolGem; // true = heat / false = cool

    public bool activated = false;
    public float valueMeter = 0;
    float IncreaseAmount = 0.5f, maxValue = 100;


    public GameObject heatBar, heatWijzer, heatRadius;
    Sprite gauge, wijzer, fillPixel;
    SpriteRenderer renderGauge, renderWijzer, renderHeat;
    Vector2Int gridLoc;

    GameObject particleSystem;
    GameObject waveParticles;

    int[,] gridHeat;

    void Start()
    {
        heatBar = new GameObject("heatBar");
        heatWijzer = new GameObject("heatWijzer");
        heatBar.transform.SetParent(transform);
        heatWijzer.transform.SetParent(transform);

        gauge = Resources.Load("Sprites/gauge", typeof(Sprite)) as Sprite;
        wijzer = Resources.Load("Sprites/wijzer", typeof(Sprite)) as Sprite;
        renderGauge = heatBar.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;
        renderWijzer = heatWijzer.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;
        renderGauge.sprite = gauge;
        renderWijzer.sprite = wijzer;
        renderGauge.color = Color.clear;
        renderWijzer.color = Color.clear;

        heatBar.transform.localScale = new Vector3(0.4f, 0.4f, 0);
        heatWijzer.transform.localScale = new Vector3(0.8f, 0.4f, 0);
        heatBar.transform.position = transform.position + new Vector3(0, 0f, -0.15f);
        heatWijzer.transform.position = transform.position + new Vector3(0, -0.2f, -0.2f);

        fillPixel = Resources.Load("Sprites/fillpixel", typeof(Sprite)) as Sprite;
        heatRadius = new GameObject("heatRadius");
        heatRadius.transform.SetParent(transform);
        renderHeat = heatRadius.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;
        renderHeat.sprite = fillPixel;
        renderHeat.color = Color.clear;
        heatRadius.transform.localScale = new Vector3(300, 300, 0);

        Vector2 tilePos = GetComponent<GemScript>().tilePos;
        heatRadius.transform.position = new Vector3(tilePos.x, tilePos.y, 0) + new Vector3(-1, 1, -0.1f);

        gridLoc = GetComponent<GemScript>().tileLoc;
        gridHeat = GetComponent<GemScript>().gridHeat;

        if (heatCoolGem)
            particleSystem = Resources.Load("Sprites/particles/heat waves", typeof(GameObject)) as GameObject;
        else
            particleSystem = Resources.Load("Sprites/particles/coldwaves", typeof(GameObject)) as GameObject;


    }

    void Update()
    {
        float prevValueMeter = valueMeter;

        if (activated)
        {
            if (valueMeter < maxValue)
                valueMeter += IncreaseAmount;
        }
        else
        {
            if (valueMeter > 0)
                valueMeter -= IncreaseAmount;
        }
        heatBar.SetActive(true);
        heatWijzer.SetActive(true);
        if (valueMeter == 0 || valueMeter == 100)
        {
            heatBar.SetActive(false);
            heatWijzer.SetActive(false);
        }
        float rotation = -180 * (valueMeter / maxValue) + 90;

       if(valueMeter == 100 && transform.childCount <= 3)
        {
            waveParticles = Instantiate(particleSystem);
            waveParticles.transform.position = transform.position + new Vector3(0.25f, -0.25f, -0.1f);
            waveParticles.transform.SetParent(transform);
        }
        else if(valueMeter < 100 && transform.childCount >= 4)
        {
            Destroy(waveParticles);
        }


        if (heatCoolGem)
        {
            renderHeat.color = new Color(1f, 0, 0, (valueMeter / maxValue) - 0.7f);
            renderWijzer.color = new Color((valueMeter / maxValue), 1 - (valueMeter / maxValue), 0.6f);
            renderGauge.color = new Color((valueMeter / maxValue), 1 - (valueMeter / maxValue), 0.2f);

            for (int iGrid = 0; iGrid < 9; iGrid++)
            {
                if (valueMeter == 100 && prevValueMeter < 100)
                    gridHeat[gridLoc.x + iGrid % 3 - 1, gridLoc.y + iGrid / 3 - 1] += 10;
                else if (valueMeter != 100 && prevValueMeter == 100)
                    gridHeat[gridLoc.x + iGrid % 3 - 1, gridLoc.y + iGrid / 3 - 1] -= 10;
            }
        }
        else
        {
            renderHeat.color = new Color(0, 0, 1f, (valueMeter / maxValue) - 0.7f);
            renderWijzer.color = new Color(0.3f, 1 - (valueMeter / maxValue), (valueMeter / maxValue));
            renderGauge.color = new Color(0.1f, 1 - (valueMeter / maxValue), (valueMeter / maxValue));

            for (int iGrid = 0; iGrid < 9; iGrid++)
            {
                if (valueMeter == 100 && prevValueMeter < 100)
                    gridHeat[gridLoc.x + iGrid % 3 - 1, gridLoc.y + iGrid / 3 - 1] += 1;
                else if (valueMeter != 100 && prevValueMeter == 100)
                    gridHeat[gridLoc.x + iGrid % 3 - 1, gridLoc.y + iGrid / 3 - 1] -= 1;
            }
        }
    

        heatWijzer.transform.rotation = Quaternion.Euler(0, 0, rotation);

        activated = false;
    }
}
