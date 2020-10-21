using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;

[RequireComponent(typeof(Seeker))]
public class Path_Movement : MonoBehaviour {

	public Vector3 target;
	public float speed = 5;
	public bool isTraveling;

	public float nextWaypointDistance = 1;  // The max distance from the AI to a waypoint for it to continue to the next waypoint	//MyNote: Lower numbers(1) is more accurat to the path, higher numbers(3) is more smother of a path
	private int currentWaypoint = 0;    // The waypoint we are currently moving towards
	private Pathfinding.Path path;

	//Componets
	private Seeker seeker;
	private CharacterController controller;
	
	// How often to recalculate the path (in seconds)
	public float pathSearchInterval = 0.5f;
	private float timeToWait;


	public void Start() {
		seeker = GetComponent<Seeker>();
		controller = GetComponent<CharacterController>();

		isTraveling = false;
		timeToWait = Time.time;
	}
	
	public void Update() {
		if(isTraveling) {
			//Update path
			if(Time.time >= timeToWait) {
				timeToWait = Time.time + pathSearchInterval;
				seeker.StartPath(this.transform.position, target, OnPathComplete);
				Debug.Log("Update Path");
			}

			//Checks if we have a path yet
			if(path == null) {
				//Debug.Log("No Path yet");
				return;
			}
			

			//Are we at the end of the path?
			if(currentWaypoint >= path.vectorPath.Count) {
				isTraveling = false;
				currentWaypoint++;
				OnTargetReached();
				//return;
			}
			//Else move to the next waypoint
			else {
				// Direction to the next waypoint
				Vector3 dir = (path.vectorPath[currentWaypoint]-transform.position).normalized;
				dir *= speed;

				//Move the controller
				controller.SimpleMove(dir);

				// The commented line is equivalent to the one below, but the one that is used is slightly faster since it does not have to calculate a square root
				//if (Vector3.Distance (transform.position,path.vectorPath[currentWaypoint]) < nextWaypointDistance) {
				if((transform.position-path.vectorPath[currentWaypoint]).sqrMagnitude < nextWaypointDistance*nextWaypointDistance) {
					currentWaypoint++;
					return;
				}
			}
		}

	}


	//Starts the pathfinding
	public void TravelToPath(Vector3 pos) {
		isTraveling = true;
		target = pos;
		timeToWait = Time.time;
		currentWaypoint = 0;
	}

	//When it is done calculating were it needs to be
	public void OnPathComplete(Pathfinding.Path p) {
		//Debug.Log("A path was calculated. Did it fail with an error? " + p.error);
		if(!p.error) {
			path = p;
			// Reset the waypoint counter so that we start to move towards the first point in the path
			currentWaypoint = 0;
		}
	}

	//When the object has reached it's target
	public void OnTargetReached() {
		Debug.Log("End Of Path Reached");
	}

}
