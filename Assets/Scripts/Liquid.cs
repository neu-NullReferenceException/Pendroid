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

    private void SetLevel(float y)  // az�rt z mer m�g mind�g szar a blender -> unity axis conversion
    {
        liquidTransform.localScale = new Vector3(1, y, 1);
    }

    public void StartFill(float t, float waitBeforeFill)
    {
        Empty();
        StartCoroutine(Fill(t, waitBeforeFill));
    }

    public IEnumerator Fill(float t, float waitBeforeFill)// ha nincs kedv�nk matekozni ez kicser�lhet� egy 1 mp hossz� anim�ci�ra amin a playback speedet manipul�ljuk
    {
        yield return new WaitForSeconds(waitBeforeFill);
        float incremental = 1 / (t*60); // 60 fps re van lockolva az app

        while(1-fillState > incremental)// am�g elfogadhat� m�rt�ken bel�lre nem �r a fojad�k
        {
            fillState += incremental;
            SetLevel(fillState);
            yield return null;
        }

        Fill();
    }
}
