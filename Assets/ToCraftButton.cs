﻿using UnityEngine;
using System.Collections;

public class ToCraftButton : MonoBehaviour {

private void OnPress(bool isPressed) {
	    if(isPressed) {
	      Logger.Log("ToCraftButton::OnPress()", Logger.Level.INFO);
	      GUITransitioner.get().GoToScreen(GUITransitioner.GameScreen.screen3);
	    }
	  }
}