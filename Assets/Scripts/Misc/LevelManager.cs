using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public string SceneName;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void changeScene() {
        if (GameManager.Instance != null) {
            Destroy(GameObject.Find("GameManager"));
        }
        if (MemoryManager.Instance != null)
        {
            Destroy(GameObject.Find("MemoryManager"));
        }
        if (InstructionManager.Instance != null)
        {
            Destroy(GameObject.Find("InstructionManager"));
        }
        if (MicrocodeExecutor.Instance != null)
        {
            Destroy(GameObject.Find("MicrocodeExecutor"));
        }
        if (MicrocodeManager.Instance != null)
        {
            Destroy(GameObject.Find("MicrocodeManager"));
        }
        SceneManager.LoadScene(SceneName);
    }

    public void changeSceneStr(string str) {
        SceneManager.LoadScene(str);
    }
}
