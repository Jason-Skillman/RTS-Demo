using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace RTSEngine {

	[Serializable]
	public abstract class RTSGameObject : MonoBehaviour {

		public static List<RTSGameObject> allRTSGameObjects = new List<RTSGameObject>();

		public TeamController teamController;
		public UnitType unitType;
		public Team team = Team.Netrual;
		public int maxHealth = 100;
		public int health = 100;
		public float attackRadius = 5.0f;                                       //Attack radius
		public float nearRadius = 1.0f;                                         //Near radius

		public bool canAttack = true;                                           //Can this RTS attack?

		public float attackTime = 0.8f;
		public float attackTimer {
			protected set;
			get;
		}

		public GameObject turret;                                               //Turret object						//Move me to Tank
		public GameObject bullet;                                               //Bullet to use on attack
		public GameObject bulletSpawn;                                          //Bullet spawn location


		//Flags
		public bool isSelected = false;
		public bool isAttacking = false;


		//private Color teamRed, teamBlue, teamYellow, teamgreen;


		//RTS target to lock on to
		public RTSGameObject targetLock = null;
		public bool isTargetLock {
			get {
				return (targetLock != null);
			}
		}
		

		//Menu actions
		public List<MenuActionItem> menuActions;



		//Method Messages
		public virtual void OnSelected() { }                                                            //
		public virtual void OnDeselected() { }                                                          //
		
		public virtual void OnHover() { }                                                               //
		public virtual void OnHoverDrag() { }                                                           //

		public virtual void OnPathTravelStart() { }                                                     //Called once when the path to the current target position has been calculated without error.
		public virtual void OnPathTravel() { }                                                          //Called while the object is traveling along the path to get to it's target position.
		public virtual void OnPathTravelUpdate() { }                                                    //Called everytime the object's recalculates it's path. (ex. When you give the object a new location to travel too.)
		public virtual void OnPathTravelEnd() { }                                                       //Called once when the object has reached it's target position.

		public virtual void OnTargetLockSearch() { }                                                    //Called while object is searching for a target lock
		public virtual void OnTargetLockEnter(RTSGameObject rtsGameObject) { }                          //Called once when this object has found a target lock
		public virtual void OnTargetLock(RTSGameObject rtsGameObject) { }                               //Called while this object has found a target lock
		public virtual void OnTargetLockLeave(RTSGameObject rtsGameObject) { }                          //Called once when this object has lost it's target lock

		public virtual void OnAttackingStart(RTSGameObject rtsGameObject) { }                           //Called once when this object starts attacking another object.
		public virtual void OnAttacking(RTSGameObject rtsGameObject) { }                                //Called while this object is attacking another object.
		public virtual void OnAttackingEnd() { }                                                        //Called once this object stops attacking another object.


		public virtual void Start() {
			//Create action menu
			menuActions = new List<MenuActionItem>();

			allRTSGameObjects.Add(this);
		}

		public virtual void Update() {
			if(teamController != null && canAttack) { //If we have a teamController && If we can attack
				if(!isTargetLock) { //If we dont have a target lock

					OnTargetLockSearch();   //Method message

					//Searching for targetLock using attack radius
					RaycastHit[] hits = Physics.SphereCastAll(this.transform.position, attackRadius, Vector3.up);
					foreach(RaycastHit hit in hits) {
						GameObject hitGameObject = hit.collider.gameObject;
						if(hitGameObject != gameObject) {   //If the gameObject is not ourself
							RTSGameObject hitRTSGameObject = hitGameObject.GetComponent<RTSGameObject>();
							if(hitRTSGameObject != null) {  //If the object is a RTSGameObject

								//if(hitRTSGameObject.unitType == UnitType.UnitEnemy || hitRTSGameObject.unitType == UnitType.BuildingEnemy) { 
								//	FindTargetLock(hitGameObject);
								//}
								
								if(teamController.IsRTSEnemy(hitRTSGameObject)) {
									FindTargetLock(hitGameObject);
								}

								if(teamController.team == Team.Red) {
									//Debug.Log("Attack: " + attackRadius);
								}
							}
						}
					}
				}
				//If we do have a target lock
				else {
					bool hasFoundTargetLock = false;

					//Check if the lock target still within range
					RaycastHit[] hits = Physics.SphereCastAll(this.transform.position, attackRadius, Vector3.up);
					foreach(RaycastHit hit in hits) {
						GameObject hitGameObject = hit.collider.gameObject;

						if(hitGameObject.GetComponent<RTSGameObject>() == targetLock) {   //If we have found our target lock
							hasFoundTargetLock = true;
							break;
						}
					}

					if(hasFoundTargetLock) {    //If the target lock is still within range
						//Debug.Log("Found target");
						OnTargetLock(targetLock);	//Method message
					} else {	//If the target left
						OnTargetLockLeave(targetLock);	//Method message
						targetLock = null;
					}
				}
			}

			if(health <= 0) {
				Destroy();
			}
		}

		public void Destroy() {
			allRTSGameObjects.Remove(this);
			teamController.allAllies.Remove(this);

			if(isSelected) {
				SelectedManager.main.DeselectObject(this);
			}

			Destroy(this.gameObject);
		}
		
		public void OnGUI() {
			//If the RTSObject is selected then draw the "Selected Overlay Texture"
			if(isSelected) {
				SelectedManager.main.DrawOverlay(this.gameObject.transform.position);
			}
		}
		
		public void OnDrawGizmosSelected() {
			//Attack radius
			if(canAttack) {
				Gizmos.color = Color.red;
				Gizmos.DrawWireSphere(this.transform.position, attackRadius);
			}

			//Near radius
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(this.transform.position, nearRadius);
		}
		

		public abstract void FindTargetLock(GameObject hitGameObject);
		public abstract void Attack(RTSGameObject rtsGameObject);
		

		//Getters and Setters
		#region Getters and Setters
		public int GetHealth() {
			return health;
		}
		public void AddHealth(int value) {
			health += value;
			if(health > maxHealth) {
				health = maxHealth;
			}
		}
		public void SubtractHealth(int value) {
			health -= value;
			if(health < 0) {
				health = 0;
			}
		}

		public int GetMaxHealth() {
			return maxHealth;
		}
		public void SetMaxHealth(int value) {
			maxHealth = value;
		}
		#endregion

	}

	public enum UnitType {
		Unit,
		Building

		//UnitFriendly,
		//UnitEnemy,
		//BuildingFriendly,
		//BuildingEnemy,
	}

}
