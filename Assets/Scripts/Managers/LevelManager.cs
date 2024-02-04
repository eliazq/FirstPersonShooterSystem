using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    
    [SerializeField] private SceneReference[] levels;
   
    private void Start(){
        if (Instance == null)
            Instance = this;
        else if(Instance != this)
            Destroy(this.gameObject);
        
        DontDestroyOnLoad(this.gameObject);
    }

    public void LoadCurrentLevel(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void LoadNextLevel(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
