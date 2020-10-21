using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using RTSEngine;

public class HoverManager : MonoBehaviour {

	//Singleton
	public static HoverManager main;

	public Vector3 hoverTerrainPosision;
	public GameObject currentHoverGameObject;
	public RTSGameObject currentHoverRTSObject;
	public bool isHoverRTSObject = false;
	public HoverOver hoverOver;


	public void Awake() {
        if(main == null) main = this;
	}
    
	public void Update() {
		//Reset variables
		currentHoverGameObject = null;
		currentHoverRTSObject = null;
		isHoverRTSObject = false;
		hoverOver = HoverOver.Nothing;
		

		//2D Raycast
		PointerEventData pointerData = new PointerEventData(EventSystem.current);
		pointerData.position = Input.mousePosition;

		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(pointerData, results);

		if(results.Count > 0) {
			if(results[0].gameObject.layer == LayerMask.NameToLayer("UI")) {
				//string dbg = "Root Element: {0} \n GrandChild Element: {1}";
				//Debug.Log(string.Format(dbg, results[results.Count-1].gameObject.name, results[0].gameObject.name));
				//results.Clear();

				hoverOver = HoverOver.UI;
			}
		}


		//3D Raycast with all objects
		{
			int rayLength = 50;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit[] hits = Physics.RaycastAll(ray, rayLength);
			for(int i = 0; i < hits.Length; i++) {
				RaycastHit hit = hits[i];

				//Layer only
				if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Terrain")) {
					hoverTerrainPosision = hit.point;
				}
			}
		}
		
		//3D Raycast
		if(hoverOver == HoverOver.Nothing) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit, Mathf.Infinity)) { //LayerMask.NameToLayer("GameObject")
				currentHoverGameObject = hit.collider.gameObject;
				
				//If the gameObject is the terrain
				if(currentHoverGameObject.layer == LayerMask.NameToLayer("Terrain")) {
					hoverOver = HoverOver.Terrain;
				}
				//If the gameObject is a RTSGameObject
				else if(currentHoverGameObject.GetComponent<RTSGameObject>() != null) {
					currentHoverRTSObject = currentHoverGameObject.GetComponent<RTSGameObject>();
					isHoverRTSObject = true;
                    

					if(TeamManager.main.player1.IsRTSAlly(currentHoverRTSObject)) {
						if(currentHoverRTSObject.unitType == UnitType.Unit)
							hoverOver = HoverOver.UnitFriendly;
						else if(currentHoverRTSObject.unitType == UnitType.Building)
							hoverOver = HoverOver.BuildingFriendly;
					}
					else if(TeamManager.main.player1.IsRTSEnemy(currentHoverRTSObject)) {
						if(currentHoverRTSObject.unitType == UnitType.Unit)
							hoverOver = HoverOver.UnitEnemy;
						else if(currentHoverRTSObject.unitType == UnitType.Building)
							hoverOver = HoverOver.BuildingEnemy;
					} else
						hoverOver = HoverOver.Nothing;
					
					currentHoverRTSObject.OnHover();	//RTS method message
				}

			
			}
		}
		
	}
    

	public HoverOver getHoverOver() {
		return hoverOver;
	}

	public RTSGameObject getHoverRTSGameObject() {
		return currentHoverRTSObject;
	}

}

public enum HoverOver {
	Nothing,
	UI,
	UnitFriendly,
	UnitEnemy,
	BuildingFriendly,
	BuildingEnemy,
	Terrain
}
