using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour {

    public InputField ParticipantID;
    public Dropdown ExperimentType;
    public InputField ExperimenterInitials;
    public InputField Date;
    public GameObject FirstPanel;
    public GameObject SelectPanel;

    private void Start()
    {
        // Comment out later, for testing only
        if (!string.IsNullOrEmpty(PlayerPrefs.GetString("ParticipantID"))) {
            FirstPanel.SetActive(false);
            SelectPanel.SetActive(true);
        }
    }

    private void Update()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (Input.GetKey(KeyCode.Escape)) Application.Quit();
        if (Input.GetKey(KeyCode.E)) SceneManager.LoadScene("Menu");
    }

    public void ClickStart() {
        // check if button clicked:  Debug.Log("Start clicked!");
        // to get text field:  Debug.Log(ParticipantID.text);
        // to take screengrabs:  ScreenCapture.CaptureScreenshot(@"C:\Users\james\Desktop\ss.png");

        // Debug.Log(ExperimentType);
        // Debug.Log(ExperimentType.captionText);
        // Debug.Log(ExperimentType.captionText.text);

        PlayerPrefs.SetString("ParticipantID", ParticipantID.text);
        PlayerPrefs.SetString("ExperimentType", ExperimentType.captionText.text);
        PlayerPrefs.SetString("ExperimenterInitials", ExperimenterInitials.text);
        PlayerPrefs.SetString("Date", Date.text);

        string[] lines = {
            "Participant ID: " + ParticipantID.text,
            "Experiment Type: " + ExperimentType.captionText.text,
            "Experimenter Initials: " + ExperimenterInitials.text,
            "Date: " + Date.text
        };
        
        System.IO.File.WriteAllLines(@"C:\Users\james\Desktop\" + ExperimentType.captionText.text + "-" + ParticipantID.text + "-" + Date.text + ".txt", lines);

    }

    public void JoystickPractice() {
        PlayerPrefs.DeleteKey("Arrows");
        SceneManager.LoadScene("Joystick Practice");
    }

    public void VisuomotorExpertise()
    {
        PlayerPrefs.DeleteKey("Arrows");
        SceneManager.LoadScene("Visuomotor Expertise Maze");
    }

    public void TaskPracticeArrows()
    {
        PlayerPrefs.SetInt("Arrows", 1);
        SceneManager.LoadScene("Task Practice");
    }

    public void TaskPracticeNoArrows()
    {
        PlayerPrefs.SetInt("Arrows", 0);
        SceneManager.LoadScene("Task Practice");
    }

    public void TestTrialsMazeAFArrows()
    {
        PlayerPrefs.SetInt("Arrows", 1);
        PlayerPrefs.SetString("Direction", "F");
        SceneManager.LoadScene("Experimental Maze A");
    }

    public void TestTrialsMazeAFNoArrows() {
        PlayerPrefs.SetInt("Arrows", 0);
        PlayerPrefs.SetString("Direction", "F");
        SceneManager.LoadScene("Experimental Maze A");
    }

    public void TestTrialsMazeARNoArrows()
    {
        PlayerPrefs.SetInt("Arrows", 0);
        PlayerPrefs.SetString("Direction", "R");
        SceneManager.LoadScene("Experimental Maze A");
    }


    public void TestTrialsMazeBFArrows()
    {
        PlayerPrefs.SetInt("Arrows", 1);
        PlayerPrefs.SetString("Direction", "F");
        SceneManager.LoadScene("Experimental Maze B");
    }

    public void TestTrialsMazeBFNoArrows()
    {
        PlayerPrefs.SetInt("Arrows", 0);
        PlayerPrefs.SetString("Direction", "F");
        SceneManager.LoadScene("Experimental Maze B");
    }

    public void TestTrialsMazeBRNoArrows()
    {
        PlayerPrefs.SetInt("Arrows", 0);
        PlayerPrefs.SetString("Direction", "R");
        SceneManager.LoadScene("Experimental Maze B");
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.DeleteAll();
    }
}