using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridInfo : MonoBehaviour
{

    LevelLoader loader;
    PlayerScript playerInfo;
    Inventory inventoryInfo;
    SpriteRenderer selectTileRender;
    public SceneSwitcher sceneSwitcher;
    public GameObject player, selectTile, mineables, lights, inventory;
    public int[,] gridHeat;
    int[,] gridInfo;
    public int[,] gridObjects;
    Vector2[,] gridPos;
    Vector2 playerSize;
    public Vector2Int selectTileGridLoc;
    public Vector2Int playerGridLoc;
    public Vector2Int finishTile;

    void Start()
    {
        //getting info from scripts
        loader = GetComponent<LevelLoader>();
        gridInfo = loader.gridInfo;
        gridPos = loader.gridPos;
        gridObjects = loader.gridObjects;
        playerInfo = player.GetComponent<PlayerScript>();
        gridHeat = loader.gridHeat;
        inventoryInfo = inventory.GetComponent<Inventory>();
        selectTileRender = selectTile.GetComponent<SpriteRenderer>();
        finishTile = loader.finishTile;
    }

    void Update()
    {
        //checking the position of the player in amount of tiles
        float tileX = player.transform.position.x;
        float tileY = player.transform.position.y;

        int gridX = Mathf.FloorToInt(tileX);
        int gridY = -Mathf.FloorToInt(tileY) - 1;

        playerGridLoc = new Vector2Int(gridX, gridY);

        if(playerGridLoc.x == finishTile.x && playerGridLoc.y == finishTile.y)
            sceneSwitcher.SwitchScene(2);

        if (gridHeat[gridX, gridY] / 10 < gridHeat[gridX, gridY] % 10)
        {
            playerInfo.temp--;
        }
        if (gridHeat[gridX, gridY] / 10 > gridHeat[gridX, gridY] % 10)
        {
            playerInfo.temp++;
        }

        //checks for the tile that the player is looking at
        float direction = playerInfo.direction;

        int selectGridX = gridX;
        int selectGridY = gridY;
        if (direction == 1)
            selectGridY--;
        else if (direction == 2)
            selectGridY++;
        else if (direction == 3)
            selectGridX--;
        else if (direction == 4)
            selectGridX++;

        selectTile.transform.position = gridPos[selectGridX, selectGridY];
        selectTile.transform.position -= new Vector3(0, 0, 0.5f);

        //makes the selectiontile visible when looking at an object interactable in the current mode
        //this object creates an highlight for the player
        selectTile.gameObject.SetActive(false);
        selectTileRender.color = Color.white;
        if (playerInfo.modes == 0)
        {
            int gridObjectNum = gridObjects[selectGridX, selectGridY];
            if (gridObjectNum != 0 && !(gridObjectNum <= 13 && gridObjectNum >= 8) )
            {
                selectTile.SetActive(true);
                Overlay.GiveHint();
            }
        }
        else
        {
            if (gridObjects[selectGridX, selectGridY] == 0 && gridInfo[selectGridX, selectGridY] != 1)
            {
                selectTile.SetActive(true);
                Overlay.GiveHint();
            }
            if (inventoryInfo.ReturnAmount(playerInfo.modes - 1) == 0)
                selectTileRender.color = Color.red;
        }
            
        selectTileGridLoc = new Vector2Int(selectGridX, selectGridY);
    }
}