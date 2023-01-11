using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase;

public class StubMenu : MonoBehaviour
{
    public static int a;
    void start()
    {
       // FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
       //     FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
       // });
    }

    public void OnClickEatingButton()
    {
        SceneManager.LoadScene("Scenes/EatingScene");
    }

    public void OnClickTrainingButton()
    {
        SceneManager.LoadScene("Scenes/TrainingScene");
    }
}