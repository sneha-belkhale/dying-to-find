using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Static Class to hand grab state
public class CCPlayer : MonoBehaviour
{
  public static CCPlayer localPlayer;
  public CCHand leftHand; 
  public CCHand rightHand;
  private void Awake() {
        if (localPlayer == null) {
            localPlayer = this;
        }
        else {
            Destroy(localPlayer);
            localPlayer = this;
        }
    }

    private void OnDestroy()
    {
        localPlayer = null;
    }
}