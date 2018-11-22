using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundsManager : MonoBehaviour {

    public GameObject player;
    static AudioSource audioSourcePlayer;

    AudioClip collectGem, placeGem, drown, explode, bulletDeath, bulletHit;
    static ArrayList playerSounds = new ArrayList();
    // index 0 = collect gem / index 1 = place gem sound


    void Start () {
        audioSourcePlayer = player.GetComponent<AudioSource>();
        collectGem = Resources.Load("Audio/Win Sound", typeof(AudioClip)) as AudioClip;
        placeGem = Resources.Load("Audio/place Gem", typeof(AudioClip)) as AudioClip;
        drown = Resources.Load("Audio/drowndeath", typeof(AudioClip)) as AudioClip;
        explode = Resources.Load("Audio/explosion", typeof(AudioClip)) as AudioClip;
        bulletDeath = Resources.Load("Audio/bullet hit", typeof(AudioClip)) as AudioClip;
        bulletHit = Resources.Load("Audio/player hit", typeof(AudioClip)) as AudioClip;

        playerSounds.Add(collectGem);
        playerSounds.Add(placeGem);
        playerSounds.Add(drown);
        playerSounds.Add(explode);
        playerSounds.Add(bulletDeath);
        playerSounds.Add(bulletHit);
    }
	
	void Update () {
    }

    static public void PlayAudio(int index, string obj)
    {
        if(obj == "player")
            audioSourcePlayer.PlayOneShot((AudioClip)playerSounds[index]);
    }

}
