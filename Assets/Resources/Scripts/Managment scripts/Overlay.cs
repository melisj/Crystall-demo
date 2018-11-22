using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Overlay : MonoBehaviour {

    public GameObject inventory, player, popUp, optionsPopUp;
    public Text shardsDiplay, lightGemDisplay, heatGemDisplay, coolGemDisplay, laserGemDisplay;
    public Image modeDiplay0, modeDiplay1, modeDiplay2, modeDiplay3, modeDiplay4;
    public Button popUpAddButton;
    public Slider tempBar;
    Inventory inventoryInfo;
    PlayerScript playerInfo;

    public GameObject[] hintPopUp = new GameObject[4];

    Sprite exit, open;

    static Transform canvas;
    static GameObject hintBar, deathScreen;
    static int hintTimer = 0;
    static int hintTimes = 10;

    static public Text popUpTextPrefab;
    static ArrayList popUpTexts = new ArrayList();

    bool inventoryPopUp = true;
    bool optionsPopUpVisible = false;
    bool hintPopUpVisible = false;

    int hint2Timer = 600;

    Color modeSelectColor = Color.white, 
        modeEmptyColor = new Color(50f/255f,50f/255f,50f/255f), 
        modeEmptySelectedColor = new Color(100f/255f, 50f/255f, 50f/255f), 
        modeNotSelectedColor = new Color(150f/255f, 150f/255f, 150f/255f);

    // Use this for initialization
    void Start () {
        inventoryInfo = inventory.GetComponent<Inventory>();
        playerInfo = player.GetComponent<PlayerScript>();

        tempBar.GetComponent<Slider>().maxValue = playerInfo.maxTemp;

        hintBar = GameObject.Find("Hintbar");
        deathScreen = GameObject.Find("DeathScreen");
        deathScreen.SetActive(false);
        popUpTextPrefab = Resources.Load("Prefabs/popuptext", typeof(Text)) as Text;
        exit = Resources.Load("Sprites/popup/exitbutton", typeof(Sprite)) as Sprite;
        open = Resources.Load("Sprites/popup/openbutton", typeof(Sprite)) as Sprite;
        canvas = transform;
    }

    // Update is called once per frame
    void Update()
    {

        int mode = playerInfo.modes;
        float temp = playerInfo.temp;

        //the text diplay
        shardsDiplay.text = "Shards: " + inventoryInfo.gemShardsAmount;
        lightGemDisplay.text = "LIGHT: " + inventoryInfo.lightGemAmount;
        heatGemDisplay.text = "HEAT: " + inventoryInfo.heatGemAmount;
        coolGemDisplay.text = "COOL: " + inventoryInfo.coolGemAmount;
        laserGemDisplay.text = "LASER: " + inventoryInfo.laserGemAmount;

        modeDiplay0.color = modeNotSelectedColor;

        modeDiplay1.color = modeNotSelectedColor;
        if (inventoryInfo.lightGemAmount == 0)
            modeDiplay1.color = modeEmptyColor;

        modeDiplay2.color = modeNotSelectedColor;
        if (inventoryInfo.heatGemAmount == 0)
            modeDiplay2.color = modeEmptyColor;

        modeDiplay3.color = modeNotSelectedColor;
        if (inventoryInfo.coolGemAmount == 0)
            modeDiplay3.color = modeEmptyColor;

        modeDiplay4.color = modeNotSelectedColor;
        if (inventoryInfo.laserGemAmount == 0)
            modeDiplay4.color = modeEmptyColor;

        switch (mode)
        {
            case 0:
                modeDiplay0.color = modeSelectColor;
                break;
            case 1:
                modeDiplay1.color = modeSelectColor;
                if (inventoryInfo.lightGemAmount == 0)
                    modeDiplay1.color = modeEmptySelectedColor;
                break;
            case 2:
                modeDiplay2.color = modeSelectColor;
                if (inventoryInfo.heatGemAmount == 0)
                    modeDiplay2.color = modeEmptySelectedColor;
                break;
            case 3:
                modeDiplay3.color = modeSelectColor;
                if (inventoryInfo.coolGemAmount == 0)
                    modeDiplay3.color = modeEmptySelectedColor;
                break;
            case 4:
                modeDiplay4.color = modeSelectColor;
                if (inventoryInfo.laserGemAmount == 0)
                    modeDiplay4.color = modeEmptySelectedColor;
                break;
        }

        tempBar.gameObject.SetActive(true);
        if (temp > 0)
        {
            tempBar.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = Color.red;
        }
        else if (temp < 0)
        {
            tempBar.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = Color.blue;
        }
        else if (temp == 0)
        {
            tempBar.gameObject.SetActive(false);
        }
        tempBar.value = Mathf.Abs(temp);

        if (popUpTexts.Count != 0)
            foreach (Text text in popUpTexts)
            {
                text.rectTransform.position += new Vector3(0, 2, 0);
                text.color -= new Color(0, 0, 0, 0.01f);
                if (text.color.a < 0)
                {
                    popUpTexts.Remove(text);
                    Destroy(text.rectTransform.gameObject);
                    break;
                }
            }

        if (hintTimer >= 0)
            hintTimer--;
        else
        {
            if (hintBar.active)
            {
                hintBar.SetActive(false);
                hintTimes--;
            }
        }

        if (hint2Timer >= 0)
            hint2Timer--;
        else
        {
            foreach (GameObject obj in hintPopUp)
            {
                if (obj.active)
                {
                    obj.SetActive(false);
                    hintPopUpVisible = false;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
            ShowPopUpOverlay();
        if (Input.GetKeyDown(KeyCode.Escape))
            ShowOptionsPopUpOverlay();

        
    }

    static public void SpawnText(string text, Vector3 position, Color color)
    {
        Text popUpText = Instantiate(popUpTextPrefab, position, Quaternion.identity);
        popUpText.rectTransform.SetParent(canvas);
        popUpText.text = text;
        popUpText.color = color;

        popUpTexts.Add(popUpText);
    }

    static public void GiveHint()
    {
        if (hintTimes > 0)
        {
            hintBar.SetActive(true);
            hintTimer = 30;
        }
    }

    public void ShowPopUpOverlay()
    {
        inventoryPopUp = !inventoryPopUp;

        if (inventoryPopUp)
            popUpAddButton.GetComponent<Image>().sprite = exit;
        else
            popUpAddButton.GetComponent<Image>().sprite = open;

        if (inventoryPopUp)
            popUp.gameObject.SetActive(true);
        else
            popUp.gameObject.SetActive(false);
    }

    public void ShowOptionsPopUpOverlay()
    {
        optionsPopUpVisible = !optionsPopUpVisible;

        if (optionsPopUpVisible)
            optionsPopUp.gameObject.SetActive(true);
        else
            optionsPopUp.gameObject.SetActive(false);
    }

    public void ShowHintsPopUpOverlay()
    {
        hintPopUpVisible = !hintPopUpVisible;
        if(!inventoryPopUp)
            ShowPopUpOverlay();

        hint2Timer = 600;

        if (hintPopUpVisible)
            foreach(GameObject obj in hintPopUp)
                obj.SetActive(true);
        else
            foreach (GameObject obj in hintPopUp)
                obj.SetActive(false);
    }

    static public void DeathScreen()
    {
        deathScreen.SetActive(true);
    }
}
