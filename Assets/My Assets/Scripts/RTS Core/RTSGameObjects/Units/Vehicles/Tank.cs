using UnityEngine;
using System.Collections;

namespace RTSEngine {

	public class Tank : Vehicle {

		public override void Start() {
			base.Start();
			
		}

		public override void Update() {
			base.Update();
			
		}


		public override void OnSelected() {
			//Debug.Log("OnSelected()");
		}
		public override void OnDeselected() {
			//Debug.Log("OnDeselected()");
		}


		public override void OnHover() {
			//Debug.Log("OnHover()");
		}


		public override void OnHoverDrag() {
			//Debug.Log("OnHoverDrag()");
		}


		public override void OnPathTravelStart() {
			//Debug.Log("OnPathTravelStart()");
		}
		public override void OnPathTravel() {
			//Debug.Log("OnPathTravel()");
		}
		public override void OnPathTravelUpdate() {
			//Debug.Log("OnPathTravelUpdate()");
		}
		public override void OnPathTravelEnd() {
			//Debug.Log("OnPathTravelEnd()");
		}

		public override void OnPursuing(RTSGameObject rtsGameObject) {
			//Debug.Log("OnPursuing");
		}

		public override void OnAttackingStart(RTSGameObject rtsGameObject) {
			//Debug.Log("OnAttackingStart");
		}
		public override void OnAttacking(RTSGameObject rtsGameObject) {
			//Debug.Log("OnAttacking");
		}
		public override void OnAttackingEnd() {
			//Debug.Log("OnAttackingEnd");
		}

	}

}