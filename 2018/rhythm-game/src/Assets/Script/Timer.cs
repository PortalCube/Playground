using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {

    private string textStr;

    public int textMode;
    private RhythmManager manager;

    // Use this for initialization
    void Start () {

        manager = RhythmManager.instance;
    }
	
	// Update is called once per frame
	void Update () {


	}
}
