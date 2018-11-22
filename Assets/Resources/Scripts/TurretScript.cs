using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretScript : MonoBehaviour {

    GameObject player;
    GameObject turret;
    PlayerScript playerScript;

    Sprite gunSprite, bulletSprite;
    RaycastHit2D hitPlayer;

    SpriteRenderer turretBaseSprite;

    AudioSource audioSource;
    AudioClip laserShoot, enemyDetectedSound, explosion;

    public Vector2Int gridLoc;

    float timeSeen = 0;
    int timeTriggerAttack = 50;

    int reloadTimer;
    int reloadTime = 40;

    float sightDecay = 0.8f; 

    float degreesTurnedTurret;
    float prevRotation;

    bool enemyDetected = false; // when sound needs to be activated

    void Start () {
        player = GameObject.Find("robot");
        playerScript = player.GetComponent<PlayerScript>();

        gunSprite = Resources.Load("Sprites/Tiles/gun", typeof(Sprite)) as Sprite;
        bulletSprite = Resources.Load("Sprites/Tiles/bullet", typeof(Sprite)) as Sprite;

        turret = new GameObject("gun");
        turret.transform.SetParent(transform);
        turret.AddComponent<SpriteRenderer>().sprite = gunSprite;
        Vector2 size = GetComponent<Renderer>().bounds.size;
        turret.transform.position = transform.position + new Vector3(size.x / 2, -size.y / 2, -0.1f);

        turretBaseSprite = GetComponent<SpriteRenderer>();

        audioSource = gameObject.AddComponent<AudioSource>();
        laserShoot = Resources.Load("Audio/shoot Laser", typeof(AudioClip)) as AudioClip;
        enemyDetectedSound = Resources.Load("Audio/enemy detected", typeof(AudioClip)) as AudioClip;
        explosion = Resources.Load("Audio/explosion", typeof(AudioClip)) as AudioClip;
    }
	

	void Update () {
        if (PlayerInVision())
        {
            turretBaseSprite.color = Color.yellow;

            if (LookingAt())
            {
                timeSeen++;
            }

            if (timeSeen >= timeTriggerAttack)
            {
                turretBaseSprite.color = Color.red;

                timeSeen = timeTriggerAttack;
                Attacking();
            }
        }
        else if (timeSeen <= 0)
        {
            turretBaseSprite.color = Color.green;

            Scanning();
        }
        else
        {
            Searching();

            turretBaseSprite.color = new Color(1f, 0.5f, 0);

            timeSeen -= sightDecay;
        }
        
    }

    void Scanning()
    {
        turret.transform.Rotate(0, 0, 3.5f);
    }

    void Searching()
    {
        float random = Random.value;
        if (random < 0.05)
            prevRotation = -4;
        else if (random > 0.95)
            prevRotation = 4;

        turret.transform.Rotate(0, 0, -prevRotation);

        enemyDetected = false;
    }

    void Attacking()
    {
        if (reloadTimer <= 0)
        {
            Shoot();
            reloadTimer = reloadTime;
        }
        else
            reloadTimer--; 
    }

    //returns true if player is in sight
    bool PlayerInVision()
    {
        hitPlayer = Physics2D.Raycast(turret.transform.position, turret.transform.rotation * Vector2.right);
        if (hitPlayer)
        {
            if (hitPlayer.collider.gameObject.tag == "Player")
            {
                return true;
            }
        }

        return false;
    }

    bool LookingAt()
    {
        float offset;
        Vector2 dif = turret.transform.position - player.transform.position;

        offset = (dif.x > 0) ? 180 : 0;
        float degreesTurned = Mathf.Atan(dif.y / dif.x) * (180 / Mathf.PI);
        float prevDegreesTurned = degreesTurnedTurret;
        degreesTurnedTurret = degreesTurned + offset;
        turret.transform.rotation = Quaternion.Euler(0, 0, degreesTurnedTurret);

        if (PlayerInVision())
        {
            if (!enemyDetected)
            {
                audioSource.PlayOneShot(enemyDetectedSound);
                enemyDetected = true;
            }
            return true;
        }
        else
        {
            turret.transform.rotation = Quaternion.Euler(0, 0, prevDegreesTurned);
            return false;
        }
    }

    void Shoot()
    {
        GameObject bullet = new GameObject("bullet");
        bullet.AddComponent<SpriteRenderer>().sprite = bulletSprite;
        bullet.transform.position = turret.transform.position;
        bullet.transform.SetParent(transform);
        BulletScript bulletScript = bullet.AddComponent<BulletScript>();
        bulletScript.rotationDegrees = degreesTurnedTurret;

        audioSource.PlayOneShot(laserShoot);
    }

    public void DestroyTurret()
    {

        GameObject particleSys = Instantiate(Resources.Load("Sprites/particles/explosion", typeof(GameObject))) as GameObject;
        particleSys.transform.position = transform.position + new Vector3(0.5f, -0.5f, -0.1f);
        particleSys.AddComponent<AudioSource>().PlayOneShot(explosion);
        Destroy(gameObject);

    }

}
