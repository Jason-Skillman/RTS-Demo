using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using RTSEngine;
using AStar;

public class GameManager : MonoBehaviour {

	//Singleton
	public static GameManager main;

	public Camera myCamera;
	public GameMode gameMode;
	public InteractionMode interactionMode;

	

	//Width of GUI menu
	private float m_GuiWidth;
	
	//Action Variables
	//private HoverOver hoverOver;lse;
	//private GameObject currentObject;
	
	//Mode Variables
	//private Mode m_Mode = Mode.Normal;
	
	//Interface variables the UI needs to deal with
	private IMiniMapController m_MiniMapController;
	
	//Building Placement variables
	private Action m_CallBackFunction;
	//private Item m_ItemBeingPlaced;
	private GameObject m_ObjectBeingPlaced;
	//private bool m_PositionValid = true;
	//private bool m_Placed = false;

	public bool readyToPlaceBuilding = false;
	public bool canPlaceBuilding = false;
	public GameObject buildingReadyToPlace;
	private Coroutine coroutineBuilding;

	bool IsShiftDown {
		set;
		get;
	}

	bool IsControlDown {
		set;
		get;
	}



	public void Awake() {
        if(main == null) main = this;
	}
	
	public void Start () {
		gameMode = GameMode.Normal;
		interactionMode = InteractionMode.Nothing;

		//Add Event Handlers
		EventManager.main.KeyBoardEvent += ShiftPressed;
        EventManager.main.KeyBoardEvent += ControlPressed;
        EventManager.main.KeyBoardEvent += KeyBoardPressedHandler;

		EventManager.main.MouseClickEvent += MouseClickedHandler;

		EventManager.main.MouseWheelEvent += MouseScrollHandler;

		EventManager.main.ScreenEdgeEvent += MouseAtScreenEdgeHandler;
	}

	public void Update() {

		HoverOver hoverOver = HoverManager.main.getHoverOver();
		if(gameMode != GameMode.PlaceBuilding) {
			if(hoverOver == HoverOver.UI) {
				gameMode = GameMode.Menu;
			} else {
				gameMode = GameMode.Normal;
			}
		}

		switch(gameMode) {
			case GameMode.Normal:
				GameMode_NormalUpdate();
				break;
			case GameMode.Menu:
				GameMode_MenuUpdate();
				break;
			case GameMode.PlaceBuilding:
				GameMode_PlaceBuilding();
				break;
		}

		//Update Cursors
		switch(interactionMode) {
			case InteractionMode.Nothing:
				CursorManager.main.ChangeCursor(CursorID.Normal);
				break;
			case InteractionMode.Select:
				CursorManager.main.ChangeCursor(CursorID.Selectable);
				break;
			case InteractionMode.Move:
				CursorManager.main.ChangeCursor(CursorID.Moveable);
				break;
			case InteractionMode.Attack:
				CursorManager.main.ChangeCursor(CursorID.Attackable);
				break;
			case InteractionMode.Build:
				CursorManager.main.ChangeCursor(CursorID.Build);
				break;
		}
	}

	//------------------------------Game Mode Updates------------------------------
	private void GameMode_NormalUpdate() {

		//Set variables
		interactionMode = InteractionMode.Nothing;
		HoverOver hoverOver = HoverManager.main.getHoverOver();
		List<RTSGameObject> allSelectedUnits = SelectedManager.main.GetSelectedObjects();
		

		//Is their any selected RTSGameObjects
		if(SelectedManager.main.GetSelectedObjectsCount() > 0) {
			RTSGameObject firstSelectedObject = SelectedManager.main.GetFirstSelectedObject();

			
			//Is the RTSGameObject able to attack
			if(firstSelectedObject.canAttack) {
				if(hoverOver == HoverOver.UnitEnemy || hoverOver == HoverOver.BuildingEnemy) {
					interactionMode = InteractionMode.Attack;
					return;
				}
			}
			
			//Is the RTSGameObject a Unit
			if(firstSelectedObject.GetComponent<Unit>()) {
				Unit unit = (Unit)firstSelectedObject;

				//Can the Unit move
				if(unit.canMove) {
					if(hoverOver == HoverOver.Terrain) {
						interactionMode = InteractionMode.Move;
						return;
					}
				}

				//Can the Unit interact
				if(unit.canInteract) {
					if(hoverOver == HoverOver.UnitFriendly) {
						//Check if Unit can interact with another Unit (Heal another unit for example)
					}
					if(hoverOver == HoverOver.BuildingFriendly) {
						//Check if Unit can interact with Building (Repair building for example)
					}
				}
			}
			
			//Is the RTSGameObject a Worker
			if(firstSelectedObject.GetComponent<Worker>()) {
				Worker worker = (Worker)firstSelectedObject;

				//Can the Worker build
				if(worker.canBuild) {
					if(hoverOver == HoverOver.BuildingFriendly) {
						Building building = (Building)HoverManager.main.currentHoverRTSObject;
						if(building.buildingMode == BuildingMode.Construction) {
							interactionMode = InteractionMode.Build;
						}
					}
				}
			}
		}
		//To select any kind of unit
		else if(hoverOver == HoverOver.UnitFriendly || hoverOver == HoverOver.BuildingFriendly || hoverOver == HoverOver.UnitEnemy || hoverOver == HoverOver.BuildingEnemy) {
			//Select Interaction
			interactionMode = InteractionMode.Select;
			return;
		}
	}

