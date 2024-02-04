using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyObjectOnLoad : MonoBehaviour
{
    private void Start() {
        DontDestroyOnLoad(this.gameObject);
    }
}
