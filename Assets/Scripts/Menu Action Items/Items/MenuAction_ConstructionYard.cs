using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MenuAction_ConstructionYard : MenuActionItem {

	private GameObject prefab;
	
	public MenuAction_ConstructionYard(bool active) : base("Construction Yard", active) {
		prefab = (GameObject)Resources.Load("Construction Yard");
	}

	public override void Action() {
		GameManager.main.PlaceBuilding(prefab);
	}

	

}
