using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemScript : MonoBehaviour {

    public GameObject gridInfo;
    public int[,] gridHeat;
    public int[,] gridObjects;
    Vector2Int selectTileGridLoc;
    public Vector2 tilePos;
    public Vector2Int tileLoc;
    public int gemNum = 0;
    Sprite gemTexture;

    void Start()
    {
        gridInfo = GameObject.Find("GridManager");
        gridObjects = gridInfo.GetComponent<LevelLoader>().gridObjects;
        selectTileGridLoc = gridInfo.GetComponent<GridInfo>().selectTileGridLoc;
        gridHeat = gridInfo.GetComponent<GridInfo>().gridHeat;

        gemTexture = Resources.Load("Sprites/gem3", typeof(Sprite)) as Sprite;
        SpriteRenderer render = gameObject.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;
        render.sprite = gemTexture;
        transform.position += new Vector3(0.5f - GetComponent<Renderer>().bounds.size.x /2 , -0.5f + GetComponent<Renderer>().bounds.size.y / 2,0);

        tilePos = gridInfo.GetComponent<LevelLoader>().gridPos[tileLoc.x, tileLoc.y];

        //checks which gem it is, and assigns the appropriate color to it
        switch (gemNum)
        {
            case 0:
                GetComponent<SpriteRenderer>().color = Color.yellow;
                break;
            case 1:
                GetComponent<SpriteRenderer>().color = Color.red;
                break;
            case 2:
                GetComponent<SpriteRenderer>().color = Color.blue;
                break;
        }
    }

    void Update()
    {
        
    }

    public void RemoveGem()
    {
        gridObjects[tileLoc.x, tileLoc.y] = 0;
        if (gemNum == 1 || gemNum == 2)
        {
            for (int iGrid = 0; iGrid < 9; iGrid++)
            {
                if (GetComponent<HeatCoolGem>().heatCoolGem && GetComponent<HeatCoolGem>().valueMeter == 100)
                    gridInfo.GetComponent<GridInfo>().gridHeat[tileLoc.x + iGrid % 3 - 1, tileLoc.y + iGrid / 3 - 1] -= 10;
                else if (!GetComponent<HeatCoolGem>().heatCoolGem && GetComponent<HeatCoolGem>().valueMeter == 100)
                    gridInfo.GetComponent<GridInfo>().gridHeat[tileLoc.x + iGrid % 3 - 1, tileLoc.y + iGrid / 3 - 1] -= 1;
            }
        }

        Destroy(gameObject);
    }
}
