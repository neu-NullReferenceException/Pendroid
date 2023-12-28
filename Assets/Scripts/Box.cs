using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Box : MonoBehaviour
{
    public EntityMaterialType type;
    public GameManager manager;
    public int myScore = 0;
    public TextMeshProUGUI myScoreText;

    private void Awake()
    {
        Debug.Log("box online");
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<MyItem>(out MyItem item))
        {
            if(item.item.type == type)
            {
                manager.AddScoreNCash(item.item.scoreValue, item.item.moneyValue);
                myScore++;
                myScoreText.text = myScore + "";
            }
            else
            {
                manager.AddScoreNCash(-item.item.scoreValue, 0);
            }

            Destroy(other.gameObject);
        }
    }
}
