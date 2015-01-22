﻿using UnityEngine;
using System.Collections;

public class ScaleLabelLengthMatcher : MonoBehaviour {

  private int _padding = 10;
  private UILabel _label;
  private UISprite _bg;
  private BoxCollider _collider;
        
  public float factor;
  public float offset;
    
    // Use this for initialization
  void Start () {        
    _label = gameObject.GetComponentInChildren<UILabel>();        
    _bg = gameObject.GetComponentInChildren<UISprite>();
    _collider = gameObject.GetComponent<BoxCollider>();	
	}
	
	// Update is called once per frame
	void Update () {
    _bg.transform.localScale = new Vector3(
            _label.relativeSize.x*_label.transform.localScale.x+_padding, 
            _bg.transform.localScale.y, 
            _bg.transform.localScale.z);   

    _collider.size = _bg.transform.localScale;
    _collider.center = new Vector3(
            _collider.size.x*factor+offset+Mathf.Sign(offset)*_label.transform.localScale.x, 
            _collider.center.y,
            _collider.center.z);
  }
}
