using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class CursorManager : MonoBehaviour {
	
	//Singleton
	public static CursorManager main;

	public List<Cursor> cursors;

	public CursorID currentCursorID = CursorID.Normal;
	private Cursor currentCursor;

	private float cursorSize = 20.0f;
	private bool showCursor = true;


	void Awake() {
        if(main == null) main = this;
		//UnityEngine.Cursor.visible = false;
	}

	void Start() {
		cursors = new List<Cursor>(this.GetComponents<Cursor>());

		foreach(Cursor cur in cursors) {
			if(cur.cursorID == CursorID.Normal) {
				currentCursor = cur;
			}
		}
	}

	void Update() {
		foreach(Cursor cur in cursors) {
			if(cur.cursorID == currentCursorID) {
				currentCursor = cur;
			}
		}

		if(currentCursor.getAnimate()) {
			currentCursor.Animate();
		}
	}
	
	void OnGUI() {
		if(showCursor) {
			GUI.depth = -2;

			//Draw Cursor
			float offset = 0f;
			
			if(currentCursor.centerTexture) {
				offset = cursorSize;
			} else {
				offset = 0;
			}
			
			GUI.DrawTexture(new Rect(Input.mousePosition.x-(offset/2), Screen.height-Input.mousePosition.y-(offset/2), cursorSize, cursorSize), currentCursor.getCursorPicture());
		}
	}

	
	public void ChangeCursor(CursorID cursorID) {
		currentCursorID = cursorID;
	}

	public void ShowCursor() {
		showCursor = true;
	}

	public void HideCursor() {
		showCursor = false;
	}
	
}
