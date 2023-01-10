using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StubMenu : MonoBehaviour
{
    public void OnClickEatingButton()
    {
        SceneManager.LoadScene("Scenes/EatingScene");
    }

    public void OnClickTrainingButton()
    {
        SceneManager.LoadScene("Scenes/TrainingScene");
    }
}