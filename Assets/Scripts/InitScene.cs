using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitScene : MonoBehaviour
{

    private void LateUpdate(){
        if (LevelManager.Instance != null){
            LevelManager.Instance.LoadNextLevel();
        }
    }
}
