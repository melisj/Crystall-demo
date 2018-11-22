using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodScript : MonoBehaviour {

    public Vector2Int tileLoc;
    public int[,] gridHeat;


    bool burning = false;
    int burnTimer = 180;

    Sprite woodSprite;
    SpriteRenderer spriteRender;

    GameObject particleSys;

    AudioSource audioSource;
    AudioClip fire;

    void Start() {
        gridHeat = GameObject.Find("GridManager").GetComponent<GridInfo>().gridHeat;

        spriteRender = gameObject.AddComponent<SpriteRenderer>();
        woodSprite = Resources.Load("Sprites/Tiles/wood", typeof(Sprite)) as Sprite;
        spriteRender.sprite = woodSprite;

        audioSource = gameObject.AddComponent<AudioSource>();
        fire = Resources.Load("Audio/fire", typeof(AudioClip)) as AudioClip;
    }

    void Update() {


        if (burning)
            BurnWood();
        else
        {
            if (gridHeat[tileLoc.x, tileLoc.y] / 10 > gridHeat[tileLoc.x, tileLoc.y] % 10)
            {
                burning = true;

                particleSys = Instantiate(Resources.Load("Sprites/particles/fireparticles", typeof(GameObject))) as GameObject;
                particleSys.transform.position = transform.position + new Vector3(0.5f, -0.5f, -0.1f);
            }
        }
    }

    void BurnWood()
    {
        if (!audioSource.isPlaying)
            audioSource.PlayOneShot(fire);

        burnTimer--;

        if (burnTimer <= 0)
        {
            GameObject.Find("GridManager").GetComponent<LevelLoader>().gridObjects[tileLoc.x, tileLoc.y] = 0;
            audioSource.Stop();
            Destroy(gameObject);
        }
    }

}
