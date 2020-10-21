using UnityEngine;
using System.Collections;

public class testAnim : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKeyDown ("r"))
		{
			GetComponent<Animation>().Play ("running");
		}
	}
}
