using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineGemScript : MonoBehaviour {

    public GameObject inventory, gridInfo;
    Inventory inventoryInfo;
    GameObject healthBarEmpty, healthBarFill;
    public int[,] gridObjects;
    public float healthMine = 100;
    public float healthDecay = 3;
    public bool gettingMined = false;
    public bool mined = false; //checks if mined
    int amountShards = 4; //amount of shards in the mine
    Vector2Int selectTileGridLoc; //location of the tile the player is looking at
    public Vector2Int tileLoc; //location of this object in tiles

    Sprite fillPixel;

    GameObject particleMined;

    AudioSource audioSource;
    AudioClip mineSound;


    void Start () {
        inventory = GameObject.Find("InventoryManager");
        inventoryInfo = inventory.GetComponent<Inventory>();
        gridInfo = GameObject.Find("GridManager");
        gridObjects = gridInfo.GetComponent<LevelLoader>().gridObjects;
        selectTileGridLoc = gridInfo.GetComponent<GridInfo>().selectTileGridLoc;

        //creates an healthbar with a lot of properties
        healthBarEmpty = new GameObject("healthBarEmpty");
        healthBarFill = new GameObject("healthBarFill");
        healthBarEmpty.transform.SetParent(transform);
        healthBarFill.transform.SetParent(transform);
        healthBarEmpty.SetActive(false);
        healthBarFill.SetActive(false);

        fillPixel = Resources.Load("Sprites/fillpixel", typeof(Sprite)) as Sprite;
        SpriteRenderer renderempty = healthBarEmpty.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;
        SpriteRenderer renderFill = healthBarFill.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;
        renderempty.sprite = fillPixel;
        renderFill.sprite = fillPixel;
        renderFill.color = Color.red;

        healthBarEmpty.transform.localScale = new Vector3(healthMine, 20, 0);
        healthBarEmpty.transform.position = transform.position + new Vector3(0, -0.23f, -0.15f);
        healthBarFill.transform.position = transform.position + new Vector3(0, -0.2f, -0.2f);

        GameObject particleSys = Instantiate(Resources.Load("Sprites/particles/mineParticleSys", typeof(GameObject))) as GameObject;
        particleSys.transform.SetParent(transform);
        particleSys.transform.position = transform.position + new Vector3(0.5f,-0.5f, -0.1f);

        particleMined = Resources.Load("Sprites/particles/minedparticles", typeof(GameObject)) as GameObject;

        mineSound = Resources.Load("Audio/mining", typeof(AudioClip)) as AudioClip;
        audioSource = gameObject.AddComponent<AudioSource>();
    }
	
	void Update () {

        if (gettingMined)
        {
            if (!audioSource.isPlaying)
                audioSource.PlayOneShot(mineSound);
        }
        else
            audioSource.Stop();

        //if mined it should be removed from the level
        if (mined)
        {
            inventoryInfo.gemShardsAmount += amountShards;
            gridObjects[tileLoc.x, tileLoc.y] = 0;
            Overlay.SpawnText(amountShards + " Shards", new Vector3(300, 300, 0), Color.green);
            GameObject particleSysMined = Instantiate(particleMined);
            particleSysMined.transform.position = transform.position + new Vector3(0.5f, -0.5f, -0.1f);
            SoundsManager.PlayAudio(0, "player");
            Destroy(gameObject);

            audioSource.Stop();
        }
        healthBarFill.transform.localScale = new Vector3(healthMine, 20, 0);

        gettingMined = false;
    }

    //if getting mined the healthbar should degrade
    public void GettingMined()
    {
        gettingMined = true;

        healthBarEmpty.SetActive(true);
        healthBarFill.SetActive(true);
        
        if (healthMine > 0)
            healthMine -= healthDecay;
        else
            mined = true;
    }
}
