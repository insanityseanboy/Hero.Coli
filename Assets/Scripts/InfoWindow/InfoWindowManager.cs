using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InfoWindowManager : MonoBehaviour {

  //////////////////////////////// singleton fields & methods ////////////////////////////////
  public static string gameObjectName = "InfoWindowManager";
  private static InfoWindowManager _instance;
  public static InfoWindowManager get() {
    if(_instance == null) {
      Logger.Log("InfoWindowManager::get was badly initialized", Logger.Level.WARN);
      _instance = GameObject.Find(gameObjectName).GetComponent<InfoWindowManager>();
    }
    return _instance;
  }
  void Awake()
  {
    Logger.Log("InfoWindowManager::Awake", Logger.Level.DEBUG);
    _instance = this;
  }
  ////////////////////////////////////////////////////////////////////////////////////////////

  string title;
  string subtitle;
  string texture;
  string explanation;
  string bottom;
  //TODO onNext;

  public UILabel titleLabel;
  public UILabel subtitleLabel;
  public UILabel explanationLabel;
  public UILabel bottomLabel;

  // TODO manage onNext

  public GameObject infoPanel;
  public UISprite infoSprite;

  public GameStateController gameStateController;

  private Dictionary<string, StandardInfoWindowInfo> _loadedInfoWindows = new Dictionary<string, StandardInfoWindowInfo>();

  private static StandardInfoWindowInfo retrieveFromDico(string code)
  {
    Logger.Log("InfoWindowManager::retrieveFromDico("+code+") starts", Logger.Level.TEMP);
    return new StandardInfoWindowInfo("title", "subtitle", "tuto_intro-01", "explanation", "bottom");
  }

  public static void displayInfoWindow()
  {
    Logger.Log("InfoWindowManager::displayInfoWindow starts", Logger.Level.TEMP);
    fillInFieldsFromCode("any_string");
    _instance.infoPanel.SetActive(true);
    _instance.gameStateController.StateChange(GameState.Pause);
    _instance.gameStateController.dePauseForbidden = true;
  }

  private static void fillInFieldsFromCode(string code)
  {
    Logger.Log("InfoWindowManager::fillInFieldsFromCode("+code+") starts", Logger.Level.TEMP);
    getStuffFromCode(code);

    _instance.titleLabel.text       = _instance.title;
    _instance.subtitleLabel.text    = _instance.subtitle;
    _instance.infoSprite.spriteName = _instance.texture;
    _instance.explanationLabel.text = _instance.explanation;
    _instance.bottomLabel.text      = _instance.bottom;

    // TODO manage onNext
  }

  private static bool getStuffFromCode(string code)
  {
    Logger.Log("InfoWindowManager::getStuffFromCode("+code+") starts", Logger.Level.TEMP);

    StandardInfoWindowInfo info = retrieveFromDico(code);

    if(null != info)
    {
      _instance.title       = info._title;
      _instance.subtitle    = info._subtitle;
      _instance.texture     = info._texture;
      _instance.explanation = info._explanation;
      _instance.bottom      = info._bottom;

      return true;
    }
    else
    {
      return false;
    }

    // TODO manage onNext
  }
}

