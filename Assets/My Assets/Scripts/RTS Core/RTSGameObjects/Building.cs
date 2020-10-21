using UnityEngine;
using System.Collections;

namespace RTSEngine {

	public abstract class Building : RTSGameObject {

		public BuildingMode buildingMode = BuildingMode.Construction;

		public int constructionPointsMax = 2000;
		public int constructionPoints = 0;
		public bool IsConstructed {
			set {
				if(value == true) {
					constructionPoints = constructionPointsMax;
				} else {
					constructionPoints = 0;
				}
			}
			get {
				return (constructionPoints >= constructionPointsMax);
			}
		}

		//GameObject Parts
		private BoxCollider boxCollider;
		public GameObject objectGhost;
		public GameObject objectBase;
		public GameObject objectConstruction;


		public override void Start() {
			base.Start();

			boxCollider = this.GetComponent<BoxCollider>();

			//Building Properties (Override)
			unitType = UnitType.Building;
		}

		public override void Update() {
			base.Update();

			switch(buildingMode) {
				case BuildingMode.Ghost:
					ModeGhost();
					break;
				case BuildingMode.Construction:
					ModeConstruction();
					break;
				case BuildingMode.Built:
					ModeBuilt();
					break;
				case BuildingMode.Destroy:
					break;
			}

		}

		public override void FindTargetLock(GameObject hitGameObject) {
			targetLock = hitGameObject.GetComponent<RTSGameObject>();
			OnTargetLockEnter(targetLock);
		}

		public override void Attack(RTSGameObject rtsGameObject) {
			//Todo:
		}

		public void ModeGhost() {
			//boxCollider.enabled = false;
			boxCollider.isTrigger = true;
			objectGhost.SetActive(true);
			objectBase.SetActive(false);
			objectConstruction.SetActive(false);
		}

		public void ModeConstruction() {
			//boxCollider.enabled = true;
			boxCollider.isTrigger = false;
			objectGhost.SetActive(false);
			objectBase.SetActive(false);
			objectConstruction.SetActive(true);

			if(IsConstructed) {
				buildingMode = BuildingMode.Built;
			}
		}

		public void ModeBuilt() {
			boxCollider.enabled = true;
			objectGhost.SetActive(false);
			objectBase.SetActive(true);
			objectConstruction.SetActive(false);
		}

	}

	public enum BuildingMode {
		Ghost,
		Construction,
		Built,
		Destroy
	}

}
