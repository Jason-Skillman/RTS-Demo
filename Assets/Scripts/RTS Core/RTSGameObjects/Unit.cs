using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using AStar;

namespace RTSEngine {

	[RequireComponent(typeof(Pathfinder))]
	public abstract class Unit : RTSGameObject {

		public bool canMove = true;                                             //Can this RTS move?
		public bool canInteract = true;                                         //Can this RTS interact with?

		private Pathfinder pathfinder;
		private Coroutine coroutineAttack = null;

		//RTS target to chase down
		bool flagTargetToPursueStart = true;
		public RTSGameObject targetToPursue = null;
		public bool isPursuing {
			get {
				return (targetToPursue != null);
			}
		}

		//Method Messages
		public virtual void OnPursuing(RTSGameObject rtsGameObject) { }                                 //Called while this object is pursuing another object.


		public override void Start() {
			base.Start();

			pathfinder = this.GetComponent<Pathfinder>();

			//Unit Properties (Override)
			unitType = UnitType.Unit;
		}

		public override void Update() {
			base.Update();

			if(isPursuing) {
				OnPursuing(targetToPursue); //Method message
			}
		}



		public override void FindTargetLock(GameObject hitGameObject) {
			if(!isPursuing) {   //We are not pursueing anyone so we can track this target
				targetLock = hitGameObject.GetComponent<RTSGameObject>();

				//Debug.Log("New target locked on: " + hitGameObject.name);
				OnTargetLockEnter(targetLock);
			} else {    //We already have a target to pursue
				if(targetToPursue == hitGameObject.GetComponent<RTSGameObject>()) {    //Check if the target is the the target we are trying to pursue
					
					//We have found the target we were pursing, so track the target
					targetLock = hitGameObject.GetComponent<RTSGameObject>();

					Debug.Log("Pursuing target locked on: " + hitGameObject.name);
					OnTargetLockEnter(targetLock);
				} else {    //We are trying to pursue another target so dont track this target
					//Debug.Log("Dont track me: " + hitGameObject.name);
				}
			}
		}



		//------------------------------ OnTargetLock ------------------------------
		#region OnTargetLock
		public override void OnTargetLockEnter(RTSGameObject rtsGameObject) {
			if(coroutineAttack != null)
				StopCoroutine(coroutineAttack);
			coroutineAttack = StartCoroutine(Attack(turret, targetLock.gameObject));
		}

		public override void OnTargetLock(RTSGameObject rtsGameObject) {
			if(isPursuing) {
				StopMoving();
			}
		}

		public override void OnTargetLockLeave(RTSGameObject rtsGameObject) {
			StopAttacking();
		}
		#endregion


		//------------------------------ Pathfinder ------------------------------
		#region Pathfinder
		//Travels to the given location
		public void TravelToPath(Vector3 position) {
			if(canMove)
				pathfinder.TravelToPath(position, OnPathTravelStart, OnPathTravel, OnPathTravelUpdate, OnPathTravelEnd);
		}

		//Stops the object from moving
		public void StopMoving() {
			pathfinder.StopMoving();
		}
		#endregion


		//------------------------------ Attacking ------------------------------
		#region Attacking
		public override void Attack(RTSGameObject rtsGameObject) {
			if(targetToPursue != rtsGameObject) {   //If we are not aleready pursuing this RTS target
				targetToPursue = HoverManager.main.currentHoverRTSObject;

				if(isTargetLock) {  //We are already targeting someone
					targetLock = targetToPursue;

					if(coroutineAttack != null)
						StopCoroutine(coroutineAttack);
					coroutineAttack = StartCoroutine(Attack(turret, targetLock.gameObject));
				} else {	//If we do not yet have a targetLock
					
				}

				TravelToPath(targetToPursue.transform.position);
			} else {
				//Debug.Log("Already pusuing this target.");
			}
		}

		public void StopAttacking() {
			isAttacking = false;
			targetToPursue = null;
			targetLock = null;

			if(coroutineAttack != null)
				StopCoroutine(coroutineAttack);
			OnAttackingEnd();   //Method message
		}

		//Rotate Turret then attack (Old)
		[Obsolete("Use Attack() instead.")]
		public IEnumerator RotateAndAttack(GameObject obj, GameObject target, float speed = 1.0f) {

			bool flagAttackStart = true, flagAttackEnd = true;
			while(true) {
				yield return null;

				Quaternion myRotation = obj.transform.rotation;
				//Find the vector pointing from our position to the target
				Vector3 direction = (target.transform.position - obj.transform.position).normalized;
				//Create the rotation we need to be in to look at the target
				Quaternion lookRotation = Quaternion.LookRotation(direction);
				//rotate us over time according to speed until we are in the required rotation
				obj.transform.rotation = Quaternion.Slerp(myRotation, lookRotation, Time.deltaTime * speed);    //Todo: change to liner translation

				//Debug.Log(lookRotation.y + "   " + myRotation.y);


				float threshold = 0.002f;
				if(Mathf.Abs(lookRotation.y) <= Mathf.Abs(myRotation.y) + threshold && Mathf.Abs(lookRotation.y) >= Mathf.Abs(myRotation.y) - threshold) {
					//Debug.Log("Looking at target");
					if(flagAttackStart) {
						flagAttackStart = false;
						OnAttackingStart(target.GetComponent<RTSGameObject>());
					}
					isAttacking = true;
					OnAttacking(target.GetComponent<RTSGameObject>());
				} else {
					//Debug.Log("Not looking at target");
					if(flagAttackEnd && !flagAttackStart) {
						flagAttackEnd = false;
						OnAttackingEnd();
					}
					isAttacking = false;
				}

			}
			//StopCoroutine(coroutineAttack); //Unreachable
		}

		//Turret LookAt then attack
		public IEnumerator Attack(GameObject obj, GameObject target) {
			bool flagAttackStart = true;
			while(true) {
				yield return null;

				//Start
				if(flagAttackStart) {
					flagAttackStart = false;
					OnAttackingStart(target.GetComponent<RTSGameObject>()); //Method message
				}

				attackTimer -= Time.deltaTime;
				if(target == null) StopCoroutine(coroutineAttack);
				else obj.transform.rotation = Quaternion.LookRotation(target.transform.position - obj.transform.position, Vector3.up);

				if(attackTimer <= 0.0f) {
					isAttacking = true;
					attackTimer = attackTime;

					ShootBullet();
				}

				if(target != null) OnAttacking(target.GetComponent<RTSGameObject>());  //Method message
			}

		}

		public void ShootBullet() {
			Bullet myBullet = bullet.GetComponent<Bullet>();
			myBullet.team = team;

			Instantiate(bullet, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
		}
		#endregion

	}

}