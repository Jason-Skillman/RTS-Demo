using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RTSEngine;

public class HUDManager : MonoBehaviour {

	public static HUDManager main;
	public GameObject HUD;

	private GameObject unitName;
	private GameObject unitPic;
	private GameObject unitActionMenu;

	RTSGameObject previousTemp;
	RTSGameObject firstSelectedGameObjects;

	
	public void Start () {
		int childrenCount = HUD.transform.childCount;
		for(int i = 0; i < childrenCount; ++i) {
			//Debug.Log("Name: " + transform.GetChild(i).gameObject.name);
			GameObject gameObject = HUD.transform.GetChild(i).gameObject;
			
			switch(gameObject.name) {
				case "Unit Name":
					unitName = gameObject;
					break;
				case "Unit Pic":
					unitPic = gameObject;
					break;
				case "Unit Action Menu":
					unitActionMenu = gameObject;
					break;
			}
		}
	}
	
	public void Update () {
		firstSelectedGameObjects = SelectedManager.main.GetFirstSelectedObject();
		
		if(firstSelectedGameObjects == null && previousTemp != null) {
			OnDeselectUnit(previousTemp);
			previousTemp = null;
		}
		else if(previousTemp != firstSelectedGameObjects) {
			previousTemp = firstSelectedGameObjects;
			OnNewSelectedUnit(firstSelectedGameObjects);
		}
		else {
			//Debug.Log("Update");
		}
	}

	List<MenuActionItem> menuActions;
	public void OnNewSelectedUnit(RTSGameObject rtsGameObject) {
		//Debug.Log("OnNewSelectedUnit()");

		//Unit name
		unitName.GetComponent<Text>().text = rtsGameObject.name;

		//Unit pic
		unitPic.GetComponent<Image>().color = Color.blue;

		//Unit action menu
		GameObject gridLayout = unitActionMenu.transform.GetChild(0).gameObject;
		int childrenCount = gridLayout.transform.childCount;

		menuActions = rtsGameObject.menuActions;

		for(int i = 0; i < childrenCount; ++i) {
			GameObject childGameObject = gridLayout.transform.GetChild(i).gameObject;
			Button childButton = childGameObject.GetComponent<Button>();
			Text childText = childGameObject.transform.GetChild(0).GetComponent<Text>();
			
			if(menuActions.Count >= i+1) {	//Buttons to edit
				//Debug.Log("Button to edit: " + i);
				childButton.interactable = menuActions[i].active;
				childText.text = menuActions[i].name;
				childButton.onClick.AddListener(menuActions[i].Action);
				//childButton.onClick.AddListener(OnClickActionEvent);
			} else {	//Spill over buttons
				//Debug.Log("Spill over: " + childGameObject.name);
				childButton.interactable = false;
				childText.text = "";
				childButton.onClick.RemoveAllListeners();
			}
		}

	}
	public void OnClickActionEvent() {
		foreach(MenuActionItem item in menuActions) {
			
		}
	}
	public void OnDeselectUnit(RTSGameObject gameObject) {
		//Debug.Log("OnDeselectUnit()");

		//Unit name
		unitName.GetComponent<Text>().text = "";

		//Unit pic
		unitPic.GetComponent<Image>().color = Color.white;

		//Unit action menu
		GameObject gridLayout = unitActionMenu.transform.GetChild(0).gameObject;
		int childrenCount = gridLayout.transform.childCount;
		for(int i = 0; i < childrenCount; ++i) {
			GameObject childGameObject = gridLayout.transform.GetChild(i).gameObject;
			Button childButton = childGameObject.GetComponent<Button>();
			Text childText = childGameObject.transform.GetChild(0).GetComponent<Text>();
			
			childButton.interactable = false;
			childText.text = "";
			childButton.onClick.RemoveAllListeners();
		}
	}

}

/*
public void OnRenderObject() {
	//GL.PushMatrix();
	// Set transformation matrix for drawing to
	// match our transform
		
	//GL.MultMatrix(transform.localToWorldMatrix);

	// Draw lines
	GL.Begin(GL.LINES);
	GL.Color(Color.black);

	// One vertex at transform position
	GL.Vertex3(0, 3, 0);
	// Another vertex at edge of circle
	GL.Vertex3(-50, 3, -50);

	GL.End();
	//GL.PopMatrix();
}
*/
