using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackAnimator : MonoBehaviour
{
    private Renderer renderer;
    private MaterialPropertyBlock propBlock;
    //private Vector3 prevPos;
    //public Transform referenceObject;
    private float offsetY;
    public Conveyor conveyor;
    public float ownSpeedUVCorrection = 1f;

    private void Start()
    {
        renderer = GetComponent<Renderer>();
        propBlock = new MaterialPropertyBlock();
        //prevPos = referenceObject.position;
    }

    private void Update()
    {
        //Vector3 offset = referenceObject.position - prevPos;
        //float scalar = ScalarMultiplication(offset.normalized, referenceObject.forward.normalized);
        //float velocity = Vector3.Distance(prevPos, referenceObject.position);

        //prevPos = referenceObject.position;

        /*if(scalar > 0)
        {
            offsetY += velocity;
        }
        else
        {

            offsetY -= velocity;
        }*/
        offsetY += Time.deltaTime * conveyor.speed * ownSpeedUVCorrection;

        renderer.GetPropertyBlock(propBlock);
        propBlock.SetFloat("_OffsetY", offsetY);
        renderer.SetPropertyBlock(propBlock);
    }

    private float ScalarMultiplication(Vector3 a, Vector3 b)
    {
        return (a.x * b.x) + (a.y * b.y) + (a.z * b.z);
    }
}