	private void GameMode_MenuUpdate() {
		//Debug.Log("GameMode: Menu");
		interactionMode = InteractionMode.Nothing;
	}

	private void GameMode_PlaceBuilding() {
		Vector3 newPos = new Vector3(HoverManager.main.hoverTerrainPosision.x, HoverManager.main.hoverTerrainPosision.y, HoverManager.main.hoverTerrainPosision.z);
		buildingReadyToPlace.transform.position = newPos;

		Building building = buildingReadyToPlace.GetComponent<Building>();

		bool colliding = false;
		foreach(RTSGameObject rtsGameObjects in RTSGameObject.allRTSGameObjects) {
			if(building) {
				if(building != rtsGameObjects) {
					if(building.GetComponent<Collider>().bounds.Intersects(rtsGameObjects.GetComponent<Collider>().bounds)) {
						colliding = true;
						break;
					}
				}
			}
		}

		if(AStar.Grid.main.NodeFromWorldPoint(newPos).isWalkable && !colliding) {
			//Debug.Log("Can Place");
			canPlaceBuilding = true;
			building.objectGhost.GetComponent<MeshRenderer>().material.color = Color.green;
		} else {
			//Debug.Log("Can't Place");
			canPlaceBuilding = false;
			building.objectGhost.GetComponent<MeshRenderer>().material.color = Color.red;
		}
	}

    

	//------------------------------KeyBoard Handler------------------------------
	private void ShiftPressed(KeyBoardEventArgs e) {
		if(e.keyCode == KeyCode.LeftShift && e.keyEventType == KeyEventType.Down) {
			IsShiftDown = true;
		} else if(e.keyCode == KeyCode.LeftShift && e.keyEventType == KeyEventType.Up) {
			IsShiftDown = false;
		}
	}

    private void ControlPressed(KeyBoardEventArgs e) {
		if(e.keyCode == KeyCode.LeftControl && e.keyEventType == KeyEventType.Down) {
			IsControlDown = true;
		} else if(e.keyCode == KeyCode.LeftControl && e.keyEventType == KeyEventType.Up) {
			IsControlDown = false;
		}
	}

	private void KeyBoardPressedHandler(KeyBoardEventArgs e) {
		if(e.keyCode == KeyCode.R && e.keyEventType == KeyEventType.Down) {
			if(gameMode == GameMode.PlaceBuilding) {
				//Quaternion newRotation = new Quaternion(buildingReadyToPlace.transform.rotation.x, buildingReadyToPlace.transform.rotation.y + 0.785398f, buildingReadyToPlace.transform.rotation.z, buildingReadyToPlace.transform.rotation.w);
                //buildingReadyToPlace.transform.rotation = newRotation;

                buildingReadyToPlace.transform.Rotate(0, 45, 0);
			}
		}
	}
	


