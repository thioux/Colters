using UnityEngine;
using System.Collections;

public class Floater : MonoBehaviour 
{
	public 		float 			waterLevel, floatHeight;
	public 		Vector3 		buoyancyCentreOffset;
	public 		float 			bounceDamp;
	public 		LayerMask 		waterLayer;
	public 		int 			cloneNumber;
	public 		float 			cloningDelay;
	
	private 	Vector3 		flowDirection = Vector3.zero;
	private 	bool 			inWater = false;
	private 	GameObject 		hitWater;
	private 	WaterVolume 	hitWaterScript;
	private 	Vector3 		startPos;
	private 	float 			timeSpent;
	private		int				spawnedClones;
	private		bool			waitingToGo;
	
	void Start ()
	{
		startPos = transform.position;
	}

	void FixedUpdate () 
	{
		if(inWater && !waitingToGo)
		{
			//Let's get the surface's height
			RaycastHit hit;
			if (Physics.Raycast(transform.position + transform.up * 3, -transform.up, out hit, 10, waterLayer))
			{
				waterLevel = hit.point.y;
				
				if (hit.normal != Vector3.up)
				{
				Vector3 up = hit.normal;
				Vector3 left = Vector3.zero;
				
				if (hitWaterScript.streamDirection.z != 0)
					{
						left = Vector3.left;
					}
				else if (hitWaterScript.streamDirection.x != 0)
					{
						left = Vector3.forward;
					}
					
				flowDirection = -Vector3.Cross(up, left); //Uncomplete
				}
				else
				flowDirection = hitWaterScript.streamDirection;
				
				Debug.Log (flowDirection.normalized);
				Debug.DrawRay( hit.point, flowDirection*5 );
			}	
		
			Vector3 actionPoint = transform.position + transform.TransformDirection(buoyancyCentreOffset);
			
			if(flowDirection.y != 0)
				actionPoint -= Vector3.up * -1f;
			
			float forceFactor = 1f - ((actionPoint.y - waterLevel) / floatHeight);
			
			if (forceFactor > 0f) 
			{
				Vector3 uplift = -Physics.gravity * (forceFactor - rigidbody.velocity.y * bounceDamp);
				rigidbody.AddForceAtPosition(uplift, actionPoint);
			}
			
			if(flowDirection != Vector3.zero)
			{
				rigidbody.AddForce(flowDirection.normalized * hitWaterScript.waterStreamSpeed);
				//Making sure speed won't be too fast
				rigidbody.velocity = rigidbody.velocity.normalized * hitWaterScript.waterStreamSpeed;
			}
		}	
	}
	
	void Update ()
	{
		timeSpent += Time.deltaTime;
		
		if (timeSpent >= 3 && spawnedClones < cloneNumber)
		{
			Instantiate(this.gameObject, startPos, Quaternion.identity);
			spawnedClones++;
			timeSpent = 0;
		}
		
		if(!waitingToGo)
		{
			rigidbody.isKinematic = false;
			rigidbody.useGravity = true;
		}
		
		if(transform.parent.GetComponent<FloatingRocksManager>().releaseAll)
		{
			Debug.Log (transform.name +" ready to go!");
			waitingToGo = false;
			transform.parent.GetComponent<FloatingRocksManager>().releaseAll = false;
			transform.parent.GetComponent<FloatingRocksManager>().readyToGoNumber = 0;
		}
	}
	
	void OnTriggerEnter(Collider hit)
	{
		if (hit.CompareTag("Water"))
		{
			inWater = true;
			hitWater = hit.gameObject;
			hitWaterScript = hitWater.GetComponent <WaterVolume>();
		}
		else if (hit.name == "RiverEnd")
		{
			transform.position = startPos;
			transform.rotation = Quaternion.identity;
			rigidbody.isKinematic = true;
			rigidbody.useGravity = false;
			waitingToGo = true;
			transform.parent.SendMessage("IncrementReadyNumber");
		}
	}
	
	void OnTriggerExit(Collider hit)
	{
		if (hit.CompareTag("Water"))
		{
			inWater = false;
		}
	}
}