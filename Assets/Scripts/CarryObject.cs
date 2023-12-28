using UnityEngine;
using System.Collections;

public class CarryObject : MonoBehaviour
{
	public float interactDistance = 3;
	public float carryDistance = 2;
	public LayerMask interactLayer;
	public LayerMask groundLayer;

	private Transform carryObject;
	private bool haveObject;
	[SerializeField] private Vector3 zOffset = new Vector3(0,2.5f,0);

	void Update()
	{
		if (Input.touchCount > 0)
		{
			Debug.Log("TOUCH");
			//Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
			Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
			RaycastHit hit;

			if (Physics.Raycast(ray, out hit, interactDistance, interactLayer))
			{
				if (Input.GetTouch(0).phase == TouchPhase.Began && hit.transform.TryGetComponent<Rigidbody>(out Rigidbody rb))
				{
					carryObject = hit.transform;
					rb.useGravity = false;
					rb.velocity = Vector3.zero;
					rb.isKinematic = true;
					haveObject = true;
				}
			}

			if (Input.GetTouch(0).phase == TouchPhase.Ended)
			{
				if (haveObject)
				{
					haveObject = false;
					carryObject.GetComponent<Rigidbody>().useGravity = true;
					carryObject.GetComponent<Rigidbody>().isKinematic = false;
					carryObject = null;
				}
			}

			if (haveObject)
			{
				if (Physics.Raycast(ray, out hit, 100, groundLayer))
				{
					carryObject.position = Vector3.Lerp(carryObject.position, hit.point + zOffset, Time.deltaTime * 8);
				}
			}
		}
		
	}
}
