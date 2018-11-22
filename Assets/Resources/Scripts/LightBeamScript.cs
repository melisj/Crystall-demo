using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBeamScript : MonoBehaviour
{

    public GameObject gridManager;
    public GameObject lights, mineables, interactables, enemies;
    GridInfo gridInformation;
    LineRenderer lineInfo;
    PlayerScript playerInfo;

    public Vector2[,] gridPos;
    public int[,] gridInfo, gridObjects;
    public Vector2Int tileLoc;

    public int direction;
    Vector2 vectorDirection;
    Material lineMat;

    public bool parentLight = true;
    public bool[] reflecionGems;

    public bool laser;

    RaycastHit2D hitPlayer;

    int raycastLength = 0;

    void Start()
    {
        mineables = GameObject.Find("Mineables");
        gridManager = GameObject.Find("GridManager");
        gridObjects = gridManager.GetComponent<LevelLoader>().gridObjects;
        gridInfo = gridManager.GetComponent<LevelLoader>().gridInfo;
        gridPos = gridManager.GetComponent<LevelLoader>().gridPos;
        gridInformation = gridManager.GetComponent<GridInfo>();
        lights = GameObject.Find("Lights");
        playerInfo = GameObject.Find("robot").GetComponent<PlayerScript>();
        interactables = GameObject.Find("Interactables");
        enemies = GameObject.Find("Enemies");

        lineInfo = gameObject.AddComponent(typeof(LineRenderer)) as LineRenderer;
        lineInfo.useWorldSpace = false;
        lineInfo.material = Resources.Load("Sprites/lineMat 1", typeof(Material)) as Material;
        if (!laser)
        {
            lineInfo.widthCurve = AnimationCurve.Linear(0.3f, 0.3f, 0.3f, 0.3f);
            lineInfo.startColor = new Color(0.8f, 0.8f, 0, 0.7f);
            lineInfo.endColor = new Color(0.8f, 0.8f, 0.8f, 0.5f);  
        }
        else
        {
            lineInfo.widthCurve = AnimationCurve.Linear(0.1f, 0.1f, 0.1f, 0.1f);
            lineInfo.startColor = new Color(0.8f, 0f, 0, 0.7f);
            lineInfo.endColor = new Color(0.8f, 0f, 0f, 0.5f);
        }
        transform.position -= new Vector3(-0.5f, 0.5f, 1);
    }

    void Update()
    {
        if(vectorDirection.x == 0)
            switch (direction)
            {
                case 1:
                    vectorDirection = Vector2.up;
                    break;
                case 2:
                    vectorDirection = Vector2.down;
                    break;
                case 3:
                    vectorDirection = Vector2.left;
                    break;
                case 4:
                    vectorDirection = Vector2.right;
                    break;
            }

        Vector2 oldEndPos = lineInfo.GetPosition(1);

        if (parentLight)
        {
            int amountLightGems = 0;
            for (int iMine = 0; iMine < mineables.transform.childCount; iMine++)
            {
                Transform childMine = mineables.transform.GetChild(iMine);
                if (childMine.transform.name == "gem")
                {
                    GemScript script = childMine.GetComponent(typeof(GemScript)) as GemScript;
                    if (script.gemNum == 0 || script.gemNum == 3)

                        amountLightGems++;
                }
            }
            reflecionGems = new bool[amountLightGems];
        }

        DetectEndLine();

        if (oldEndPos != (Vector2)lineInfo.GetPosition(1))
        {
            for (int iChild = 0; iChild < transform.childCount; iChild++)
                Destroy(transform.GetChild(iChild).gameObject);
        }

        if (transform.childCount != 0)
        {
            foreach (Transform lightChild in transform)
            {
                LightBeamScript script = lightChild.GetComponent(typeof(LightBeamScript)) as LightBeamScript;
                script.reflecionGems = reflecionGems;
            }
        }
        if (laser)
        {
            hitPlayer = Physics2D.Raycast(transform.position, vectorDirection, raycastLength);
            if (hitPlayer)
            {
                if(hitPlayer.collider.gameObject.tag != "Player")
                {
                    Debug.DrawRay(transform.position, vectorDirection, Color.yellow);
                }
                else if (hitPlayer.collider.gameObject.tag == "Player")
                {
                    playerInfo.temp += 5;
                } 
            }
        }
    }

    void DetectEndLine()
    {
        int nextTileX = tileLoc.x;
        int nextTileY = tileLoc.y;
        bool endNotFound = true;
        int emergencyBrake = 0;
        raycastLength = 0;
        Vector2 offset;

        while (endNotFound)
        {
            if (direction == 1)
            {
                nextTileY--;
                offset = new Vector2(0, 0.5f);
            }
            else if (direction == 2)
            {
                nextTileY++;
                offset = new Vector2(0, -0.5f);
            }
            else if (direction == 3)
            {
                nextTileX--;
                offset = new Vector2(-0.5f, 0);
            }
            else
            {
                nextTileX++;
                offset = new Vector2(0.5f, 0);
            }
            if (gridObjects[nextTileX, nextTileY] >= 2 && gridObjects[nextTileX, nextTileY] <= 7)
            {
                ActivateGems(nextTileX, nextTileY);
                offset = Vector2.zero;
            }
            if (CheckLightGem(nextTileX, nextTileY) && transform.childCount == 0)
            {
                if (gridObjects[nextTileX, nextTileY] == 2)
                    CreateNewLines(nextTileX, nextTileY, true);
                else if (gridObjects[nextTileX, nextTileY] == 5)
                    CreateNewLines(nextTileX, nextTileY, false);
            }
            if (gridObjects[nextTileX, nextTileY] == 8)
                ActivateMachine(nextTileX, nextTileY, direction);

            if (laser)
            {
                if (gridObjects[nextTileX, nextTileY] == 12)
                    DestroyTurret(nextTileX, nextTileY);
            }

            raycastLength++;

            if (gridInfo[nextTileX, nextTileY] == 1 || gridObjects[nextTileX, nextTileY] != 0)
            {
                lineInfo.SetPosition(1, gridPos[nextTileX, nextTileY] - gridPos[tileLoc.x, tileLoc.y] - offset);
                endNotFound = false;
            }
            emergencyBrake++;
            if (emergencyBrake == 100)
                endNotFound = false;
        }
    }

    void CreateNewLines(int xGrid, int yGrid, bool lightBeam)
    {
        if(lightBeam)
        for (int i = 0; i < 2; i++)
        {
            int newDirection = 0;
            if (direction <= 2)
                newDirection = 3 + i;
            if (direction >= 3)
                newDirection = 1 + i;
            GameObject light = new GameObject("light");
            LightBeamScript lightScript = light.AddComponent(typeof(LightBeamScript)) as LightBeamScript;
            light.transform.position = new Vector3(gridPos[xGrid, yGrid].x, gridPos[xGrid, yGrid].y, 0);
            lightScript.tileLoc = new Vector2Int(xGrid, yGrid);
            light.transform.SetParent(transform);
            lightScript.direction = newDirection;
            lightScript.parentLight = false;
            if(laser)
                    lightScript.laser = true;
        }
        else
        {
            GameObject light = new GameObject("laser");
            LightBeamScript lightScript = light.AddComponent(typeof(LightBeamScript)) as LightBeamScript;
            light.transform.position = new Vector3(gridPos[xGrid, yGrid].x, gridPos[xGrid, yGrid].y, 0);
            lightScript.tileLoc = new Vector2Int(xGrid, yGrid);
            light.transform.SetParent(transform);
            lightScript.direction = direction;
            lightScript.parentLight = false;
            lightScript.laser = true;
        }

        
    }

    bool CheckLightGem(int xGrid, int yGrid)
    {
        int indexLightGems = 0;
        for (int iMine = 0; iMine < mineables.transform.childCount; iMine++)
        {
            Transform childMine = mineables.transform.GetChild(iMine);
            if (childMine.transform.name == "gem")
            {
                GemScript script = childMine.GetComponent(typeof(GemScript)) as GemScript;
                if (script.gemNum == 0 || script.gemNum == 3)
                {
                    if (script.tileLoc.x == xGrid && script.tileLoc.y == yGrid)
                    {
                        if (reflecionGems[indexLightGems] == false)
                        {
                            reflecionGems[indexLightGems] = true;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    indexLightGems++;
                }
            }
        }
        return false;
    }

    void ActivateGems(int xGrid, int yGrid)
    {
        for (int iMine = 0; iMine < mineables.transform.childCount; iMine++)
        {
            Transform childMine = mineables.transform.GetChild(iMine);
            if (childMine.transform.name == "gem")
            {
                GemScript script = childMine.GetComponent(typeof(GemScript)) as GemScript;
                if (script.tileLoc.x == xGrid && script.tileLoc.y == yGrid)
                    if (script.gemNum == 1 || script.gemNum == 2)
                        childMine.GetComponent<HeatCoolGem>().activated = true;
            }
        }
    }

    void ActivateMachine(int xGrid, int yGrid, int direction)
    {
        for (int iAct = 0; iAct < interactables.transform.childCount; iAct++)
        {
            Transform childAct = interactables.transform.GetChild(iAct);
            if (childAct.transform.name == "machine")
            {
                MachineScript script = childAct.GetComponent(typeof(MachineScript)) as MachineScript;
                if (script.gridLoc.x == xGrid && script.gridLoc.y == yGrid)
                    if (direction == script.direction)
                        script.activated = true;
            }
        }
    }

    void DestroyTurret(int xGrid, int yGrid)
    {
        for (int iTurret = 0; iTurret < enemies.transform.childCount; iTurret++)
        {
            Transform childTurret = enemies.transform.GetChild(iTurret);
            if (childTurret.transform.name == "turret")
            {
                TurretScript script = childTurret.GetComponent(typeof(TurretScript)) as TurretScript;
                if (script.gridLoc.x == xGrid && script.gridLoc.y == yGrid)
                {
                    script.DestroyTurret();
                    gridInformation.gridObjects[xGrid, yGrid] = 0;
                }
            }
        }
    }
}
