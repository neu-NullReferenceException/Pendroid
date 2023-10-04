using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DrinkMachine : MonoBehaviour
{
    public float fillTime = 3;
    public float outputPipeRadius;
    public float flowSpeed;

    public Cup[] cups;
    public Drink[] drinks;

    public int currentDrinkID;
    public int currentCupID;

    [HideInInspector]
    public GameObject currentCup;
    public Transform cupSpawnPoint;

    [Space(5)]
    [Header("References")]
    public TextMeshProUGUI drinkNameText;
    public TextMeshProUGUI volumeText;
    public TextMeshProUGUI pipeRadiusText;
    public TextMeshProUGUI fillSpeedText;
    public TextMeshProUGUI fillTimeText;
    public TextMeshProUGUI timerText;

    public void RefreshDrinkData()
    {
        drinkNameText.text = drinks[currentDrinkID].name;
    }
    public void NextDrink()
    {
        if(currentDrinkID < drinks.Length - 1)
        {
            currentDrinkID++;
        }
        else
        {
            currentDrinkID = 0;
        }
        RefreshDrinkData();
    }
    public void PrevDrink()
    {
        if (currentDrinkID > 0)
        {
            currentDrinkID--;
        }
        else
        {
            currentDrinkID = drinks.Length-1;
        }
        RefreshDrinkData();
    }

    public void NextCup()
    {
        cups[currentCupID].icon.SetActive(false);
        if (currentCupID < cups.Length - 1)
        {
            currentCupID++;
        }
        else
        {
            currentCupID = 0;
        }
        cups[currentCupID].icon.SetActive(true);
        volumeText.text = cups[currentCupID].volume + " ml";
    }
    public void PrevCup()
    {
        cups[currentCupID].icon.SetActive(false);
        if (currentCupID > 0)
        {
            currentCupID--;
        }
        else
        {
            currentCupID = cups.Length-1;
        }
        cups[currentCupID].icon.SetActive(true);
        volumeText.text = cups[currentCupID].volume + " ml";
    }

    public void SetPipeRadius(int r)
    {
        outputPipeRadius = r;
        pipeRadiusText.text = r + "";
    }
    public void SetFlowSpeed(int mlPerSec)
    {
        flowSpeed = mlPerSec;
        pipeRadiusText.text = mlPerSec + "";
    }
    public void SetFillTime(float t)
    {
        fillTime = t;
        pipeRadiusText.text = fillTime + "";
    }

    public void ServeDrink()
    {
        if (currentCup)
        {
            Destroy(currentCup);
        }
        currentCup = Instantiate(cups[currentCupID].prefab, cupSpawnPoint.position, cupSpawnPoint.rotation);

        /*
            na ide is kéne a matematikai mókolásból ami a fillTime a flowSpeed; outputPipeRadius; és a térfogat eredményébõ következik
         */

        currentCup.GetComponent<Liquid>().StartFill(fillTime, 3);
        currentCup.GetComponent<Liquid>().liquidTransform.GetChild(0).GetComponent<MeshRenderer>().material = drinks[currentDrinkID].drinkMaterial;
    }

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }
}

[System.Serializable]
public class Cup
{
    public string name;
    public GameObject icon;
    public float volume; // ml ben értendõ
    public GameObject prefab;
}

[System.Serializable]
public class Drink
{
    public string name;
    public Material drinkMaterial; // ha különbözõ színûre akarjuk a fojadékokat
}
