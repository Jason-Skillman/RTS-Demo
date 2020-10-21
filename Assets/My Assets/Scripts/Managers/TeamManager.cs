using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TeamManager : MonoBehaviour {

	//Singleton
	public static TeamManager main;

	public List<TeamController> allTeams = new List<TeamController>();
	public TeamController player1;


	public void Awake() {
        if(main == null) main = this;
	}

	public void AddTeam(TeamController teamPlayer) {
		allTeams.Add(teamPlayer);
		
		if(teamPlayer.control == PlayerControl.Player) {
			player1 = teamPlayer;
		}
		
	}

}
