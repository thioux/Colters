﻿using UnityEngine;
using System.Collections;

/*This script can check what composes the point the player is looking at on a terrain
 * Under certain conditions, it can spawn a rock
 */

public class RockSpawn : MonoBehaviour {

	//Set up & inspector vars
	int surfaceIndex = 0;
	public int rockSpawnableSurface = 0;
	public int maxSpawnableRocks = 3;
	public Vector3 spawnOffset = new Vector3(0,.5f,0);
	public Material[] spawnableMaterials;

	//External scripts and objects
	GameObject spawnedRock;
	Camera mainCamera;
	public GameObject spawnableRock;

	//Terrain vars
	Terrain terrain;
	TerrainData terrainData;
	Vector3 terrainPos;

	//Misc. Vars
	int rockNumber = 0;
	int rockNumberToDestroy = 0;
	int spawnedRockAmount = 0;
	bool coolingDown = false;
	bool aimingAtTerrain;
	
void Start()
{
	mainCamera = Camera.main;
}
	
	
void Update()
{
	if(!coolingDown) //If the spawn power isn't cooling down.
	{
		Vector3 p = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width/2, Screen.height/2, mainCamera.nearClipPlane));

		if(Input.GetButtonDown ("RockSpawn"))
		{
			StartCoroutine(coolDown()); //Let's start the cooldown right now.

			//Those two lines below set the rayCast coordinates used further in the script.
			RaycastHit hit = new RaycastHit();
			Ray ray = new Ray(p, mainCamera.transform.forward);

			//Let's check if what we hit was a terrain
			if(Physics.Raycast(ray, out hit, Mathf.Infinity))
			{
				if(hit.collider.GetComponent<Terrain>()!= null)
				{ //If it's really a terrain, let's gather all the datas we need for later
					aimingAtTerrain = true;
					terrain = hit.collider.GetComponent<Terrain>();
					terrainData = terrain.terrainData;
					terrainPos = terrain.transform.position;
				}
				else
					aimingAtTerrain = false;
			}

#region Terrain Check
			if(aimingAtTerrain)
			{
				//Let's get the hit point's dominant texture.
				if (terrain.collider.Raycast(ray, out hit, Mathf.Infinity)) 
					surfaceIndex = GetMainTexture(hit.point );

				//If the dominant texture's is the same as the rock spawnable surface...
				if(surfaceIndex == rockSpawnableSurface)
				{
					//Let's set the rock spawn point's coordinates right now.
					//float terrainHeight = Terrain.activeTerrain.SampleHeight(transform.position);
					
					if (terrain.collider.Raycast(ray, out hit, Mathf.Infinity)) 
					{ //Now, let's get the real amount of the slope we hit, so we can rotate the rock properly.
						spawning (hit);
					}
				}
			}
#endregion

#region GameObject check
		if(!aimingAtTerrain)
		{
			//Let's check if we can spawn on this material
			if(Physics.Raycast(ray, out hit, Mathf.Infinity))
			{
				Renderer hitRenderer = hit.collider.GetComponent<Renderer>();
				bool materialIsCompatible = false;
				
				foreach (Material material in spawnableMaterials)
				{
							if(hitRenderer.material.mainTexture != null && hitRenderer.material != null)
					{
						if (material.mainTexture.name == hitRenderer.material.mainTexture.name)
							materialIsCompatible = true;
							Debug.Log ("Material was "+hitRenderer.material.mainTexture.name);
					}
						else
							Debug.Log ("No material or texture attached to aimed object");
				}
			
				if(materialIsCompatible)
				{
					spawning (hit);
				}
			}		
		}
#endregion
		}
	}
}

void spawning(RaycastHit hit)
	{
		Vector3 rockSpawnPoint = hit.point;
		Vector3 slope = hit.normal;
		rockSpawnPoint -=spawnOffset;
		spawnedRock = Instantiate(spawnableRock, rockSpawnPoint , Quaternion.identity) as GameObject;
		Debug.Log(spawnedRock.transform.rotation.eulerAngles);
		spawnedRock.transform.rotation = Quaternion.FromToRotation(spawnedRock.transform.up, slope) * spawnedRock.transform.rotation;
		Debug.Log(spawnedRock.transform.rotation.eulerAngles);
		spawnedRock.gameObject.name = "spawnedRock_"+rockNumber;
		spawnedRockAmount++;
		
		if(spawnedRockAmount>maxSpawnableRocks) //If there's more than the maximum amount of rock, let's destroy the first we spawned.
		{
			GameObject firstSpawnedRock = GameObject.Find ("spawnedRock_"+rockNumberToDestroy);
			Destroy (firstSpawnedRock.gameObject);
			rockNumberToDestroy++;
			spawnedRockAmount--;
		}
		rockNumber++;
	}
	
float[] GetTextureMix( Vector3 worldPos )
{
	// Return an array which will contain datas from the textures mix
	// of the main terrain, based on world coordinates.
	
	// The amount of returned values will be
	// equal to the number of textures attached to the terrain.

	//Let's calculate which point of the terrain corresponds to the given world coordinates
	float mapXf = ((worldPos.x - terrainPos.x) / terrainData.size.x) * terrainData.alphamapWidth ;
	float mapZf = ((worldPos.z - terrainPos.z) / terrainData.size.z) * terrainData.alphamapHeight ;
	int mapX = (int)mapXf;
	int mapZ = (int)mapZf;

	//Then we get the splat/alpha of this point
	float[,,] splatmapData = terrainData.GetAlphamaps( mapX, mapZ, 1, 1 );
	
	//Let's put it in a unidimensional array.
	float[] cellMix = new float[ splatmapData.GetUpperBound(2) + 1 ];
	
	for ( int n = 0; n < cellMix.Length; n ++ )
	{
		cellMix[n] = splatmapData[ 0, 0, n ];
	}
	
	return cellMix;
}
	
	
int GetMainTexture( Vector3 worldPos )
{
	// Let's get the index of the detected texture
	float[] mix = GetTextureMix( worldPos );
	
	float maxMix = 0;
	int maxIndex = 0;
	
	//Let's look at each texture and find the dominant.
	for ( int n = 0; n < mix.Length; n ++ )
	{
		if ( mix[n] > maxMix )
		{
			maxIndex = n;
			maxMix = mix[n];
		}
	}
	
	return maxIndex;
}

IEnumerator coolDown(){
	coolingDown = true;
	yield return new WaitForSeconds(.5f);
	coolingDown = false;
}
	

}
// END OF SCRIPT
