using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AStar {

	[RequireComponent(typeof(CharacterController))]
	public class Pathfinder : MonoBehaviour {

		public Path waypoints;                                                  //All nodes
		public Vector3 targetPosition;                                          //The position to travel to as a Vector3
		public int currentWaypoint = 0;                                         //The waypoint we are currently moving towards. Resets every time path is updated.

		public float speed = 10;												//Movement speed
		public float nextWaypointDistance = 1.2f;                               //Minimum distance you need to be to the current waypoint to go onto the next waypoint

		public float turnSpeed = 20;											//Turning speed
		[Obsolete()]
		public float nextWaypointTurn = 5;										//Minimum distance to start turning towards next waypoint

		public float updatePathInterval = 0.5f;                                 //Time to wait until pathfinder will recalculate it's position

		public bool isMoving = false;                                           //Is the object moving?
		
		
		private Coroutine coroutineUpdatePath;
		private Coroutine coroutineFollowPath;
		public delegate void Callback();
		private Callback CallbackStart, CallbackUpdate, CallbackUpdateTarget, CallbackEnd;


		public void Start() {
			//Testing V V V
			Node myNode = Grid.main.NodeFromWorldPoint(this.transform.position);
			List<Node> myNeighbours = Grid.main.GetNeighbours(myNode);

			
		}

		public void Update() {
			//Testing V V V
			List<Node> allNeighbourNodes = Grid.main.GetNeighbourNodesAroundObject(this.gameObject);

			foreach(Node node in allNeighbourNodes) {
				//node.color = Color.blue;
			}
		}

		/*
		public bool colliding = false;
		public void OnCollisionEnter(Collision collision) {
			if(collision.gameObject.GetComponent<RTSGameObject>() != null && collision.gameObject != this.gameObject) {
				Debug.Log("Enter");
				colliding = true;
				StartCoroutine(StopMoving());
			}
		}
		public void OnCollisionExit(Collision collision) {
			if(collision.gameObject.GetComponent<RTSGameObject>() != null && collision.gameObject != this.gameObject) {
				Debug.Log("Exit");
				colliding = false;
				//StopCoroutine(StopMoving());
			}
		}
		public IEnumerator StopMoving() {
			yield return new WaitForSeconds(2);
			if(colliding) {
				Debug.Log("Stop Moving");
				isMoving = false;
				StopCoroutine(StopMoving());
			}
			StopCoroutine(StopMoving());
		}
		*/



		//##### MAIN METHOD TO CALL FOR PATHFINDING #####
		public void TravelToPath(Vector3 targetPosition, Callback CallbackStart, Callback CallbackUpdate, Callback CallbackUpdateTarget, Callback CallbackEnd) {
			this.CallbackStart = CallbackStart;
			this.CallbackUpdate = CallbackUpdate;
			this.CallbackUpdateTarget = CallbackUpdateTarget;
			this.CallbackEnd = CallbackEnd;

			TravelToPath(targetPosition);
		}
		public void TravelToPath(Vector3 targetPosition) {
			//Debug.Log("TravelToPath");
			this.targetPosition = targetPosition;
			isMoving = true;

			if(targetPosition == Vector3.zero || targetPosition == null) {
				Debug.LogError("Cant travel to null location");
			}

			if(coroutineUpdatePath != null)
				StopCoroutine(coroutineUpdatePath);
			coroutineUpdatePath = StartCoroutine("UpdatePath");
		}

		[Obsolete("Use UpdatePath() instead.")]
		private IEnumerator UpdatePathOld() {
			//Fixes/Solves large delta time errors
			if(Time.timeSinceLevelLoad < 0.3f) {
				yield return new WaitForSeconds(0.3f);
			}

			CallbackStart(); //Call the callback delagate

			Vector3 targetPositionOld = targetPosition;

			//Request the path from the PathfinderManager
			//PathfinderManager.main.RequestPathOld(new PathRequest(this.transform.position, targetPosition, OnPathCalculation));       //Undo

			while(true) {   //Update path
				if(updatePathInterval > 0) {
					yield return new WaitForSeconds(updatePathInterval);
				} else {
					yield return new WaitForSeconds(Mathf.Infinity);
				}
				Debug.Log("Updated path");

				CallbackUpdateTarget(); //Call the callback delagate

				//PathfinderManager.main.RequestPathOld(new PathRequest(this.transform.position, targetPosition, OnPathCalculation));   //Undo
				targetPositionOld = targetPosition;
				currentWaypoint = 0;
			}
			
		}

		private IEnumerator UpdatePath() {
			//Fixes/Solves large delta time errors
			if(Time.timeSinceLevelLoad < 0.3f) {
				yield return new WaitForSeconds(0.3f);
			}

			CallbackStart(); //Call the callback delagate

			Vector3 targetPositionOld = targetPosition;

			do {
				yield return null;
				
				//PathfinderManager.main.RequestPath(new PathRequest(this.transform.position, targetPosition, OnPathCalculation));
				PathfinderManager.main.RequestPath(this.transform.position, targetPosition, OnPathCalculation, PathType.EndOfTheLine);

				targetPositionOld = targetPosition;
				currentWaypoint = 0;

				CallbackUpdateTarget(); //Call the callback delagate

				if(updatePathInterval > 0) {
					yield return new WaitForSeconds(updatePathInterval);
				} else {
					yield return new WaitForSeconds(Mathf.Infinity);
				}
			} while(true);
		}

        //List<Node> nodeWaypoints;
        //Called when the path has been caculated
        private void OnPathCalculation(List<Node> nodeWaypoints, Vector3[] waypoints, bool pathSuccessful) {
			if(pathSuccessful) {
                foreach(Node node in nodeWaypoints) {
                    node.color = Color.blue;
                }

                //this.nodeWaypoints = nodeWaypoints;
                
                this.waypoints = new Path(waypoints, this.transform.position, nextWaypointTurn);

				if(coroutineFollowPath != null)
					StopCoroutine(coroutineFollowPath);
				coroutineFollowPath = StartCoroutine("FollowPath");
			} else {
				Debug.LogError("Path failed");
				StopMoving();
			}
		}

		/*
		//Moves the seeker along the path
		public IEnumerator FollowPath() {
			int pathIndex = 0;
			this.transform.LookAt(path.waypoints[0]);

			while(true) {
				Vector2 position2D = new Vector2(this.transform.position.x, this.transform.position.z);
				while(path.turnBoundaries[pathIndex].HasCrossedLine(position2D)) {
					//If the seeker has reached the end of its path
					if(pathIndex == path.finishLineIndex) {
						isMoving = false;
						OnEndOfPath();  //Method message

						break;
					} else {
						pathIndex++;
					}
				}

				if(isMoving) {
					Quaternion targetRoation = Quaternion.LookRotation(path.waypoints[pathIndex] - this.transform.position);
					this.transform.rotation = Quaternion.Lerp(this.transform.rotation, targetRoation, turnSpeed * Time.deltaTime);

					Vector3 direction = new Vector3();
					direction *= speed;

					this.transform.Translate(Vector3.forward * Time.deltaTime * speed, Space.Self);
					//this.GetComponent<CharacterController>().SimpleMove(Vector3.forward * speed);
					//this.GetComponent<Rigidbody>().AddForce(Vector3.forward * speed);
				}
				yield return null;
			}

		}
		*/


		//Moves the object along the path
		private IEnumerator FollowPath() {
			Vector3 direction = new Vector3();
			Vector2 position2D;

			while(true) {
				yield return null;

				position2D = new Vector2(this.transform.position.x, this.transform.position.z);

				CallbackUpdate();   //Call the callback delagate
				
				if(currentWaypoint < waypoints.waypoints.Length) {	//If their is a waypoint to move towards
					//Direction and speed to the next waypoint
					direction = (waypoints.waypoints[currentWaypoint] - this.transform.position).normalized;
					direction *= speed;

					//Rotate towards the next path
					RotateTowards(direction);

					//Move the object
					//this.transform.Translate(direction * Time.deltaTime, Space.World);
					this.GetComponent<CharacterController>().SimpleMove(direction);
					//this.GetComponent<Rigidbody>().MovePosition(direction);

					//If we are near the next waypoint. Based on "nextWaypointDistance" variable
					if((transform.position - waypoints.waypoints[currentWaypoint]).sqrMagnitude < nextWaypointDistance * nextWaypointDistance) {
						currentWaypoint++;
					}
				} else {    //If we are at the end of the waypoint list
					//End of the path
					currentWaypoint = 1;

					OnReachedEndOfPath();
					CallbackEnd();   //Call the callback delagate
				}
			}
		}

		private void OnReachedEndOfPath() {
			StopMoving();
		}


		//##### MAIN METHOD TO CALL TO STOP PATHFINDING #####
		public void StopMoving() {
			isMoving = false;
			if(coroutineUpdatePath != null)
				StopCoroutine(coroutineUpdatePath);
			if(coroutineFollowPath != null)
				StopCoroutine(coroutineFollowPath);
		}
		
		public void RotateTowards(Vector3 dir) {
			if(dir != Vector3.zero) {
				Quaternion rot = transform.rotation;
				Quaternion toTarget = Quaternion.LookRotation(dir);

				rot = Quaternion.Slerp(rot, toTarget, turnSpeed*Time.deltaTime);
				Vector3 euler = rot.eulerAngles;
				euler.z = 0;
				euler.x = 0;
				rot = Quaternion.Euler(euler);

				transform.rotation = rot;
			}
		}
		

		public void OnDrawGizmos() {
			//Draws the path's gizmos
			if(isMoving) {
				waypoints.DrawGizmos();
			}
		}
		
	}

}