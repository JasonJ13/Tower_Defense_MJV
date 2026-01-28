using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections;

public class App : MonoBehaviour
{
    private InputAction exitAction;
    private string firstscene = "MainMenu"; 
    public static App Instance
    {
        get;
        private set;
    }

    private void Awake()
    {
        if (App.Instance != null){
            Debug.LogError("Error : Instance of App already exists");
        }
        App.Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SceneManager.LoadScene(this.firstscene, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(this.firstscene));
//        this.exitAction = InputSystem.actions.FindAction("Exit");
        // Give controls back to the player
//        this.exitAction.Enable();
//        InputSystem.actions.FindActionMap("Player").Enable();
    }

    // Update is called once per frame
    void Update()
    {
//        if (this.exitAction.activeValueType != null){
//            QuitGame();
//        }
    }

    private IEnumerator UnloadAndLoad(string[] toUnload, string[] toLoad)
    {
        Debug.Log("Affiche Loading Screen");

        foreach (string nameScene1 in toUnload)
        {
            AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(nameScene1);
            // Wait until the asynchronous scene fully unloads
            int frame = 0;
            while (!asyncUnload.isDone)
            {
                frame++;
                if (frame > 1000)
                {
                    Debug.LogError("Scene couldn't unload : " + nameScene1);
                    yield break;
                }
                yield return null; // yield and return one frame later
            }            
        }

        foreach (string nameScene2 in toLoad)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nameScene2, LoadSceneMode.Additive);
            // Wait until the asynchronous scene fully unloads
            int frame = 0;
            while (!asyncLoad.isDone)
            {
                frame++;
                if (frame > 1000)
                {
                    Debug.LogError("Scene couldn't load : " + nameScene2);
                    yield break;
                }
                yield return null; // yield and return one frame later
            }
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(nameScene2));            
        }


        Debug.Log("Désaffiche Loading Screen");

        // Give controls back to the player
        this.exitAction.Enable();
        InputSystem.actions.FindActionMap("Player").Enable();
//        InputSystem.actions.FindActionMap("Player").Enable();
    }

    public void StartGame()
    {
        string[] toUnload = {"MainMenu"};
        string[] toLoad = {"MapSelectionScreen"};
        this.StartCoroutine(UnloadAndLoad(toUnload, toLoad));

    }

    public void MapSelected()
    {
        string[] toUnload = {"MapSelectionScreen"};
        string[] toLoad = {"Main"};
        this.StartCoroutine(UnloadAndLoad(toUnload, toLoad));        
    }

    public void QuitMapSelection()
    {
        string[] toUnload = {"MapSelectionScreen"};
        string[] toLoad = {"MainMenu"};
        this.StartCoroutine(UnloadAndLoad(toUnload, toLoad));

    }

    public void QuitGame()
    {
        string[] toUnload = {"Game"};
        string[] toLoad = {"MainMenu"};
        this.StartCoroutine(UnloadAndLoad(toUnload, toLoad));        
    }

    public void StartOption()
    {
        string[] toUnload = {"MainMenu"};
        string[] toLoad = {"Options"};
        this.StartCoroutine(UnloadAndLoad(toUnload, toLoad));        
    }

    public void QuitOption()
    {
        string[] toUnload = {"Options"};
        string[] toLoad = {"MainMenu"};
        this.StartCoroutine(UnloadAndLoad(toUnload, toLoad));        
    }


}
