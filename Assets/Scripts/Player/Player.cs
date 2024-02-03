using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;
    public WeaponHandling WeaponHandling{get; private set;}

    private void Start() {
        if (Instance == null){
            Instance = this;
        }
        else if(Instance != this){
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }

    private void Awake(){
        WeaponHandling = GetComponent<WeaponHandling>();
    }
}
