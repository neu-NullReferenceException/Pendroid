using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/*
 Az extra funkci�k:
    - 3Ds begjelen�t�s
    - t�lt�s anim�ci� a kv g�pen
    - a folyad�k a poh�rba t�lt�dik (a perspekt�va miatt nem minden poh�rn�l l�that� de mindegyik ugyan�gy m�k�dik, stabil 60 fps aj�nlott)
    - a folyad�kok k�l�nb�z� megjelen�ssel rendelkeznek
    - particle system vizualiz�lja a folyad�k t�lt�s�t
    - t�lt�s hangeffekt
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
                                        // a t�rfogat ml

    public Animator animator;
    public AudioSource audio;

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
            na ide is k�ne a matematikai m�kol�sb�l ami a fillTime a flowSpeed; outputPipeRadius; �s a t�rfogat eredm�ny�b� k�vetkezik
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
        yield return new WaitForSeconds(fillTime);
        animator.Play("RETURN");
        audio.Play();
        foreach (ParticleSystem ps in psystems)
        {
            ps.Pause();
            ps.Clear();
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
    public float volume; // ml ben �rtend�
    public GameObject prefab;
}

[System.Serializable]
public class Drink
{
    public string name;
    public Material drinkMaterial; // ha k�l�nb�z� sz�n�re akarjuk a fojad�kokat
}
