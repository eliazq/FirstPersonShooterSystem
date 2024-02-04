using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{

    private void Awake() {
        Player.Instance.transform.position = transform.position;
    }
}
