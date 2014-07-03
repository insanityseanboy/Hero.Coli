using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CellControl : MonoBehaviour{

	public float baseMoveSpeed;
	public float rotationSpeed;
  public float relativeRotationSpeed;
	public List<Animation> anims;
  public Hero hero;
  public float moveEnergyCost;
  public float currentMoveSpeed;
  public string wallname;

  public CellControlButton absoluteWASDButton;
  public CellControlButton leftClickToMoveButton;
  public CellControlButton relativeWASDButton;
  public CellControlButton rightClickToMoveButton;
  public UISprite selectedControlTypeSprite;

  private bool _pause;
  private Vector3 _inputMovement;
  public Vector3 GetInputMovement() { return _inputMovement;}

  /* 
   * Click to move variables
   */
  private int _smooth; // Determines how quickly object moves towards position
  private float _hitdist = 0.0f;
  private Vector3 _targetPosition;
  private bool _isFirstUpdate = false;
    
  private enum ControlType {
      RightClickToMove = 0,
      LeftClickToMove = 1,
      AbsoluteWASDAndLeftClickToMove = 2,
      AbsoluteWASD = 3,
      RelativeWASD = 4
  };
  private ControlType _currentControlType = ControlType.AbsoluteWASDAndLeftClickToMove;

	public enum RockCollisionType {
		Slide = 2,
		Grab = 1,
		Normal = 0
	}
	//Drag PushableBox
	private RockCollisionType _currentCollisionType = 0;
	public RockCollisionType GetCurrentCollisionType() {return _currentCollisionType;}

	private float _angle =0f;		// the angle for the rotation around the pushable box
	private float _angleProgress = 0f;	// the current progression of the rotation
	private float _angleStep = 0f;
	private float _rotationSpeed = 2f;

	private bool _isDragging = false;
	public bool GetIsDragging() {return _isDragging;}
	public void SetIsDragging(bool b) { _isDragging =b;}

	GameObject box;
	public void SetBox(GameObject o) {box = o;}
	public GameObject GetBox(){return box;}

  public void Pause(bool pause)
  {
    _pause = pause;
  }

  public bool isPaused()
  {
    return _pause;
  }

  private void ClickToMoveUpdate(KeyCode mouseButtonCode) {
    Vector3 lastTickPosition = transform.position;
    if(Input.GetKeyDown(mouseButtonCode))            
    {
		    _smooth = 1;
			    
			Plane playerPlane = new Plane(Vector3.up, transform.position);            
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);            
			      
			if (playerPlane.Raycast (ray, out _hitdist) && !UICamera.hoveredObject) {                
				 _targetPosition = ray.GetPoint(_hitdist);
		}

			if(_isDragging)
			{
				box.GetComponent<PushableBox>().SetUsedClicked(true);
				box.GetComponent<PushableBox>().Clicked();

				if(_isDragging)
				{
					_angle = Vector3.Angle(transform.forward,_targetPosition-box.transform.position);
					if(_angle >=20f)
					{
						Vector3 relativepoint = transform.InverseTransformPoint(_targetPosition);

						_angleProgress = 0f;
						if (relativepoint.x < 0.0f)
						{
							_angle = -_angle;

							_angleStep = -_rotationSpeed;
						}
						else 
							_angleStep = _rotationSpeed;

						int i = 0 ;
						bool isFound = false;
						Vector3 inverseTargetPosition = box.transform.TransformPoint(box.transform.InverseTransformPoint(_targetPosition)*(-1f));


						RaycastHit[] hits;
						//hits = Physics.RaycastAll(inverseTargetPosition, box.transform.position);
						Ray ray2 = new Ray(inverseTargetPosition, _targetPosition);
						hits = Physics.RaycastAll(ray2);
						Debug.DrawLine(inverseTargetPosition, _targetPosition, Color.red, 1000f);
						Logger.Log("HitCount::"+hits.Length,Logger.Level.WARN);
						while (i < hits.Length && !isFound)
						{
							Logger.Log("looking for drawline::"+hits[i].transform,Logger.Level.WARN);
							//if the pushableBox have been clicked
							if(hits[i].collider == box.collider)
							{
								Logger.Log ("drawLine::"+box.transform.InverseTransformPoint(hits[i].point),Logger.Level.WARN);
								Debug.DrawLine(box.transform.localPosition,hits[i].point,Color.red,1000f);
								isFound = true;
							}
							i++;
						}
					}
				}


			}

    }

    if(Vector3.zero == _inputMovement)
    {
      Vector3 aim = _targetPosition - transform.position;    
      _inputMovement = new Vector3(aim.x, 0, aim.z);

      if(_inputMovement.sqrMagnitude <= 5f) {
        stopMovement();
      } else {
        _inputMovement = _inputMovement.normalized;
      }
	  if(_isDragging)
	    DraggingUpdate();
      rotationUpdate();
    }
  }
    
  private void AbsoluteWASDUpdate() {
    if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) {

      cancelMouseMove();

      //Translate
      _inputMovement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
      if(_inputMovement.sqrMagnitude > 1) _inputMovement /= Mathf.Sqrt(2);

      rotationUpdate();

    } else if (Vector3.zero != _inputMovement) {
      stopMovement();
    }
  }

  private void RelativeWASDUpdate() {

    if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) {

      cancelMouseMove();

      float norm = Input.GetAxis("Vertical");
      if(norm < 0) norm = 0;
      
      float deltaAngle = Input.GetAxis("Horizontal");

      transform.RotateAround(transform.position, Vector3.up, deltaAngle * relativeRotationSpeed);

      float angle = transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
      _inputMovement = new Vector3(Mathf.Sin(angle), 0f, Mathf.Cos(angle)) * norm;
    }
  }

  public void reset() {
    stopMovement();
  }

  private void cancelMouseMove() {
    _targetPosition = transform.position;
  }

  private void stopMovement() {        
    _inputMovement = Vector3.zero;
    cancelMouseMove();
    setSpeed();
  }

  private void rotationUpdate() {
    if(Vector3.zero != _inputMovement) {
      //Rotation
      float rotation = Mathf.Atan2(_inputMovement.x, _inputMovement.z) * Mathf.Rad2Deg;
      transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.AngleAxis(rotation, Vector3.up), Time.deltaTime * rotationSpeed);
    }
  }

	private void DraggingMove(float angle) {
		if(_isDragging)
		{
			Vector3 moveAmount = new Vector3(0,0,-150);
			//this.collider.attachedRigidbody.AddForce(moveAmount);
			transform.RotateAround(box.transform.position,new Vector3(0,1,0),angle);

		}
	}

	private void DraggingUpdate() {

		if (Mathf.Abs(_angle) > 20 && Mathf.Abs(_angle)< 50)
		{
			if(Mathf.Abs(_angleProgress) < Mathf.Abs (0.8f*_angle))
					_angleProgress += _angleStep;

			else _angleProgress += 0.5f*_angleStep;

			if(Mathf.Abs(_angleProgress) >= Mathf.Abs (_angle))
			{
				_angle = 0.0f;
				_angleProgress =0.0f;
				_angleStep = 0.0f;
			}
				
			else 
			{
				//Logger.Log(_angleProgress+"° of ::"+_angle,Logger.Level.WARN);
				DraggingMove(_angleStep);
			}
		}

	}

  private void commonUpdate() {
    if(Vector3.zero != _inputMovement) {

      Vector3 moveAmount = _inputMovement * currentMoveSpeed;
      
      this.collider.attachedRigidbody.AddForce(moveAmount);
      
      updateEnergy(moveAmount);

      setSpeed();
    }
  }

  private void setSpeed() {   
    //SetSpeed
    float speed = _inputMovement.sqrMagnitude + 0.3f;
    Animation[] anims = GetComponentsInChildren<Animation>();
    foreach(Animation anim in anims) {
      foreach (AnimationState state in anim) {
        state.speed = speed;
      }
    }
  }

  private void updateEnergy(Vector3 moveAmount) {
    float cost = moveAmount.sqrMagnitude*moveEnergyCost;
    hero.subEnergy(cost);
  }
	
	void Start (){
    gameObject.GetComponent<PhenoSpeed>().setBaseSpeed(baseMoveSpeed);

    _targetPosition = transform.position;

   /* absoluteWASDButton = GameObject.Find("AbsoluteWASDButton").GetComponent<AbsoluteWASDButton>();
    leftClickToMoveButton = GameObject.Find("LeftClickToMoveButton").GetComponent<LeftClickToMoveButton>();
    relativeWASDButton = GameObject.Find("RelativeWASDButton").GetComponent<RelativeWASDButton>();
    rightClickToMoveButton = GameObject.Find("RightClickToMoveButton").GetComponent<RightClickToMoveButton>();
    selectedControlTypeSprite = GameObject.Find ("SelectedControlTypeSprite").GetComponent<UISprite>();*/

//    absoluteWASDButton.cellControl = this;
//    leftClickToMoveButton.cellControl = this;
//    relativeWASDButton.cellControl = this;
//    rightClickToMoveButton.cellControl = this;

    switchControlTypeToAbsoluteWASDAndLeftClickToMove();
	}
  
	void Update(){
		//Keyboard controls
		if(!_pause) {

      _inputMovement = Vector3.zero;

      switch(_currentControlType) {
        case ControlType.AbsoluteWASDAndLeftClickToMove:  
            AbsoluteWASDUpdate();
            if(!_isFirstUpdate) {
                ClickToMoveUpdate(KeyCode.Mouse0);
            } else { _isFirstUpdate = false; }
            break;
        case ControlType.LeftClickToMove:      
          if(!_isFirstUpdate) {
              ClickToMoveUpdate(KeyCode.Mouse0);
          } else { _isFirstUpdate = false; }
          break;
        case ControlType.RightClickToMove:
          ClickToMoveUpdate(KeyCode.Mouse1);
          break;
        case ControlType.AbsoluteWASD:
          AbsoluteWASDUpdate();
          break;
        case ControlType.RelativeWASD:
          RelativeWASDUpdate();
          break;
        default:
          AbsoluteWASDUpdate();
          break;
      }
      commonUpdate();
    }
    
		switchRockCollision();
    //if(Input.GetKeyDown(KeyCode.Space)) {
    //  switchControlTypeTo((ControlType)(((int)_currentControlType + 1) % 5));
    //}


    
  }


	private void switchRockCollision () {

		if(Input.GetKeyDown(KeyCode.Space))
		   {
			if (_currentCollisionType == RockCollisionType.Slide)
				_currentCollisionType -=2;
			else
				_currentCollisionType +=1;
			Logger.Log ("collisionType::"+_currentCollisionType,Logger.Level.WARN);

			}
		}

  private void switchControlTypeTo(ControlType newControlType) {
    switch(newControlType) {
      case ControlType.AbsoluteWASDAndLeftClickToMove:
        switchControlTypeToAbsoluteWASDAndLeftClickToMove();
        break;
      case ControlType.AbsoluteWASD:
        switchControlTypeToAbsoluteWASD();
        break;
      case ControlType.LeftClickToMove:
        switchControlTypeToLeftClickToMove();
            break;
      case ControlType.RelativeWASD:
        switchControlTypeToRelativeWASD();
        break;
      case ControlType.RightClickToMove:
        switchControlTypeToRightClickToMove();
        break;
    }
  }
    
  public void switchControlTypeToAbsoluteWASDAndLeftClickToMove() {
    switchControlTypeTo(ControlType.AbsoluteWASDAndLeftClickToMove, absoluteWASDButton.transform.position);
  }  
  public void switchControlTypeToRightClickToMove() {
    switchControlTypeTo(ControlType.RightClickToMove, rightClickToMoveButton.transform.position);
  }

  public void switchControlTypeToLeftClickToMove() {
    switchControlTypeTo(ControlType.LeftClickToMove, leftClickToMoveButton.transform.position);
  }

  public void switchControlTypeToAbsoluteWASD() {
    switchControlTypeTo(ControlType.AbsoluteWASD, absoluteWASDButton.transform.position);
  }

  public void switchControlTypeToRelativeWASD() {
    switchControlTypeTo(ControlType.RelativeWASD, relativeWASDButton.transform.position);
  }
  
  private void switchControlTypeTo(ControlType newControlType, Vector3 position) {
    Logger.Log("CellControl::switchControlTypeTo("+newControlType+") with old="+_currentControlType, Logger.Level.DEBUG);

    if(ControlType.LeftClickToMove == newControlType) {
      _isFirstUpdate = true;
    }

    selectedControlTypeSprite.transform.position = position;
    _targetPosition = transform.position;
    _currentControlType = newControlType;
  }

  void OnCollisionStay(Collision col) {
    if ((Vector3.zero != _inputMovement) && col.collider && (wallname == col.collider.name)){
      stopMovement();
    }
  }
}