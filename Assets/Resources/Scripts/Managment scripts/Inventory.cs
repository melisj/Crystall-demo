using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public string[] gemNames = {"light", "heat", "cool", "laser"};

    public int gemShardsAmount = 0,
        lightGemAmount = 0,
        heatGemAmount = 0,
        coolGemAmount = 0,
        laserGemAmount = 0,
        lightGemCost = 2,
        heatGemCost = 5,
        coolGemCost = 5,
        laserGemCost = 8;



    public int ReturnAmount(int gemNum)
    {
       switch (gemNum)
        {
            case 0:
                return lightGemAmount;
            case 1:
                return heatGemAmount;
            case 2:
                return coolGemAmount;
            case 3:
                return laserGemAmount;
        }
        return 0;
    }

    void Start()
    {
    }

    void Update()
    {
        
    }

    //checks if gem can be added by checking amount of shards
    public void AddGem(string name)
    {
        switch (name)
        {
            case "light":
                if (gemShardsAmount >= lightGemCost)
                {
                    lightGemAmount++;
                    gemShardsAmount -= lightGemCost;
                    Overlay.SpawnText("+1 " + gemNames[0] + "gem", new Vector3(300, 300, 0), Color.green);
                    SoundsManager.PlayAudio(0, "player");
                }
                break;
            case "heat":
                if (gemShardsAmount >= heatGemCost)
                {
                    heatGemAmount++;
                    gemShardsAmount -= heatGemCost;
                    Overlay.SpawnText("+1 " + gemNames[1] + "gem", new Vector3(300, 300, 0), Color.green);
                    SoundsManager.PlayAudio(0, "player");
                }
                break;
            case "cool":
                if (gemShardsAmount >= coolGemCost)
                {
                    coolGemAmount++;
                    gemShardsAmount -= coolGemCost;
                    Overlay.SpawnText("+1 " + gemNames[2] + "gem", new Vector3(300, 300, 0), Color.green);
                    SoundsManager.PlayAudio(0, "player");
                } 
                break;
            case "laser":
                if (gemShardsAmount >= laserGemCost)
                {
                    laserGemAmount++;
                    gemShardsAmount -= laserGemCost;
                    Overlay.SpawnText("+1 " + gemNames[3] + "gem", new Vector3(300, 300, 0), Color.green);
                    SoundsManager.PlayAudio(0, "player");
                }
                break;
        }
    }

    //method to place and pickup an gem, if bool is true the gem will be placed if enough gems are available in the inventory
    //if false the gem will be picked up and added to the inventory
    public bool PlacePickUp(int gemNum, bool placePickUp)
    {
        Color color;
        int addRemoveNum = 0;
        if (placePickUp)
        {
            addRemoveNum--;
            color = Color.red;
        }
        else
        {
            addRemoveNum++;
            color = Color.green;
        }

        switch (gemNum)
        {
            case 0:
                lightGemAmount += addRemoveNum;
                if (lightGemAmount < 0)
                {
                    lightGemAmount -= addRemoveNum;
                    return false;
                }
                Overlay.SpawnText(addRemoveNum + " " + gemNames[0] + "gem", new Vector3(300, 300, 0), color);
                break;
            case 1:
                heatGemAmount += addRemoveNum;
                if (heatGemAmount < 0)
                {
                    heatGemAmount -= addRemoveNum;
                    return false;
                }
                Overlay.SpawnText(addRemoveNum + " " + gemNames[1] + "gem", new Vector3(300, 300, 0), color);
                break;
            case 2:
                coolGemAmount += addRemoveNum;
                if (coolGemAmount < 0)
                {
                    coolGemAmount -= addRemoveNum;
                    return false;
                }
                Overlay.SpawnText(addRemoveNum + " " + gemNames[2] + "gem", new Vector3(300, 300, 0), color);
                break;
            case 3:
                laserGemAmount += addRemoveNum;
                if (laserGemAmount < 0)
                {
                    laserGemAmount -= addRemoveNum;
                    return false;
                }
                Overlay.SpawnText(addRemoveNum + " " + gemNames[3] + "gem", new Vector3(300, 300, 0), color);
                break;
        }
        return true;
    }
}
