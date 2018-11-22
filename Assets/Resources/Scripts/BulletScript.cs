using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour {

    public float rotationDegrees;
    float speed = 0.08f;

    Vector2 size;
    GameObject tiles, mineables, interactables, enemies, player;

    void Start () {
        size = GetComponent<SpriteRenderer>().bounds.size;

        tiles = GameObject.Find("Tiles");
        mineables = GameObject.Find("Mineables");
        interactables = GameObject.Find("Interactables");
        enemies = GameObject.Find("Enemies");
        player = GameObject.Find("robot");
    }
	
	void Update () {

        float rotationRadial = rotationDegrees * (Mathf.PI / 180);

        float ySpeed = Mathf.Sin(rotationRadial);
        float xSpeed = Mathf.Cos(rotationRadial);

        Vector3 speedVector = new Vector3(xSpeed, ySpeed, 0);
        speedVector.Normalize();
        transform.position += speedVector * speed;


        //checks collision for all tiles and mineable objects in the level
        for (int iTile = 0; iTile < tiles.transform.childCount; iTile++)
        {
            Transform childTile = tiles.transform.GetChild(iTile);
            if (childTile.name == "wall")
                Collision(childTile, false);
        }
        for (int iMine = 0; iMine < mineables.transform.childCount; iMine++)
        {
            Transform childTile = mineables.transform.GetChild(iMine);
            Collision(childTile, false);
        }
        for (int iAct = 0; iAct < interactables.transform.childCount; iAct++)
        {
            Transform childTile = interactables.transform.GetChild(iAct);
            if (childTile.name != "water")
                Collision(childTile, false);
                if (childTile.childCount != 0 && childTile.name == "machine")
                    Collision(childTile.GetChild(0), false);
        }
        for (int iEnemy = 0; iEnemy < enemies.transform.childCount; iEnemy++)
        {
            Transform childTile = enemies.transform.GetChild(iEnemy);
            if(childTile != transform.parent)
                Collision(childTile, false);
        }
        Collision(player.transform, true);

    }

    void Collision(Transform obj, bool centeredObj)
    {
        if (obj.GetComponent<SpriteRenderer>() == null)
            return;
        Vector2 objPos, thisPos, objSize;
        objSize = obj.GetComponent<SpriteRenderer>().bounds.size;
        objPos = obj.transform.position;
        thisPos = (Vector2)transform.position - new Vector2(size.x / 2, -size.y / 2);

        // checks for collision

        if (centeredObj)
        {
            if (thisPos.y - size.y < objPos.y + objSize.y / 2 &&
                thisPos.y > objPos.y - objSize.y / 2 &&
                thisPos.x + size.x > objPos.x - objSize.x / 2 &&
                thisPos.x < objPos.x + objSize.x / 2)
            {
                SoundsManager.PlayAudio(4, "player");
                Destroy(gameObject);
                if (obj.gameObject.tag == "Player")
                {
                    PlayerScript playerScript = obj.gameObject.GetComponent<PlayerScript>();

                    playerScript.temp += 150;
                    playerScript.Hit();
                }
            }
        }
        else
        {
            if (thisPos.y - size.y < objPos.y &&
                thisPos.y > objPos.y - objSize.y &&
                thisPos.x + size.x > objPos.x &&
                thisPos.x < objPos.x + objSize.x)
            {

                SoundsManager.PlayAudio(4, "player");
                Destroy(gameObject);
            }
        }
    }
}
