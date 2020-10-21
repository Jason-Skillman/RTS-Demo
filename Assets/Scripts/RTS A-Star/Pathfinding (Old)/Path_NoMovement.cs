using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;

[RequireComponent(typeof(Seeker))]
public class Path_NoMovement : MonoBehaviour {

	public Vector3 target;
	public float pathSearchInterval = 0.5f;

	private Seeker seeker;
	private bool isTraveling;
	private float timeToWait;


	public void Start() {
		seeker = this.GetComponent<Seeker>();

		isTraveling = false;
		timeToWait = Time.time;
	}

	public void Update() {
		if(isTraveling) {
			if(Time.time >= timeToWait) {
				timeToWait = Time.time + pathSearchInterval;
				seeker.StartPath(this.transform.position, target, OnPathComplete);
				Debug.Log("Update Path");
			}
		}
	}

	//Starts the pathfinding
	public void TravelToPath(Vector3 pos) {
		isTraveling = true;
		target = pos;
		timeToWait = Time.time;
	}

	//When it is done calculating were it needs to be
	public void OnPathComplete(Pathfinding.Path p) {
		//Debug.Log("Yay, we got a path back. Did it have an error? "+p.error);
	}

}
