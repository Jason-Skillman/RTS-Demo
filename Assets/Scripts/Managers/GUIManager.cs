using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIManager : MonoBehaviour {

	//Singleton
	public static GUIManager main;


	public void Start() {
        if(main == null) main = this;
	}

	public int height = 180;
	public int width = 875;
	public void OnGUI() {
		GUI.Box(new Rect((Screen.width - width)/2, Screen.height - height, width, height), "");

		if(GUI.Button(new Rect(0, 0, 50, 50), "Click me")) {
			Debug.Log("You clicked me");
		}
	}

}
