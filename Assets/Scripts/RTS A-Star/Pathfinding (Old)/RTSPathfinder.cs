using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using RTSEngine;

[RequireComponent(typeof(Seeker))]
[RequireComponent(typeof(CharacterController))]
public class RTSPathfinder : MonoBehaviour {

	public Vector3 target;
	public float speed = 5.0f;
	public float turningSpeed = 5.0f;
	public bool isTraveling = false;
	//public float pathSearchInterval = 0.5f;	//Time to wait interval until it updates it's target path position
	public float nextWaypointDistance = 1;      //The max distance from the AI to a waypoint for it to continue to the next waypoint	//MyNote: Lower numbers(1) is more accurat to the path, higher numbers(3) is more smother of a path

	private Pathfinding.Path path;
	//private float timeToWait;
	private int currentWaypoint = 0;			// The waypoint we are currently moving towards
	private int currentWaypointIndex = 0;
	private bool flagOnStart = true;

	//Componets
	private Seeker seeker;
	private CharacterController controller;
	private RTSGameObject rtsGameObject;

	
	public void Start() {
		seeker = GetComponent<Seeker>();
		controller = GetComponent<CharacterController>();
		rtsGameObject = GetComponent<RTSGameObject>();
		if(rtsGameObject == null) Debug.LogWarning("No RTSGameObject was found.");

		//timeToWait = Time.time;
	}

	public Vector3 direction = new Vector3();

	public void Update() {
		if(isTraveling) {
			/*
			//Update path (NOT NEEDED)
			if(Time.time >= timeToWait) {
				timeToWait = Time.time + pathSearchInterval;
				seeker.StartPath(this.transform.position, target, OnPathCalculationComplete);
			}
			*/

			//Checks if we have a path yet
			if(path != null) {
				if(currentWaypoint < path.vectorPath.Count) {
					//Direction and speed to the next waypoint
					direction = (path.vectorPath[currentWaypoint]-transform.position).normalized;
					direction *= speed;

					//Rotate towards the next path
					RotateTowards(direction);

					//Move the object
					controller.SimpleMove(direction);


					if((transform.position-path.vectorPath[currentWaypoint]).sqrMagnitude < nextWaypointDistance*nextWaypointDistance) {
						currentWaypoint++;
					}
				} else {
					//End of the path
					isTraveling = false;
					currentWaypoint++;
					OnTargetReached();
				}
				rtsGameObject.OnPathTravel();
			}
		}
	}

	//Activate Pathfinding
	public void TravelToPath(Vector3 pos) {
		isTraveling = true;
		target = pos;
		//timeToWait = Time.time;
		currentWaypoint = 0;

		seeker.StartPath(this.transform.position, target, OnPathCalculationComplete);
	}

	private void RotateTowards(Vector3 dir) {
		if(dir != Vector3.zero) {
			Debug.Log("Rotate");
			Quaternion rot = transform.rotation;
			Quaternion toTarget = Quaternion.LookRotation(dir);

			rot = Quaternion.Slerp(rot, toTarget, turningSpeed*Time.deltaTime);
			Vector3 euler = rot.eulerAngles;
			euler.z = 0;
			euler.x = 0;
			rot = Quaternion.Euler(euler);

			transform.rotation = rot;
		} else {
			Debug.Log("U????");
		}
	}


	//Called once when the path to the current target position has been calculated.
	public void OnPathCalculationComplete(Pathfinding.Path path) {
		//OnPathTravelStart() flag
		if(flagOnStart) {
			flagOnStart = false;
			rtsGameObject.OnPathTravelStart();
		}

		//Check for path calculation error
		if(!path.error) {
			//Reset variables
			this.path = path;
			currentWaypoint = 0;

			rtsGameObject.OnPathTravelUpdate();
		} else {
			Debug.LogError("Path calculation error: " + path.error);
		}
	}

	//Called once when the object has reached it's target position.
	public void OnTargetReached() {
		flagOnStart = true;
		rtsGameObject.OnPathTravelEnd();
	}

}
