using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCanvas : MonoBehaviour
{
    
    private void Start() {
        DontDestroyOnLoad(this.gameObject);
    }
}
