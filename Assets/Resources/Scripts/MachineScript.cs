using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineScript : MonoBehaviour {


    public GameObject gridInfo;
    public int[,] gridObjects;
    Vector2[,] gridPos;

    public int direction;
    public Vector2Int gridLoc;
    public Vector2Int gateGridLoc;

    SpriteRenderer textureState;
    Sprite rotation0, rotation90, rotation180, rotation270, gate0, gate90, gate180, gate270;

    AudioSource audioSource;
    AudioClip open, close;

    public bool activated = false;
    public bool firstTimeLoad = true;


    void Start () {
        rotation0 = Resources.Load("Sprites/Tiles/machine0", typeof(Sprite)) as Sprite;
        rotation90 = Resources.Load("Sprites/Tiles/machine90", typeof(Sprite)) as Sprite;
        rotation180 = Resources.Load("Sprites/Tiles/machine180", typeof(Sprite)) as Sprite;
        rotation270 = Resources.Load("Sprites/Tiles/machine270", typeof(Sprite)) as Sprite;
        gate0 = Resources.Load("Sprites/Tiles/gate0", typeof(Sprite)) as Sprite;
        gate90 = Resources.Load("Sprites/Tiles/gate90", typeof(Sprite)) as Sprite;
        gate180 = Resources.Load("Sprites/Tiles/gate180", typeof(Sprite)) as Sprite;
        gate270 = Resources.Load("Sprites/Tiles/gate270", typeof(Sprite)) as Sprite;

        gridInfo = GameObject.Find("GridManager");
        gridObjects = gridInfo.GetComponent<LevelLoader>().gridObjects;
        gridPos = gridInfo.GetComponent<LevelLoader>().gridPos;
        textureState = gameObject.AddComponent<SpriteRenderer>() as SpriteRenderer;

        audioSource = gameObject.AddComponent<AudioSource>();

        open = Resources.Load("Audio/Open Gate", typeof(AudioClip)) as AudioClip;
        close = Resources.Load("Audio/Close Gate", typeof(AudioClip)) as AudioClip;

        gateGridLoc = gridLoc;
    }

	void Update () {

        if (transform.childCount == 0 && !activated)
            SpawnGate();
        else if (transform.childCount != 0 && activated)
            DeleteGate();

        activated = false;
        firstTimeLoad = false;
    }

    void SpawnGate()
    {
        GameObject gateObj = new GameObject("gate");
        SpriteRenderer gateSprite = gateObj.AddComponent<SpriteRenderer>() as SpriteRenderer;
        gateObj.transform.SetParent(transform);
        gateGridLoc = gridLoc;

        if (direction == 4)
        {
            textureState.sprite = rotation0;
            gateSprite.sprite = gate0;
            gateObj.transform.localPosition = new Vector3(0,-1,-0.1f);
            gateGridLoc += new Vector2Int(0, 1);
        }
        else if (direction == 3)
        {
            textureState.sprite = rotation180;
            gateSprite.sprite = gate180;
            gateObj.transform.localPosition = new Vector3(0, 1, -0.1f);
            gateGridLoc += new Vector2Int(0, -1);
        }
        else if (direction == 2)
        {
            textureState.sprite = rotation270;
            gateSprite.sprite = gate270;
            gateObj.transform.localPosition = new Vector3(-1, 0, -0.1f);
            gateGridLoc += new Vector2Int(-1, 0);
        }
        else
        {
            textureState.sprite = rotation90;
            gateSprite.sprite = gate90;
            gateObj.transform.localPosition = new Vector3(1, 0, -0.1f);
            gateGridLoc += new Vector2Int(1, 0);
        }

        if (gridObjects[gateGridLoc.x, gateGridLoc.y] == 0)
        {
            gridObjects[gateGridLoc.x, gateGridLoc.y] = gridInfo.GetComponent<LevelLoader>().gateNum;
            if (!firstTimeLoad)
                audioSource.PlayOneShot(close);
        }
        else
            Destroy(gateObj.gameObject);
    }

    public void DeleteGate()
    {
        if (transform.childCount != 0)
        {
            gridObjects[gateGridLoc.x, gateGridLoc.y] = 0;

            Transform gate = transform.GetChild(0);
            Destroy(gate.gameObject);
        }
        if (!firstTimeLoad)
            audioSource.PlayOneShot(open);
    }
}
