using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FinalizationInfoPanelManager : MonoBehaviour {

  //TODO refactor with LastHoveredInfoManager
  private string                  _lengthPrefix = "Length: ";
  private string                  _lengthPostfix = " bp";
  private const string            _defaultInfo = "";
  private const string            _defaultName = "no name";
  private const string            _defaultStatus = "invalid device!";

  public UISprite   finalizationIconSprite;
  public UILabel    finalizationInfoLabel;
  public UILabel    finalizationNameLabel;
  public UILabel    finalizationStatusLabel;

  private Device    _device;

  public void setDisplayedDevice(
    Device device
    , string status = _defaultStatus
    , string infos = _defaultInfo
    , string name = _defaultName
    ){
    Logger.Log("FinalizationInfoPanelManager::setDisplayedDevice("+device+") with _device="+_device, Logger.Level.WARN);

    _device = device;
    string displayedInfo = _device!=null?_lengthPrefix+_device.getSize().ToString()+_lengthPostfix:infos;
    finalizationInfoLabel.text = displayedInfo;

    string displayedName = _device!=null?_device.getName():name;
    finalizationNameLabel.text = displayedName;

    finalizationStatusLabel.text = status;
  }

  void Start() {
    setDisplayedDevice(null, "status");
  }
}

