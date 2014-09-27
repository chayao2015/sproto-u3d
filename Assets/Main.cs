﻿using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class Main : MonoBehaviour {
	private Client mClient = null;

	void Start () {
		new Test ().Run ();
		new TestAll ().Run ();
		//new TestRpc ().Run ();
	}

	void OnGUI () {
		if (GUI.Button (new Rect (10, 10, 100, 100), "Benchmark")){
			new Benchmark().Run ();
		}

		if (mClient != null)
			GUI.enabled = false;

		if (GUI.Button (new Rect (150, 10, 100, 100), "Client")){
			mClient = new Client();
			mClient.Run ();
		}

		GUI.enabled = true;
	}
}
