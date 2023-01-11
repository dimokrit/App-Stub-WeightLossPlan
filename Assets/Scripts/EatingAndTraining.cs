using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EatingAndTraining : MonoBehaviour
{
    public Text programText;
    public Dropdown daysDropdown;
    public TextAsset eatingProgramFile;
    public string[] oneDayProgram;

    public void Start()
    {
        oneDayProgram = eatingProgramFile.text.Split('?');
    }

    public void OnClickBackButton()
    {
        SceneManager.LoadScene("Scenes/StubScene");
    }

    public void OnChangeDayValue()
    {
        var day = daysDropdown.value;
        programText.text = oneDayProgram[day];
    }

}
