﻿using UnityEngine;
using System.Collections;

public class AbsoluteWASDMainMenuItem : ControlMainMenuItem {
    public override void click() {
        // Debug.Log(this.GetType());
		base.click();
        controlsArray.switchControlTypeToAbsoluteWASD();
    }
}
