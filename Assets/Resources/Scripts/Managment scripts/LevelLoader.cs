using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour {

    public TextAsset levelFile;
    public int rowLength;
    public int columLength;
    public int[,] gridInfo; //info about block in levelData
    public int[,] gridObjects; //info about object within the level (changeable)
    public int[,] gridHeat; //info about heat within the level (changeable) / 1X = heat amount / X1 = cool amount
    public Vector2[,] gridPos = new Vector2[0, 0]; //positions of tiles in the grid
    public Vector2Int finishTile;
    Sprite grassSprite, wallSprite, mineSprite, lightSourceSprite, machineSprite, waterSprite, turretSprite, exitSprite;

    //object numbers in object grid
    [NonSerialized] public int mineNum = 1;
    [NonSerialized] public int lightGemNum = 2;
    [NonSerialized] public int heatGemNum = 3;
    [NonSerialized] public int coolGemNum = 4;
    [NonSerialized] public int laserGemNum = 5;
    [NonSerialized] public int electricGemNum = 6;
    [NonSerialized] public int explosiveGemNum = 7;
    [NonSerialized] public int machineNum = 8;
    [NonSerialized] public int gateNum = 9;
    [NonSerialized] public int lightsourceNum = 10;
    [NonSerialized] public int woodNum = 11;
    [NonSerialized] public int turretNum = 12;

    public GameObject tileGroup, mineGroup, lights, interactables, enemies;


    void Start () {
        //reading data from levelData
        string[] levelData = levelFile.text.Split(';');
        string[] rowData = levelData[1].Split(':');

        rowLength = rowData[0].Length / 3;
        columLength = rowData.Length;

        gridInfo = new int[rowLength, columLength];
        gridObjects = new int[rowLength, columLength];
        gridHeat = new int[rowLength, columLength];
        for (int iColum = 0; iColum < columLength; iColum++)
        {
            string[] rowParts = rowData[iColum].Split(',');
            for (int iRow = 0; iRow < rowLength; iRow++)
            {
                gridInfo[iRow, iColum] = int.Parse(rowParts[iRow]);
                gridObjects[iRow, iColum] = 0;
                gridHeat[iRow, iColum] = 0;
            }
        }

        //loading sprites
        grassSprite = Resources.Load("Sprites/Tiles/grastile", typeof(Sprite)) as Sprite;
        wallSprite = Resources.Load("Sprites/Tiles/wall", typeof(Sprite)) as Sprite;
        mineSprite = Resources.Load("Sprites/Tiles/gemmine2", typeof(Sprite)) as Sprite;
        lightSourceSprite = Resources.Load("Sprites/Tiles/lightsource", typeof(Sprite)) as Sprite;
        turretSprite = Resources.Load("Sprites/Tiles/turret", typeof(Sprite)) as Sprite;
        exitSprite = Resources.Load("Sprites/Tiles/levelfinish", typeof(Sprite)) as Sprite;

        FillGrid();
    }
	
	void Update () {
	}

    //filling the grid with positions and textures and the right data in every array.
    void FillGrid()
    {
        gridPos = new Vector2[rowLength, columLength];
        float gridDistance = 1f;

        for (int yGrid = 0; yGrid < gridPos.Length / gridPos.GetLength(0); yGrid++)
        {
            for (int xGrid = 0; xGrid < gridPos.GetLength(0); xGrid++)
            {
                gridPos[xGrid, yGrid] = new Vector2(xGrid * gridDistance, yGrid * -gridDistance);

                GameObject obj = new GameObject("tile");
                SpriteRenderer render = obj.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;

                switch (gridInfo[xGrid, yGrid])
                {
                    case -1: //empty grid
                        Destroy(obj);
                        break;
                    case 1: //wall
                        render.sprite = wallSprite;
                        obj.AddComponent<BoxCollider2D>();
                        obj.name = "wall";
                        break;
                    case 2: //lightsource
                        render.sprite = lightSourceSprite;
                        obj.name = "wall";
                        gridObjects[xGrid, yGrid] = lightsourceNum;

                        GameObject light = new GameObject("parentLight");
                        LightBeamScript lightScript = light.AddComponent(typeof(LightBeamScript)) as LightBeamScript;
                        light.transform.position = new Vector3(gridPos[xGrid, yGrid].x, gridPos[xGrid, yGrid].y, 0);
                        lightScript.tileLoc = new Vector2Int(xGrid, yGrid);
                        light.transform.SetParent(lights.transform);
                        if (yGrid == 0)
                            lightScript.direction = 2;
                        else if (yGrid == columLength - 1)
                            lightScript.direction = 1;
                        else
                            lightScript.direction = 4;
                        break;
                    case 3: //mine
                        GameObject mine = new GameObject("mine");
                        mine.AddComponent<BoxCollider2D>().offset = new Vector2(0.5f, -0.5f);
                        SpriteRenderer render2 = mine.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;
                        MineGemScript mineScript = mine.AddComponent(typeof(MineGemScript)) as MineGemScript;
                        render2.sprite = mineSprite;
                        mine.transform.parent = mineGroup.transform;
                        mine.transform.position = new Vector3(gridPos[xGrid, yGrid].x, gridPos[xGrid, yGrid].y, -0.1f);

                        gridObjects[xGrid, yGrid] = mineNum;
                        mineScript.tileLoc = new Vector2Int(xGrid, yGrid);
                        render.sprite = grassSprite;
                        obj.name = "grass";
                        break;
                    case 4: //machine
                        GameObject machine = new GameObject("machine");
                        machine.AddComponent<BoxCollider2D>().offset = new Vector2(0.5f,-0.5f);
                        MachineScript machineScript = machine.AddComponent(typeof(MachineScript)) as MachineScript;
                        machine.transform.parent = interactables.transform;
                        machine.transform.position = new Vector3(gridPos[xGrid, yGrid].x, gridPos[xGrid, yGrid].y, -0.1f);
                        gridObjects[xGrid, yGrid] = machineNum;

                        if (gridInfo[xGrid - 1, yGrid] == 5)
                            machineScript.direction = 2;
                        else if (gridInfo[xGrid + 1, yGrid] == 5)
                            machineScript.direction = 1;
                        else if (gridInfo[xGrid, yGrid - 1] == 5)
                            machineScript.direction = 3;
                        else if (gridInfo[xGrid, yGrid + 1] == 5)
                            machineScript.direction = 4;
                        machineScript.gridLoc = new Vector2Int(xGrid, yGrid);
                        break;

                    case 6: //water
                        GameObject water = new GameObject("water");
                        water.transform.parent = interactables.transform;
                        water.transform.position = gridPos[xGrid, yGrid];
                        WaterScript waterScript = water.AddComponent(typeof(WaterScript)) as WaterScript;
                        waterScript.tileLoc = new Vector2Int(xGrid, yGrid);
                        break;
                    case 7: //wood
                        GameObject wood = new GameObject("wood");
                        wood.AddComponent<BoxCollider2D>().offset = new Vector2(0.5f, -0.5f);
                        wood.transform.parent = interactables.transform;
                        wood.transform.position = (Vector3)gridPos[xGrid, yGrid] + new Vector3(0, 0, -0.1f);
                        gridObjects[xGrid, yGrid] = woodNum;
                        WoodScript woodScript = wood.AddComponent(typeof(WoodScript)) as WoodScript;
                        woodScript.tileLoc = new Vector2Int(xGrid, yGrid);

                        render.sprite = grassSprite;
                        obj.name = "grass";
                        break;
                    case 8: //lightgem
                        GameObject lightGem = new GameObject("gem");
                        GemScript lightscript = lightGem.AddComponent<GemScript>() as GemScript;
                        lightscript.gemNum = 0;
                        lightscript.tileLoc = new Vector2Int(xGrid, yGrid);
                        lightGem.transform.parent = mineGroup.transform;
                        lightGem.transform.position = (Vector3)gridPos[xGrid, yGrid] + new Vector3(0, 0, -0.1f);
                        gridObjects[xGrid, yGrid] = lightGemNum;

                        render.sprite = grassSprite;
                        obj.name = "grass";
                        break;
                    case 9: //heatgem
                        GameObject heatGem = new GameObject("gem");
                        GemScript heatScript = heatGem.AddComponent<GemScript>() as GemScript;
                        HeatCoolGem heatGemScript = heatGem.AddComponent<HeatCoolGem>() as HeatCoolGem;
                        heatGemScript.heatCoolGem = true;
                        heatScript.gemNum = 1;
                        heatScript.tileLoc = new Vector2Int(xGrid, yGrid);
                        heatGem.transform.parent = mineGroup.transform;
                        heatGem.transform.position = (Vector3)gridPos[xGrid, yGrid] + new Vector3(0, 0, -0.1f);
                        gridObjects[xGrid, yGrid] = heatGemNum;

                        render.sprite = grassSprite;
                        obj.name = "grass";
                        break;
                    case 10: //coolgem
                        GameObject coolGem = new GameObject("gem");
                        GemScript coolScript = coolGem.AddComponent<GemScript>() as GemScript;
                        HeatCoolGem coolGemScript = coolGem.AddComponent<HeatCoolGem>() as HeatCoolGem;
                        coolGemScript.heatCoolGem = false;
                        coolScript.gemNum = 2;
                        coolScript.tileLoc = new Vector2Int(xGrid, yGrid);
                        coolGem.transform.parent = mineGroup.transform;
                        coolGem.transform.position = (Vector3)gridPos[xGrid, yGrid] + new Vector3(0, 0, -0.1f);
                        gridObjects[xGrid, yGrid] = coolGemNum;

                        render.sprite = grassSprite;
                        obj.name = "grass";
                        break;
                    case 11: //lasergem
                        GameObject laserGem = new GameObject("gem");
                        GemScript laserScript = laserGem.AddComponent<GemScript>() as GemScript;
                        laserScript.gemNum = 3;
                        laserScript.tileLoc = new Vector2Int(xGrid, yGrid);
                        laserGem.transform.parent = mineGroup.transform;
                        laserGem.transform.position = (Vector3)gridPos[xGrid, yGrid] + new Vector3(0, 0, -0.1f);
                        gridObjects[xGrid, yGrid] = laserGemNum;

                        render.sprite = grassSprite;
                        obj.name = "grass";
                        break;
                    case 13: //enemy turret
                        GameObject turret = new GameObject("turret");
                        TurretScript turretScript = turret.AddComponent<TurretScript>() as TurretScript;
                        turretScript.gridLoc = new Vector2Int(xGrid, yGrid);
                        turret.transform.parent = enemies.transform;
                        turret.transform.position = (Vector3)gridPos[xGrid, yGrid] + new Vector3(0, 0, -0.1f);
                        gridObjects[xGrid, yGrid] = turretNum;

                        SpriteRenderer renderTurret = turret.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;
                        renderTurret.sprite = turretSprite;

                        render.sprite = grassSprite;
                        obj.name = "grass";
                        break;
                    case 14: //enemy turret
                        GameObject exit = new GameObject("exit");
                        exit.transform.parent = tileGroup.transform;
                        exit.transform.position = (Vector3)gridPos[xGrid, yGrid] + new Vector3(0, 0, -0.1f);

                        SpriteRenderer renderexit = exit.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;
                        renderexit.sprite = exitSprite;

                        render.sprite = grassSprite;
                        obj.name = "grass";

                        finishTile = new Vector2Int(xGrid, yGrid);
                        break;

                    default: //default tile grass
                        render.sprite = grassSprite;
                        obj.name = "grass";
                        break;
                }

                obj.transform.parent = tileGroup.transform;
                obj.transform.position = new Vector3(gridPos[xGrid, yGrid].x, gridPos[xGrid, yGrid].y, 0);
            }
        }
    }
}
