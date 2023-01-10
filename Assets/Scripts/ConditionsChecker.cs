namespace Firebase.Sample.RemoteConfig {
  using Firebase.Extensions;
  using System;
  using System.Threading.Tasks;
  using UnityEngine;
  using UnityEngine.SceneManagement;
 

  public
  class ConditionsChecker : MonoBehaviour {
    private string logText = "";
    const int kMaxLogSize = 16382;
    public string link;
    Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;
    protected bool isFirebaseInitialized = false;

    protected virtual void Start() {
      if (PlayerPrefs.HasKey("SavedString")) {
        link = PlayerPrefs.GetString("SavedString");
        SceneManager.LoadScene("Scenes/WebViewScene");
      } else {
          Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
              InitializeFirebase();
            else
              Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
          });
      }
    }


    void InitializeFirebase() {
        System.Collections.Generic.Dictionary<string, object> defaults = new System.Collections.Generic.Dictionary<string, object>();
        Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults).ContinueWithOnMainThread(task => {
            DebugLog("RemoteConfig configured and ready!");
            isFirebaseInitialized = true;
        });
        FetchDataAsync();
    }

    public void DisplayAllKeys() {
        DebugLog("Current Keys:");
        System.Collections.Generic.IEnumerable<string> keys = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.Keys;
        foreach (string key in keys) {
            DebugLog("    " + key);
        }
        DebugLog("GetKeysByPrefix(\"config_test_s\"):");
        keys = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetKeysByPrefix("config_test_s");
        foreach (string key in keys) {
            DebugLog("    " + key);
        }
    }

    public Task FetchDataAsync() {
      DebugLog("Fetching data...");
      System.Threading.Tasks.Task fetchTask = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero);
      return fetchTask.ContinueWithOnMainThread(FetchComplete);
    }

    void FetchComplete(Task fetchTask) {
      if (fetchTask.IsCanceled) {
        DebugLog("Fetch canceled.");
      } else if (fetchTask.IsFaulted) {
        DebugLog("Fetch encountered an error.");
      } else if (fetchTask.IsCompleted) {
        DebugLog("Fetch completed successfully!");
      }

        var info = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.Info;
        switch (info.LastFetchStatus) {
            case Firebase.RemoteConfig.LastFetchStatus.Success:
            Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.ActivateAsync().ContinueWithOnMainThread(task => {
              DebugLog(String.Format("Remote data loaded and ready (last fetch time {0}).", info.FetchTime));
              Show();
            });

            break;
            case Firebase.RemoteConfig.LastFetchStatus.Failure:
            switch (info.LastFetchFailureReason) {
                case Firebase.RemoteConfig.FetchFailureReason.Error:
                DebugLog("Fetch failed for unknown reason");
                break;
                case Firebase.RemoteConfig.FetchFailureReason.Throttled:
                DebugLog("Fetch throttled until " + info.ThrottledEndTime);
                break;
            }
            break;
            case Firebase.RemoteConfig.LastFetchStatus.Pending:
            DebugLog("Latest Fetch call still pending.");
            break;
        }
    }

    public void Show() {
      link = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("link").StringValue;
      DebugLog(link);
      if (link == "") {
        SceneManager.LoadScene("Scenes/StubScene");
      } else {
        PlayerPrefs.SetString("SavedString", link);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Scenes/WebViewScene");
      }
    }

    public void DebugLog(string s) {
        print(s);
        logText += s + "\n";

      while (logText.Length > kMaxLogSize) {
        int index = logText.IndexOf("\n");
        logText = logText.Substring(index + 1);
      }
    }
  }
}