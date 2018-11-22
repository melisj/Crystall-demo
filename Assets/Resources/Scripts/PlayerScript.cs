using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour {

    public float temp = 0;
    public float maxTemp = 200;
    float tempRestoreSpeed = 0.3f;
    float cameraSpeed = 0.05f; // 0 = laag / 1= hoog
    float speed = 4f;
    float rotation;
    public int direction; // 1 = up / 2 = down / 3 = left / 4 = right
    public Vector3 velocity;

    Inventory inventory;
    LevelLoader loader;
    SpriteRenderer spriteRenderer;
    public GameObject tiles, mineables, camera, gridManager, inventoryManager, interactables, enemies;

    int[,] gridInfo;
    int[,] gridObjects;
    Vector2[,] gridPos;
    Vector2Int gridLocationSelect;
    Vector2Int gridLocation;
    Vector2 size;

    GameObject damagedParticle;

    public int modes; //0 = mining, 1+ = placing 

    bool startDeath;
    int deathTimer = 180;

    void Start()
    {
        loader = gridManager.GetComponent<LevelLoader>();
        gridInfo = loader.gridInfo;
        gridPos = loader.gridPos;
        gridObjects = loader.gridObjects;
        inventory = inventoryManager.GetComponent<Inventory>();
        size = GetComponent<Renderer>().bounds.size;
        spriteRenderer = GetComponent<SpriteRenderer>();

        damagedParticle = Resources.Load("Sprites/particles/robotDamaged", typeof(GameObject)) as GameObject;
    }

    void Update()
    {
        if (startDeath)
            deathTimer--;
        if(deathTimer <= 0)
            SceneManager.LoadScene("gameover");

        velocity = Vector3.zero;

        gridLocationSelect = gridManager.GetComponent<GridInfo>().selectTileGridLoc;
        gridLocation = gridManager.GetComponent<GridInfo>().playerGridLoc;
        transform.rotation = new Quaternion(0, 0, 0, 0);

        CheckInput();
        CheckRotation();

        transform.position += velocity * Time.deltaTime;

        if (Mathf.Abs(temp) > maxTemp)
        {
            if (!startDeath)
                SoundsManager.PlayAudio(3, "player");
            Die();
            
            if (temp > 0)
                temp = maxTemp;
            else
                temp = -maxTemp;
        }
        if (Mathf.Abs(temp) > 0)
        {
            temp += (temp > 0) ? -tempRestoreSpeed : tempRestoreSpeed;
        }
        if (Mathf.Abs(temp) < 0.5f)
            temp = 0;

        //checks collision for all tiles and mineable objects in the level
        for (int iTile = 0; iTile < tiles.transform.childCount; iTile++)
        {
            Transform childTile = tiles.transform.GetChild(iTile);
            if (childTile.name == "wall")
                Collision(childTile, true);
        }
        for (int iMine = 0; iMine < mineables.transform.childCount; iMine++)
        {
            Transform childTile = mineables.transform.GetChild(iMine);
            Collision(childTile, true);
        }
        for (int iAct = 0; iAct < interactables.transform.childCount; iAct++)
        {
            Transform childTile = interactables.transform.GetChild(iAct);
            if (childTile.name == "water")
            {
                WaterScript waterScript = childTile.GetComponent<WaterScript>();
                if (gridLocation == waterScript.tileLoc)
                    if (!waterScript.ice)
                    {
                        if (!startDeath)
                            SoundsManager.PlayAudio(2, "player");
                        Die();
                        
                        break;
                    }
                if (!waterScript.ice)
                    Collision(childTile, true);
            }
            else
            {
                Collision(childTile, true);
                if (childTile.childCount != 0)
                    Collision(childTile.GetChild(0), true);
            }
        }
        for (int iEnemy = 0; iEnemy < enemies.transform.childCount; iEnemy++)
        {
            Transform childTile = enemies.transform.GetChild(iEnemy);
            Collision(childTile, true);
        }

        Mining();
        PlaceGem();

        //camera is locked to the player
        camera.transform.position = Vector3.Lerp(camera.transform.position,
            transform.position - new Vector3(-GetComponent<Renderer>().bounds.size.x / 2, GetComponent<Renderer>().bounds.size.y / 2, 10),
            cameraSpeed);

        spriteRenderer.color = (temp > 0) ? new Color(1, 1 - temp / maxTemp, 1 - temp / maxTemp) : new Color(1 - -temp / maxTemp, 1 - -temp / maxTemp, 1);
    }

    bool Collision(Transform obj, bool placeBack)
    {
        Vector2 objPos, thisPos, objSize;
        if (obj.GetComponent<SpriteRenderer>() == null)
            return false;
        objSize = obj.GetComponent<Renderer>().bounds.size;
        objPos = obj.transform.position;
        thisPos = (Vector2)transform.position - new Vector2(size.x / 2, -size.y / 2);

        //checks for collision
        if (thisPos.y - size.y < objPos.y &&
            thisPos.y > objPos.y - objSize.y &&
            thisPos.x + size.x > objPos.x &&
            thisPos.x < objPos.x + objSize.x)
        {
            if (placeBack)
            {
                //calulates the distance it has to be set back
                float distanceYdown = thisPos.y - size.y - objPos.y;
                float distanceYup = objPos.y - objSize.y - thisPos.y;
                float distanceXright = thisPos.x + size.x - objPos.x;
                float distanceXleft = objPos.x + objSize.x - thisPos.x;

                float distanceY = (distanceYdown >= distanceYup) ? -distanceYdown : distanceYup;
                float distanceX = (distanceXright >= distanceXleft) ? distanceXleft : -distanceXright;

                if (Mathf.Abs(distanceY) > Mathf.Abs(distanceX))
                    transform.position += new Vector3(distanceX, 0, 0);

                else if (Mathf.Abs(distanceY) < Mathf.Abs(distanceX))
                    transform.position += new Vector3(0, distanceY, 0);
            }
            return true;
        }
        return false;
    }

    void Mining()
    {
        if (Input.GetKey(KeyCode.Space) && modes == 0)
        {
            for (int iMine = 0; iMine < mineables.transform.childCount; iMine++)
            {
                Transform childTile = mineables.transform.GetChild(iMine);
                Vector3 childTilePos = childTile.transform.position + new Vector3(0, 0, 0.1f);
                Vector3 positionInGrid = gridPos[gridLocationSelect.x, gridLocationSelect.y];

                if (childTilePos.y <= positionInGrid.y &&
                    childTilePos.y > positionInGrid.y - 1 &&
                    childTilePos.x >= positionInGrid.x &&
                    childTilePos.x < positionInGrid.x + 1)
                {
                    int gridNum = gridObjects[gridLocationSelect.x, gridLocationSelect.y];

                    if (gridNum == loader.mineNum)
                    {
                        childTile.GetComponent<MineGemScript>().GettingMined();
                    }
                    if (gridNum >= loader.lightGemNum)
                    {
                        if (inventory.PlacePickUp(gridNum - 2, false))
                        {
                            childTile.GetComponent<GemScript>().RemoveGem();
                            SoundsManager.PlayAudio(0, "player");
                        }
                    }
                }
            }
        }
    }

    void PlaceGem()
    {
        if (Input.GetKeyDown(KeyCode.Space) && modes >= 1)
        {
            if (gridObjects[gridLocationSelect.x, gridLocationSelect.y] == 0 && gridInfo[gridLocationSelect.x, gridLocationSelect.y] != 1)
            {
                if (inventory.PlacePickUp(modes - 1, true))
                {
                    GameObject obj = new GameObject("gem");
                    GemScript script = obj.AddComponent<GemScript>() as GemScript;
                    script.gemNum = modes - 1;
                    script.tileLoc = new Vector2Int(gridLocationSelect.x, gridLocationSelect.y);
                    obj.transform.parent = mineables.transform;
                    obj.transform.position = (Vector3)gridPos[gridLocationSelect.x, gridLocationSelect.y] + new Vector3(0, 0, -0.1f);
                    if (modes == 2)
                        obj.AddComponent<HeatCoolGem>().heatCoolGem = true;
                    if (modes == 3)
                        obj.AddComponent<HeatCoolGem>().heatCoolGem = false;

                    gridObjects[gridLocationSelect.x, gridLocationSelect.y] = modes + 1;

                    SoundsManager.PlayAudio(1, "player");
                }
                else
                {
                    Overlay.SpawnText("No gems in this catogory", new Vector3(300, 300, 0), Color.red);
                }
            }
        }
    }

    void CheckRotation()
    {
        if (direction == 1)
            rotation = 90;
        else if (direction == 2)
            rotation = -90;
        else if (direction == 3)
            rotation = 180;
        else if (direction == 4)
            rotation = 0;

        transform.rotation = Quaternion.Euler(0, 0, rotation);
    }

    void CheckInput()
    {
        //checks which direction the player is facing and what velocity it should get
        if (Input.GetKey(KeyCode.W))
        {
            velocity = new Vector3(0, speed);
            direction = 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            velocity = new Vector3(-speed, 0);
            direction = 3;
        }
        if (Input.GetKey(KeyCode.S))
        {
            velocity = new Vector3(0, -speed);
            direction = 2;
        }
        if (Input.GetKey(KeyCode.D))
        {
            velocity = new Vector3(speed, 0);
            direction = 4;
        }

        //checks which key is pressed and assigns the mode
        if (Input.GetKey(KeyCode.Alpha1) || Input.GetKey(KeyCode.Keypad1))
            modes = 0;
        else if (Input.GetKey(KeyCode.Alpha2) || Input.GetKey(KeyCode.Keypad2))
            modes = 1;
        else if (Input.GetKey(KeyCode.Alpha3) || Input.GetKey(KeyCode.Keypad3))
            modes = 2;
        else if (Input.GetKey(KeyCode.Alpha4) || Input.GetKey(KeyCode.Keypad4))
            modes = 3;
        else if (Input.GetKey(KeyCode.Alpha5) || Input.GetKey(KeyCode.Keypad5))
            modes = 4;
    }

    public void Die()
    {
        startDeath = true;
        Overlay.DeathScreen();
        camera.GetComponent<AudioSource>().Stop();
    }

    public void Hit()
    {
        GameObject particleSys = Instantiate(damagedParticle);
        particleSys.transform.position = transform.position + new Vector3(0f, 0f, -0.1f);
        particleSys.GetComponent<ParticleSystem>().startColor = spriteRenderer.color;
        SoundsManager.PlayAudio(5, "player");
    }
}
