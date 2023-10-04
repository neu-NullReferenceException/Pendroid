using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Liquid : MonoBehaviour
{
    [SerializeField] private float fillState = 0;
    public Transform liquidTransform;

    public void Empty()
    {
        fillState = 0;
        SetLevel(fillState);
    }
    public void Fill()
    {
        fillState = 1;
        SetLevel(fillState);
    }

    private void SetLevel(float y)  // azért z mer még mindíg szar a blender -> unity axis conversion
    {
        liquidTransform.localScale = new Vector3(1, y, 1);
    }

    public void StartFill(float t, float waitBeforeFill)
    {
        Empty();
        StartCoroutine(Fill(t, waitBeforeFill));
    }

    public IEnumerator Fill(float t, float waitBeforeFill)// ha nincs kedvünk matekozni ez kicserélhetõ egy 1 mp hosszú animációra amin a playback speedet manipuláljuk
    {
        yield return new WaitForSeconds(waitBeforeFill);
        float incremental = 1 / (t*60); // 60 fps re van lockolva az app

        while(1-fillState > incremental)// amíg elfogadható mértéken belülre nem ér a fojadék
        {
            fillState += incremental;
            SetLevel(fillState);
            yield return null;
        }

        Fill();
    }
}
