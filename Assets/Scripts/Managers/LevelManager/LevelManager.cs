using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private static LevelManager _instance;
    private static readonly object _lock = new object();

    Scene previousScene;

    public static LevelManager Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        LevelManager prefab = Resources.Load<LevelManager>("LevelManager");

                        if (prefab == null)
                        {
                            Debug.LogError("LevelManager prefab not found in Resources!", _instance.gameObject); // Assets/Resources/ Empty prefab with name "LevelManager"
                            Debug.Log("Create Empty GameObject Prefab With SoundManager Script Attached. In Folder Assets/Resources/");
                        }
                        else
                        {
                            _instance = Instantiate(prefab);
                            DontDestroyOnLoad(_instance.gameObject);
                        }
                    }
                }
            }
            return _instance;
        }
    }

    public enum Level
    {
        MainMenu,
        Preview,
        LosSantos,
        // Add more levels here
    }

    public LevelData levelData;

    private static Dictionary<Level, string> levelNameDictionary;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Initialize()
    {
        levelNameDictionary = new Dictionary<Level, string>();
        foreach (var levelInfo in levelData.levels)
        {
            levelNameDictionary[levelInfo.level] = levelInfo.sceneName;
        }
    }

    public static void LoadLevel(Level level)
    {
        if (levelNameDictionary.TryGetValue(level, out string levelName))
        {
            Debug.Log($"Loading level: {levelName}");
            Instance.previousScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(levelName);
        }
        else
        {
            Debug.LogError($"Level {level} not found in dictionary");
        }
    }

    public static void LoadLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public static void LoadCurrentLevel()
    {
        Instance.previousScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public static bool LoadPreviousLevel()
    {
        if (Instance.previousScene != null)
        {
            SceneManager.LoadScene(Instance.previousScene.buildIndex);
            return true;
        }
        return false;
    }
}
