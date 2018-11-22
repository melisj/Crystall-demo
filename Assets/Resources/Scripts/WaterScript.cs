using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterScript : MonoBehaviour {

    public bool ice = false;
    public Vector2Int tileLoc;
    public int[,] gridHeat;
    public float temp;
    public float maxTemp = 120;

    Sprite waterSprite, iceFullSprite, iceDecay1Sprite, iceDecay2Sprite, iceDecay3Sprite;
    SpriteRenderer spriteRender;

	void Start () {
        waterSprite = Resources.Load("Sprites/Tiles/water", typeof(Sprite)) as Sprite;
        iceFullSprite = Resources.Load("Sprites/Tiles/icefull", typeof(Sprite)) as Sprite;
        iceDecay1Sprite = Resources.Load("Sprites/Tiles/icedecay1", typeof(Sprite)) as Sprite;
        iceDecay2Sprite = Resources.Load("Sprites/Tiles/icedecay2", typeof(Sprite)) as Sprite;
        iceDecay3Sprite = Resources.Load("Sprites/Tiles/icedecay3", typeof(Sprite)) as Sprite;
        spriteRender = gameObject.AddComponent<SpriteRenderer>();
        spriteRender.sprite = waterSprite;

        gridHeat = GameObject.Find("GridManager").GetComponent<GridInfo>().gridHeat;
    }
	
	void Update () {
        if (temp >= maxTemp)
        {
            spriteRender.sprite = iceFullSprite;
            temp = maxTemp;
        }
        else if (temp < maxTemp && temp >= maxTemp * (2f / 3f))
            spriteRender.sprite = iceDecay1Sprite;
        else if (temp < maxTemp * (2f / 3f) && temp >= maxTemp * (1f / 3f))
            spriteRender.sprite = iceDecay2Sprite;
        else if (temp < maxTemp * (1f / 3f) && temp > 0)
            spriteRender.sprite = iceDecay3Sprite;
        else if (temp <= 0)
        {
            spriteRender.sprite = waterSprite;
            temp = 0;
        }

        if (gridHeat[tileLoc.x, tileLoc.y] / 10 < gridHeat[tileLoc.x, tileLoc.y] % 10)
            temp++;
        else
            temp--;

        if (temp > 0)
            ice = true;
        else
            ice = false;
    }
}
