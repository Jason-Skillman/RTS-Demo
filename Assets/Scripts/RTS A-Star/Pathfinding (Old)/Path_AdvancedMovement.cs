using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;

[RequireComponent(typeof(Seeker))]
public class Path_AdvancedMovement : MonoBehaviour {

	public Vector3 target;
	public float speed = 5;
	public bool isTraveling;

	public float nextWaypointDistance = 1;  // The max distance from the AI to a waypoint for it to continue to the next waypoint	//MyNote: Lower numbers(1) is more accurat to the path, higher numbers(3) is more smother of a path
	private int currentWaypoint = 0;    // The waypoint we are currently moving towards
	private Pathfinding.Path path;
	private int currentWaypointIndex = 0;

	//Componets
	private Seeker seeker;
	private CharacterController controller;
	private Rigidbody rigBody;

	// How often to recalculate the path (in seconds)
	public float pathSearchInterval = 0.5f;
	private float timeToWait;

	//OTHER
	protected Vector3 targetPoint;
	protected Vector3 targetDirection;
	public float turningSpeed = 5.0f;
	public float slowdownDistance = 0.6f;
	protected float minMoveScale = 0.05f;
	public float endReachedDistance = 0.2f;
	public float pickNextWaypointDist = 2;
	protected Vector3 lastFoundWaypointPosition;
	protected float lastFoundWaypointTime = -9999;
	protected bool targetReached = false;
	public float forwardLook = 1;

	
	public void Start() {
		seeker = GetComponent<Seeker>();
		controller = GetComponent<CharacterController>();
		rigBody = GetComponent<Rigidbody>();

		isTraveling = false;
		timeToWait = Time.time;
	}

	public virtual void Update() {
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
				// Direction to the next waypoint, and speed
				Vector3 direction = (path.vectorPath[currentWaypoint]-transform.position).normalized;
				direction *= speed;

				//Vector3 direction = CalculateVelocity(GetFeetPosition());


				//Move the controller
				//RotateTowards(targetDirection);
				RotateTowards(direction);

				//Move
				controller.SimpleMove(direction);
				//rigBody.AddForce(direction);


				if((transform.position-path.vectorPath[currentWaypoint]).sqrMagnitude < nextWaypointDistance*nextWaypointDistance) {
					currentWaypoint++;
					return;
				}
			}
		}
		/*
		if(controller != null) {
			controller.SimpleMove(dir);
		} else if(rigid != null) {
			rigid.AddForce(dir);
		} else {
			tr.Translate(dir*Time.deltaTime, Space.World);
		}
		*/
	}

	private void RotateTowards(Vector3 dir) {
		if(dir == Vector3.zero) return;

		Quaternion rot = transform.rotation;
		Quaternion toTarget = Quaternion.LookRotation(dir);

		rot = Quaternion.Slerp(rot, toTarget, turningSpeed*Time.deltaTime);
		Vector3 euler = rot.eulerAngles;
		euler.z = 0;
		euler.x = 0;
		rot = Quaternion.Euler(euler);

		transform.rotation = rot;
	}

	private Vector3 CalculateVelocity(Vector3 currentPosition) {
		if(path == null || path.vectorPath == null || path.vectorPath.Count == 0) return Vector3.zero;

		List<Vector3> vPath = path.vectorPath;

		if(vPath.Count == 1) {
			vPath.Insert(0, currentPosition);
		}

		if(currentWaypointIndex >= vPath.Count) { currentWaypointIndex = vPath.Count-1; }

		if(currentWaypointIndex <= 1) currentWaypointIndex = 1;

		while(true) {
			if(currentWaypointIndex < vPath.Count-1) {
				//There is a "next path segment"
				float dist = XZSqrMagnitude(vPath[currentWaypointIndex], currentPosition);
				//Mathfx.DistancePointSegmentStrict (vPath[currentWaypointIndex+1],vPath[currentWaypointIndex+2],currentPosition);
				if(dist < pickNextWaypointDist*pickNextWaypointDist) {
					lastFoundWaypointPosition = currentPosition;
					lastFoundWaypointTime = Time.time;
					currentWaypointIndex++;
				} else {
					break;
				}
			} else {
				break;
			}
		}

		Vector3 dir = vPath[currentWaypointIndex] - vPath[currentWaypointIndex-1];
		Vector3 targetPosition = CalculateTargetPoint(currentPosition, vPath[currentWaypointIndex-1], vPath[currentWaypointIndex]);


		dir = targetPosition-currentPosition;
		dir.y = 0;
		float targetDist = dir.magnitude;

		float slowdown = Mathf.Clamp01(targetDist / slowdownDistance);

		this.targetDirection = dir;
		this.targetPoint = targetPosition;

		if(currentWaypointIndex == vPath.Count-1 && targetDist <= endReachedDistance) {
			if(!targetReached) { targetReached = true; OnTargetReached(); }

			//Send a move request, this ensures gravity is applied
			return Vector3.zero;
		}

		Vector3 forward = transform.forward;
		float dot = Vector3.Dot(dir.normalized, forward);
		float sp = speed * Mathf.Max(dot, minMoveScale) * slowdown;


		if(Time.deltaTime > 0) {
			sp = Mathf.Clamp(sp, 0, targetDist/(Time.deltaTime*2));
		}
		return forward*sp;
	}

	private Vector3 GetFeetPosition() {
		if(controller != null) {
			return transform.position - Vector3.up*controller.height*0.5F;
		}

		return transform.position;
	}

	private float XZSqrMagnitude(Vector3 a, Vector3 b) {
		float dx = b.x-a.x;
		float dz = b.z-a.z;

		return dx*dx + dz*dz;
	}

	private Vector3 CalculateTargetPoint(Vector3 p, Vector3 a, Vector3 b) {
		a.y = p.y;
		b.y = p.y;

		float magn = (a-b).magnitude;
		if(magn == 0) return a;

		float closest = Mathf.Clamp01(VectorMath.ClosestPointOnLineFactor(a, b, p));
		Vector3 point = (b-a)*closest + a;
		float distance = (point-p).magnitude;

		float lookAhead = Mathf.Clamp(forwardLook - distance, 0.0F, forwardLook);

		float offset = lookAhead / magn;
		offset = Mathf.Clamp(offset+closest, 0.0F, 1.0F);
		return (b-a)*offset + a;
	}






	//Main method for Pathfinding :)
	public virtual void TravelToPath(Vector3 pos) {
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
