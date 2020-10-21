using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RTSEngine;

public class SelectedManager : MonoBehaviour {
    
    //Singleton
	public static SelectedManager main;
		
	public List<RTSGameObject> selectedObjects = new List<RTSGameObject>();
	public List<List<RTSGameObject>> savedGroups = new List<List<RTSGameObject>>();

	//Overlay
	private Rect rect;
	public Texture2D tex;
	public float overlaySize = 50.0f;


	public void Awake() {
        if(main == null) main = this;
	    
	    //Create 9 saved groups
		for(int i = 0; i < 9; i++) {
			savedGroups.Add(new List<RTSGameObject>());
		}
	}

	public void Start() {
		EventManager.main.MouseClickEvent += MouseClickSelectUnit;
	}



	private void MouseClickSelectUnit(MouseClickEventArgs e) {
		
        //Give the selected units an action
		if(e.buttonType == ButtonType.RightButton && e.buttonEventType == ButtonEventType.Down) {
			if(GetSelectedObjectsCount() > 0 && GameManager.main.gameMode == GameMode.Normal) {	//If there is at least one object selected && you are in normal mode

				//Mode: Move
				if(GameManager.main.interactionMode == InteractionMode.Move) {
					foreach(Unit unit in selectedObjects) {
						//MyAIPath myAIPath = gameObject.GetComponent<MyAIPath>();
						//myAIPath.TravelToPath(e.worldPosition);

						//Unit unit = (Unit)gameObject;
						//unit.TravelToPath(e.worldPosition);

						//Path_NoMovement move = gameObject.GetComponent<Path_NoMovement>();
						//move.TravelToPath(e.worldPosition);

						//Path_Movement move2 = gameObject.GetComponent<Path_Movement>();
						//move2.TravelToPath(e.worldPosition);

						//Path_AdvancedMovement move3 = gameObject.GetComponent<Path_AdvancedMovement>();
						//move3.TravelToPath(e.worldPosition);

						//RTSPathfinder pathFinder = gameObject.GetComponent<RTSPathfinder>();		//Original working
						//pathFinder.TravelToPath(e.worldPosition);

						//Pathfinder pathfinder = gameObject.GetComponent<Pathfinder>();
						//pathfinder.TravelToPath(e.worldPosition);
						unit.TravelToPath(e.worldPosition);

						if(unit.targetToPursue) {
							unit.StopAttacking();
						}
					}
				}
				//Mode: Attack
				else if(GameManager.main.interactionMode == InteractionMode.Attack) {
					foreach(Unit unit in selectedObjects) {
						unit.Attack(HoverManager.main.currentHoverRTSObject);
					}
				}
				//Mode: Build
				else if(GameManager.main.interactionMode == InteractionMode.Build) {
					foreach(Worker worker in selectedObjects) {
						if(worker) {
							if(worker.canBuild) {
								worker.BuildBuilding((Building)e.gameObject);
							} else {
								Debug.Log("Worker can't build right now.");
							}
						}
					}
				}

			}
		}

	}
	
	public void SelectObject(RTSGameObject gameObject) {
		if(!selectedObjects.Contains(gameObject)) { //If the selectedObject does NOT already contain the newly selected object
            
            //Add the object to the list of selected objects
            selectedObjects.Add(gameObject);

            //GameObject select data and method message
            gameObject.isSelected = true;
            gameObject.OnSelected();

            CursorManager.main.ChangeCursor(CursorID.Moveable);
        } else {
			//If the object is already selected and tried to be selected again
		}
	}

	public void SelectObject(List<RTSGameObject> gameObjectList) {
		foreach(RTSGameObject rtsGameObject in gameObjectList) {
			if(!selectedObjects.Contains(rtsGameObject)) { //If the selectedObject does NOT already contain the newly selected object
                
				if(TeamManager.main.player1.IsRTSAlly(rtsGameObject)) {
					if(rtsGameObject.unitType == UnitType.Unit) {
						selectedObjects.Add(rtsGameObject);

						//GameObject select stuff
						rtsGameObject.isSelected = true;
						rtsGameObject.OnSelected();

						CursorManager.main.ChangeCursor(CursorID.Moveable);
					}
				}

			} else {
				//If the object is already selected and tried to be selected again
			}
		}
	}

	public void DeselectObject(RTSGameObject gameObject) {
        //GameObject select data and method message
        gameObject.isSelected = false;
		gameObject.OnDeselected();

		//Remove the object from the list of selected objects
		selectedObjects.Remove(gameObject);

		CursorManager.main.ChangeCursor(CursorID.Normal);
	}
	
	public void DeselectAllObjects() {
		foreach(RTSGameObject gameObject in selectedObjects) {
			gameObject.isSelected = false;
			gameObject.OnDeselected();
		}
		selectedObjects.Clear();

		CursorManager.main.ChangeCursor(CursorID.Normal);
	}

	
	public void DrawOverlay(Vector3 pos) {
		//Screen coversion
		Vector3 screenPos = Camera.main.WorldToScreenPoint(pos);
		Vector3 realScreenPos = new Vector3(screenPos.x, Screen.height-screenPos.y, screenPos.z);

		//Create rech bounds
		rect.width = overlaySize;
		rect.height = overlaySize;
		rect.x = realScreenPos.x - (overlaySize/2);
		rect.y = realScreenPos.y - (overlaySize/2);

		//Draw
		GUI.DrawTexture(rect, tex);
	}
	
	
	public void SaveSelectionToGroup(int groupNumber) {
	    //Clear the current group to save to
		savedGroups[groupNumber].Clear ();
		
		foreach(RTSGameObject gameObject in selectedObjects) {
			savedGroups[groupNumber].Add(gameObject);
		}
	}
	
	public void SelectSavedGroup(int groupNumber) {
		DeselectAllObjects();
		
		foreach(RTSGameObject gameObject in savedGroups[groupNumber]) {
			SelectObject(gameObject);
		}
	}
	
	
	
	public int GetSelectedObjectsCount() {
		return selectedObjects.Count;
	}
	
	public List<RTSGameObject> GetSelectedObjects() {
	    return selectedObjects;
	}
	
	public RTSGameObject GetFirstSelectedObject() {
		if(GetSelectedObjectsCount() != 0) {
			return selectedObjects[0];
		}
		Debug.LogWarning("No objects are selected.");
		return null;
	}
	
	public bool IsObjectSelected(RTSGameObject gameObject) {
		return selectedObjects.Contains(gameObject);
	}
}
