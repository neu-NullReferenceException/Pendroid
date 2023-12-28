using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public GameObject[] instantiateObjects;
    public Transform spawnPosition;
    public float waitBeforeInstantiate;
    public bool isProduceing = true;
    private bool isWaiting = false;

    void Update()
    {
        if(isProduceing)
        {
            if(!isWaiting)
            {
                StartCoroutine("SpawnItem");
                isWaiting = true;
            }
        }
    }

    public IEnumerator SpawnItem()
    {
        yield return new WaitForSeconds(waitBeforeInstantiate);
        Instantiate(instantiateObjects[Random.Range(0,instantiateObjects.Length)], spawnPosition.position, spawnPosition.rotation);
        isWaiting = false;
    }
}
