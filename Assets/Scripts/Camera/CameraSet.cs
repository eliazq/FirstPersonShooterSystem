using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSet : MonoBehaviour
{
    
    private void Start() {
        DontDestroyOnLoad(this.gameObject);
    }
}
