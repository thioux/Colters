﻿using UnityEngine;
using System.Collections;

public class TrapV4 : MonoBehaviour {


	public AnimationClip anim;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {


	}


	void OnTriggerEnter(Collider c)
	{
		if (c.gameObject.tag == "ThrowableRock")
		{
			this.gameObject.animation.Play("StatueMain1");
			Debug.Log("Play!");
		}

}
}
