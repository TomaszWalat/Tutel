using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
    // Cross fade
    [SerializeField]
    Animator transition;
    [SerializeField]
    float transitionTime = 1.0f;

    // A set of currently loaded scene names
    [SerializeField]
    HashSet<string> loadedScenes;

    // A stack to keep a breadcrumb trail for "back" buttons
    [SerializeField]
    Stack<string> sceneStack;

    [SerializeField]
    Scene currentScene;

    [SerializeField]
    ScreenManagerScript currentScreenManager;

    [SerializeField]
    bool isLoadingScene;

    [SerializeField]
    string partloadedSceneName;
    [SerializeField]
    AsyncOperation partloadedScene;

    [SerializeField]
    bool allDataLoaded;

    // Deserialisation example flags
    [SerializeField]
    bool test1DataLoaded;
    [SerializeField]
    bool test2DataLoaded;
    [SerializeField]
    bool test3DataLoaded;


    void Awake()
    {
        // Find any all objects tagged with GameManager
        GameObject[] objects = GameObject.FindGameObjectsWithTag("GameManager");

        if (objects.Length > 1)
        {
            // If one already exists, destroy self (as you're the duplicate)
            Destroy(this.gameObject);
        }

        // Set object to be persisten between scenes
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        isLoadingScene = false;
        partloadedSceneName = "";
        partloadedScene = null;

        sceneStack = new Stack<string>();

        loadedScenes = new HashSet<string>();

        currentScene = SceneManager.GetActiveScene();

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        SceneManager.activeSceneChanged += OnActiveSceneChanged;

        // --- For testing only --- //
        allDataLoaded = false;
        test1DataLoaded = false;
        test2DataLoaded = false;
        test3DataLoaded = false;

        // --- For testing only --- //
        StartCoroutine(LoadTest1Data());
        StartCoroutine(LoadTest2Data());
        StartCoroutine(LoadTest3Data());
        //PreloadScene("_unload");

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    // ----- Data loading (deserialisation) example ----- //
    private IEnumerator LoadTest1Data()
    {
        yield return new WaitForEndOfFrame(); // Wait until the end of the frame to start

        yield return new WaitForSeconds(0.5f);

        test1DataLoaded = true;

        StartCoroutine(CheckDataLoadingProgress());
    }

    private IEnumerator LoadTest2Data()
    {
        yield return new WaitForEndOfFrame(); // Wait until the end of the frame to start

        yield return new WaitForSeconds(1.5f);

        test2DataLoaded = true;

        StartCoroutine(CheckDataLoadingProgress());
    }

    private IEnumerator LoadTest3Data()
    {
        yield return new WaitForEndOfFrame(); // Wait until the end of the frame to start

        yield return new WaitForSeconds(3.5f);

        test3DataLoaded = true;

        StartCoroutine(CheckDataLoadingProgress());
    }

    private IEnumerator CheckDataLoadingProgress()
    {
        yield return new WaitForEndOfFrame(); // Wait until the end of the frame to start

        allDataLoaded = test1DataLoaded && test2DataLoaded && test3DataLoaded;

        //if (allDataLoaded)
        //{
        //    TryLoadScene("MainMenuScene");
        //}
    }
    // ----- End of desrialisation example ----- //


    // --- For button OnClick() functionallity only --- //
    // (it cannot use mathods with return values)
    public void GoToScene(string sceneName)
    {
        TryLoadScene(sceneName);
    }
    public void GoBack()
    {
        if (sceneStack.Count > 0)
        {
            TryLoadScene(sceneStack.Peek());
        }
    }
    public void ExitPreloadScene()
    {
        if (allDataLoaded)
        {
            TryLoadScene("MainMenuScene");
        }
    }
    public void QuitGame()
    {
        TryLoadScene("_unload");
    }
    // --- End of button OnClick() functionality --- //

    private bool TryLoadScene(string sceneName)
    {
        bool success = false;

        if(!isLoadingScene)
        {
            if(!(loadedScenes.Contains(sceneName)))
            {
                if (sceneName != currentScene.name)
                {
                    isLoadingScene = true;

                    StartCoroutine(ChangeToScene(sceneName));

                    success = true;
                }
            }
        }

        return success;
    }

    // Changes what scene is active
    IEnumerator ChangeToScene(string sceneName)
    {
        StartCoroutine(LoadSceneInBackground(sceneName));

        yield return new WaitForEndOfFrame();

        while(partloadedScene.progress < 0.9f)
        {
            yield return new WaitForFixedUpdate();
        }
        
        Scene targetScene = SceneManager.GetSceneByName(sceneName);

        if (targetScene.IsValid())
        {
            Debug.LogFormat("Scene is valid - {0}", sceneName);


            //yield return 0; 
            //// --- source: https://forum.unity.com/threads/loading-scene-additively-causes-change-in-lighting.511566/ --- //

            transition.SetTrigger("Fade_Out"); // Fade to black

            yield return new WaitForSeconds(transitionTime);

            partloadedScene.allowSceneActivation = true;

            while(!partloadedScene.isDone)
            {
                yield return new WaitForEndOfFrame();
            }

            partloadedScene = null;
            partloadedSceneName = "";

            yield return new WaitForEndOfFrame(); // wait a frame to make sure ambient lighting has a chance to switch over

            SceneManager.SetActiveScene(targetScene);

            currentScene = targetScene;

            yield return new WaitForSeconds(transitionTime);

            transition.SetTrigger("Fade_In"); // Fade from black

            isLoadingScene = false;
            // Can execute more here if anything needs to be done after changing active scene
        }

    }

    // Load scene in the background, adding it to Unity's SceneManager's pool of loaded scenes
    IEnumerator LoadSceneInBackground(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        partloadedScene = asyncLoad;
        partloadedSceneName = sceneName;

        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.progress < 0.9f)
        {
            Debug.LogFormat("Loading scene progress: {0:F}", asyncLoad.progress);
            //yield return 0;//new WaitForEndOfFrame();
        }

        Debug.LogFormat("Finished loading scene: {0}", sceneName);

        loadedScenes.Add(sceneName);

        yield return new WaitForEndOfFrame();

        // Can execute more here if anything needs to be done after loading in a scene
    }


    IEnumerator UnloadLoadSceneInBackground(string sceneName)
    {
        AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(sceneName, UnloadSceneOptions.None);

        while (!asyncUnload.isDone)
        {

            yield return new WaitForEndOfFrame();
        }

        // Can execute more after scene is unloaded if needed
    }

    IEnumerator FindScreenManager()
    {

        while(SceneManager.sceneCount > 1)
        {
            yield return new WaitForEndOfFrame();
        }

        GameObject screenManagerObject = GameObject.Find("ScreenManager");

        if(screenManagerObject != null)
        {
            if(screenManagerObject.TryGetComponent(out ScreenManagerScript smScript))
            {
                currentScreenManager = smScript;

                currentScreenManager.AttachGameManager(this);
            }
        }
    }

    private void ExitApplication()
    {

    }

    // --------------- Event triggers --------------- //
    private void OnActiveSceneChanged(Scene current, Scene next)
    {
        Debug.LogFormat("Active scene change: current ({0}) -> next ({1})", current.name, next.name);

        if(sceneStack.Count > 0)
        { 
            if(sceneStack.Peek() == next.name)  // If going back
            {
                sceneStack.Pop();
            }
        }
        else                               
        {
            sceneStack.Push(current.name);
        }

        StartCoroutine(UnloadLoadSceneInBackground(current.name));
        StartCoroutine(FindScreenManager());
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.LogFormat("New scene has been loaded: {0} - in mode: {1}", scene.name, mode.ToString());

        if(scene.name == "MainMenuScene")
        {
            sceneStack.Clear();
        }
    }

    private void OnSceneUnloaded(Scene scene)
    {

        Debug.LogFormat("Scene had been unloaded:{0}", scene.name);


        loadedScenes.Remove(scene.name);
    }



}