	//------------------------------Mouse Button Handler------------------------------
	private void MouseClickedHandler(MouseClickEventArgs e) {
		SelectedManager selectedManager = SelectedManager.main;
        DragManager dragManager = DragManager.main;
        HoverManager hoverManager = HoverManager.main;
        RTSGameObject hoverRTS = hoverManager.getHoverRTSGameObject();

        //Place building
        if(e.buttonType == ButtonType.LeftButton && e.buttonEventType == ButtonEventType.Up) {
			if(gameMode == GameMode.PlaceBuilding) {
				if(!readyToPlaceBuilding) {	//Ready to place the building
					readyToPlaceBuilding = true;
				}
				else if(canPlaceBuilding) {	//Place the building
					//Debug.Log("Building placed");

					readyToPlaceBuilding = false;
					gameMode = GameMode.Normal;
					buildingReadyToPlace.GetComponent<Building>().buildingMode = BuildingMode.Construction;
					buildingReadyToPlace.GetComponent<Building>().objectBase.GetComponent<MeshRenderer>().material = TeamManager.main.player1.matColor;	//Set building base to team material color

					foreach(RTSGameObject rtsGameObject in selectedManager.GetSelectedObjects()) {
						Worker worker = rtsGameObject.GetComponent<Worker>();
						if(worker) {
							//worker.TravelToPath(buildingReadyToPlace.transform.position);
							worker.BuildBuilding(buildingReadyToPlace.GetComponent<Building>());
						}
					}
				} else {
					Debug.Log("Cant place building here");
				}
			}
		}


        
		//Drag select the RTSGameObjects
		if(dragManager.isDragging) {
            if(e.buttonType == ButtonType.LeftButton && e.buttonEventType == ButtonEventType.Down) {
                //Note: This will never run because you can start dragging on the first click. There is a threshold.
            }
            if(e.buttonType == ButtonType.LeftButton && e.buttonEventType == ButtonEventType.Holding) {
                selectedManager.DeselectAllObjects();
                foreach(RTSGameObject rtsGameObject in RTSGameObject.allRTSGameObjects) {
					if(TeamManager.main.player1.IsRTSAlly(rtsGameObject)) {
						if(rtsGameObject.unitType == UnitType.Unit) {
							if(dragManager.IsWithinDragBox(rtsGameObject.transform.position)) {
								dragManager.SelectObject(rtsGameObject);
								//selectedManager.SelectObject(rtsGameObject);

								rtsGameObject.OnHoverDrag();
							} else {
								dragManager.DeselectObject(rtsGameObject);
								//selectedManager.DeselectObject(rtsGameObject);
							}
						}
					}
				}
			}
            if(e.buttonType == ButtonType.LeftButton && e.buttonEventType == ButtonEventType.Up) {
                //Note: This will never run because dragManager.isDragging will be false when the button is up.
            }
        }
		else if(dragManager.selectedObjects.Count > 0) {
            //Select the drag group and clear the drag selection
            SelectedManager.main.SelectObject(dragManager.selectedObjects);
            dragManager.selectedObjects.Clear();
        }



		//Select the RTSGameObjects
		else if(e.buttonType == ButtonType.LeftButton && e.buttonEventType == ButtonEventType.Up) {
			if(gameMode == GameMode.Normal) {	//Make sure not to deselect the RTSGameObject while in any other mode (ex. HUD clicking)
				if(hoverManager.isHoverRTSObject) {  //If you are hovering over a RTSGameObject
                    
					if(TeamManager.main.player1.IsRTSAlly(hoverRTS)) { //Is this RTS friendly
						if(hoverRTS.unitType == UnitType.Unit) {	//Is this RTS a Unit
							if(IsShiftDown) {
								selectedManager.SelectObject(hoverRTS);
							} else {
								selectedManager.DeselectAllObjects();
								selectedManager.SelectObject(hoverRTS);
							}

						} else if(hoverRTS.unitType == UnitType.Building) { //Is this RTS a Building
							if(selectedManager.GetSelectedObjectsCount() == 0) {    //If you have nothing selected
								selectedManager.SelectObject(hoverRTS);
							}
						}
					}

					if(TeamManager.main.player1.IsRTSEnemy(hoverRTS)) {
						if(hoverRTS.unitType == UnitType.Unit) {
							//Debug.Log("You cant select a enemy unit");
						} else if(hoverRTS.unitType == UnitType.Building) {
                            //Debug.Log("You cant select a enemy building");
                        }
                    }

				} else {
					selectedManager.DeselectAllObjects();
				}
			}
		}
		
	}
    
	//------------------------------Scroll Wheel Handler-----------------------------
	private void MouseScrollHandler(MouseWheelEventArgs e) {
        //Todo: Zoom In/Out
        //m_Camera.Zoom(sender, e);
        //m_MiniMapController.ReCalculateViewRect();

        //Pan
        //m_Camera.Pan(sender, e);
        //m_MiniMapController.ReCalculateViewRect();
    }

    //------------------------------Mouse At Screen Edge Handler------------------------------
    Vector3 pos = new Vector3();
    private void MouseAtScreenEdgeHandler(ScreenEdgeEventArgs e) {
		
		//Camera Screen Move
		float newX = myCamera.transform.position.x;
		float newY = myCamera.transform.position.y;
		float newZ = myCamera.transform.position.z;
		float amount = 0.3f;

		if(e.screenEdgeEventType == ScreenEdgeEventType.Up) {
			newZ += amount;
			pos.Set(newX, newY, newZ);
			myCamera.transform.position = pos;
		} else if(e.screenEdgeEventType == ScreenEdgeEventType.Down) {
			newZ -= amount;
			pos.Set(newX, newY, newZ);
			myCamera.transform.position = pos;
		} else if(e.screenEdgeEventType == ScreenEdgeEventType.Right) {
			newX += amount;
			pos.Set(newX, newY, newZ);
			myCamera.transform.position = pos;
		} else if(e.screenEdgeEventType == ScreenEdgeEventType.Left) {
			newX -= amount;
			pos.Set(newX, newY, newZ);
			myCamera.transform.position = pos;
		}
	}


	
	public void PlaceBuilding(GameObject prefabBuilding) {       //Main method for placing Buildings
		gameMode = GameMode.PlaceBuilding;
		
		buildingReadyToPlace = Instantiate(prefabBuilding);

		Building building = buildingReadyToPlace.GetComponent<Building>();
		building.buildingMode = BuildingMode.Ghost;
		building.team = TeamManager.main.player1.team;
	}
	

}

public enum GameMode {
	Normal,
	Menu,
	PlaceBuilding
}

public enum InteractionMode {
	Nothing,
	Menu,
	Select,
	Move,
	Attack,
	Interact,
	Build,

	//Invalid = 1,
	//Deploy = 5,
	//Sell = 7,
	//CantSell = 8,
	//Fix = 9,
	//CantFix = 10,
	//Disable = 11,
	//CantDisable = 12,
}
