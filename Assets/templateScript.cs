using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;

public class templateScript : MonoBehaviour 
{
	public KMBombInfo bomb;
	public KMAudio audio;

	//public KMSelectable btn1;

	//Logging
	static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

	void Awake()
	{
		moduleId = moduleIdCounter++;
		//btn.OnInteract += delegate () { PressButton(); return false; };
	}

	void Start () 
	{
		
	}
	
	void Update () 
	{
		
	}
}
