using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace RTSEngine {

	[Serializable]
	public class Worker : Infantry {

		public bool canBuild = true;                                   //Can this RTS build buildings?
		public int buildPointAmount = 1;

		public bool isBuilding = false;
		public Building buildingToBuild = null;
		public bool isGoingToBuild {
			get {
				return (buildingToBuild != null);
			}
		}
		private Coroutine coroutineBuild = null;


		public override void Start() {
			base.Start();

			//menuActions.Add(new MenuAction_Base(true));
			menuActions.Add(new MenuAction_Airfield(true));
			menuActions.Add(new MenuAction_Barracks(true));
			menuActions.Add(new MenuAction_ConstructionYard(true));
			menuActions.Add(new MenuAction_PowerPlant(true));
			menuActions.Add(new MenuAction_Radar(true));
			menuActions.Add(new MenuAction_Refinery(true));
			menuActions.Add(new MenuAction_TechCenter(true));
			menuActions.Add(new MenuAction_WarFactory(true));
		}

		public override void Update() {
			base.Update();

			if(isGoingToBuild) {
				if(buildingToBuild.IsConstructed) {
					isBuilding = false;
					buildingToBuild = null;
					StopCoroutine(coroutineBuild);
				}
			}
		}

		public override void OnPathTravelStart() {
			base.OnPathTravelStart();


		}



		public void BuildBuilding(Building building) {
			Debug.Log("Building Placed");
			buildingToBuild = building;
			TravelToPath(buildingToBuild.gameObject.transform.position);

			coroutineBuild = StartCoroutine(Build());
		}
		private IEnumerator Build() {
			while(true) {
				yield return null;

				RaycastHit[] hits = Physics.SphereCastAll(this.transform.position, nearRadius, Vector3.forward);
				foreach(RaycastHit hit in hits) {
					Building hitBuilding = hit.collider.gameObject.GetComponent<Building>();

					if(hitBuilding && hitBuilding.buildingMode == BuildingMode.Construction) {
						if(hitBuilding == buildingToBuild) {
							//Debug.Log("Reach building to build");
							buildingToBuild.constructionPoints += buildPointAmount;
							isBuilding = true;
							StopMoving();
							break;
						} else {
							//Debug.Log("WRONG building to build");
						}
					}
				}
			}
		}

	}

}