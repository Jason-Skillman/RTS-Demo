using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RTSEngine;

public class Bullet : MonoBehaviour {

	public int damage = 10;
	public float strength = 1.0f;
	public Team team = Team.Netrual;


	public void Start() {
		Destroy(this.gameObject, 5);
	}

	public void Update() {
		this.transform.position += this.transform.forward * strength;
	}

	public void OnTriggerEnter(Collider other) {
		//Debug.Log("Hit");
		RTSGameObject rtsGameObject = other.gameObject.GetComponent<RTSGameObject>();
		if(rtsGameObject != null) {
			
			//if(rtsGameObject.unitType == UnitType.UnitEnemy) {
			//	rtsGameObject.SubtractHealth(damage);
			//	Destroy(this.gameObject);
			//	//Debug.Log("Dead");
			//}

			if(team != rtsGameObject.team) {
				rtsGameObject.SubtractHealth(damage);
				Destroy(this.gameObject);
				//Debug.Log("Destroy");
			}

		}
	}

}