using UnityEngine;
using System.Collections;

public class EquipCraftButton : MonoBehaviour {

  public GUITransitioner transitioner;

 // Use this for initialization
 void Start () {
 
 }
 
 // Update is called once per frame
 void Update () {
 
 }

  private void OnPress(bool isPressed) {
    if(isPressed) {
      Logger.Log("EquipCraftButton::OnPress()", Logger.Level.INFO);
      transitioner.GoToScreen(GUITransitioner.GameScreen.screen3);
    }
  }
}

