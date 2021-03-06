﻿using UnityEngine;
using System.Collections;

public class FloatingRocksManager : MonoBehaviour {

	[HideInInspector]
	public int FloatingRockNumber;
	[HideInInspector]
	public int readyToGoNumber;
	[HideInInspector]
	public bool releaseAll;
	public GameObject FloatingRocksManagerPrefabToClone;
	
	public 		int 			cloneNumber;
	public 		float 			cloningDelay;
	[HideInInspector]
	public		bool			alreadyCloned = false;
	private 	float 			timeSpent;
	private		int				spawnedClones;
	
	// Use this for initialization
	void Start () 
	{
		FloatingRockNumber = transform.childCount;
	}
	
	// Update is called once per frame
	void Update () 
	{
	
		timeSpent += Time.deltaTime;
	
		if(readyToGoNumber == FloatingRockNumber)
		{
			releaseAll = true;
		}
		else if (readyToGoNumber == 0)
		{
			releaseAll = false;
		}
		
		if (!alreadyCloned && timeSpent >= cloningDelay && spawnedClones < cloneNumber)
		{
			GameObject clonedFloatRock = Instantiate (FloatingRocksManagerPrefabToClone) as GameObject;
			clonedFloatRock.GetComponent <FloatingRocksManager>().alreadyCloned = true;
			spawnedClones++;
			timeSpent = 0;
		}	
	}
	
	void IncrementReadyNumber()
	{
		readyToGoNumber++;
	}
	
	void DecrementReadyNumber()
	{
		readyToGoNumber--;
	}
}
