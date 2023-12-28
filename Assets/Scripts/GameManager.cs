using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Conveyor[] conveyors;
    //public TrackAnimator[] trackAnimators;
    public ItemSpawner itemSpawner;

    public int score;
    public int money;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI moneyText;
    public Animator anim;
    public GameObject pauseCanvas;
    public GameObject alertObject;
    public TextMeshProUGUI alertText;

    public float gameSpeed;
    private float ogGameSpeed;
    public bool isPaused;
    public bool isSlown;
    public int slowCost = 500;
    private float t;
    private float nextLevel = 30;


    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        ogGameSpeed = gameSpeed;
        itemSpawner.isProduceing = false;
        //Begin();
        //anim.Play("PrevStart");
    }

    public void Begin()
    {
        itemSpawner.isProduceing = true;
        anim.SetTrigger("Start");
    }

    private void Update()
    {
        if (!isPaused && !isSlown)
        {
            if(t < nextLevel)
            {
                t += Time.deltaTime;
            }
            else
            {
                t = 0;
                gameSpeed *= 1.1f;
                SetConveyorSpeed(gameSpeed);
            }
        }
    }

    public void TriggerSlow()
    {
        if(money > 500 && !isSlown)
        {
            AddScoreNCash(0, -slowCost);
            StartCoroutine(SlowDown());
        }
        else if(money < 500)
        {
            StartCoroutine(ShowAlert("Nincs elég pénzed!", 3));
        }
        else if (isSlown)
        {
            StartCoroutine(ShowAlert("Lassítás már aktív!", 3));
        }
    }

    public IEnumerator ShowAlert(string alertText, float time)
    {
        alertObject.SetActive(true);
        this.alertText.text = alertText;
        yield return new WaitForSeconds(time);
        alertObject.SetActive(false);
    }

    public IEnumerator SlowDown()
    {
        isSlown = true;
        SetConveyorSpeed(ogGameSpeed);
        yield return new WaitForSeconds(10);
        SetConveyorSpeed(gameSpeed);
        isSlown = false;
    }

    public void Pause()
    {
        isPaused = true;
        itemSpawner.isProduceing = false;
        SetConveyorSpeed(0);
        pauseCanvas.SetActive(true);
    }
    public void UnPause()
    {
        isPaused = false;
        itemSpawner.isProduceing = true;
        SetConveyorSpeed(gameSpeed);
        pauseCanvas.SetActive(false);
    }


    public void SetConveyorSpeed(float speed)
    {
        foreach (Conveyor item in conveyors)
        {
            item.speed = speed;
        }
        
    }

    public void AddScoreNCash(int score, int cash)
    {
        this.score += score;
        this.money += cash;
        RefreshUI();
    }

    public void RefreshUI()
    {
        scoreText.text = "" + score;
        moneyText.text = "" + money;
    }

    public void Lose()
    {
        isPaused = true;
        SetConveyorSpeed(0);
        anim.Play("Loose");
    }

    public void Exit()
    {
        Application.Quit();
    }
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
