using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/*
 Az extra funkciók:
    - 3Ds begjelenítés
    - töltés animáció a kv gépen
    - a folyadék a pohárba töltõdik (a perspektíva miatt nem minden pohárnál látható de mindegyik ugyanúgy mûködik, stabil 60 fps ajánlott)
    - a folyadékok különbözõ megjelenéssel rendelkeznek
    - particle system vizualizálja a folyadék töltését
    - töltés hangeffekt
 */

public class DrinkMachine : MonoBehaviour
{
    public float fillTime = 3;
    public float outputPipeDiameter;
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
    public TextMeshProUGUI pipeDiameterText;
    public TextMeshProUGUI fillSpeedText;
    public TextMeshProUGUI fillTimeText;
    public TextMeshProUGUI timerText;

    public ParticleSystem[] psystems;

    public Slider fillSpeedSlider;      // cm/sec
    public Slider fillTimeSlider;       // sec
    public Slider pipeDiameterSlider;   //mm
                                        // a térfogat ml

    public Animator animator;
    public GameObject audio; // azért mer így nem kell a looptime al szívni csak bekapcs kikapcs

    private bool isCounting;
    private float t;

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

    public void SetPipeRadius()
    {
        outputPipeDiameter = pipeDiameterSlider.value;
        pipeDiameterText.text = pipeDiameterSlider.value + "";
        CalculateFillParameters();
    }
    public void SetFlowSpeed()
    {
        flowSpeed = fillSpeedSlider.value;
        fillSpeedText.text = fillSpeedSlider.value + "";
        CalculateFillParameters();
    }
    public void SetFillTime()
    {
        fillTime = fillTimeSlider.value;
        if(fillTime.ToString().Length > 4)
        {
            fillTimeText.text = fillTime.ToString().Substring(0, 4);
        }
        else
        {
            fillTimeText.text = fillTime.ToString();
        }
        
        CalculateFillParametersWithGivenTime();
    }

    public void SetPipeRadiusSlider()
    {
        pipeDiameterSlider.value = outputPipeDiameter ;
        pipeDiameterText.text = pipeDiameterSlider.value + "";
    }
    public void SetFlowSpeedSlider()
    {
        fillSpeedSlider.value = flowSpeed;
        pipeDiameterText.text = fillSpeedSlider.value + "";
    }
    public void SetFillTimeSlider()
    {
        fillTimeSlider.value = fillTime;
        pipeDiameterText.text = fillTime + "";
    }

    public void CalculateFillParameters()
    {
        //Flowrate = v * (pi*r2)
        float volume = cups[currentCupID].volume;
        fillTime = volume / (flowSpeed * Mathf.PI * Mathf.Pow(outputPipeDiameter / 2, 2));
        fillTimeText.text = fillTime.ToString().Substring(0,4);
        fillTimeSlider.value  = fillTime;
    }

    public void CalculateFillParametersWithGivenTime()
    {
        //Flowrate = v * (pi*r2)
        float volume = cups[currentCupID].volume;
        fillSpeedSlider.value = Mathf.Round(volume / fillTime);
        fillSpeedText.text = fillSpeedSlider.value + "";
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

        currentCup.GetComponent<Liquid>().liquidTransform.GetChild(0).GetComponent<MeshRenderer>().material = drinks[currentDrinkID].drinkMaterial;
        currentCup.GetComponent<Liquid>().StartFill(fillTime, 3);
        StartCoroutine(UseDrinkParticle(3, fillTime));
        
    }

    public IEnumerator UseDrinkParticle(float waitBeforeFill, float fillTime)
    {
        animator.Play("PREPARE");
        yield return new WaitForSeconds(waitBeforeFill);
        foreach (ParticleSystem ps in psystems)
        {
            ps.Play();
        }
        audio.SetActive(true);
        StartCounting();
        yield return new WaitForSeconds(fillTime);
        animator.Play("RETURN");
        audio.SetActive(false);
        StopCounting();
        foreach (ParticleSystem ps in psystems)
        {
            ps.Pause();
            ps.Clear();
        }
    }

    private void StartCounting()
    {
        isCounting = true;
        t = 0;
    }

    private void StopCounting()
    {
        isCounting = false;
    }

    private void Update()
    {
        if (isCounting)
        {
            t += Time.deltaTime;
            timerText.text = Mathf.Floor(t / 60) + ":" + Mathf.Floor(t % 60);
        }
    }

    private void Awake()
    {
        Application.targetFrameRate = 60;
        fillSpeedText.text = fillTimeSlider.value + "";
        flowSpeed = fillTimeSlider.value;
        pipeDiameterText.text = pipeDiameterSlider.value + "";
        outputPipeDiameter = pipeDiameterSlider.value;

        CalculateFillParameters();
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
