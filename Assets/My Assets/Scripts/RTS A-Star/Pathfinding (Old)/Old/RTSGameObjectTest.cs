using UnityEngine;
using System;
using System.Collections;

public class RTSGameObjectTest : MonoBehaviour {

	public void Update() {
		if(Input.GetMouseButtonDown(0)) {
			Debug.Log("Click");
			//this.GetComponent<Pathfinder>().TravelToPath(OnEndOfPath);	//Uncomment me and fix
		}
	}

	public void OnEndOfPath() {

	}

}
