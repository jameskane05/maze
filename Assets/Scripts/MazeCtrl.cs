using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MazeCtrl : MonoBehaviour {

    public Canvas canvas;
    public Text text;

    static public float totalDistance;
    static public float totalTime;
    public GameObject player;
    public GameObject arrows;
    public Transform startF;
    public Transform startR;

    private float avgVelocity;
    private Vector3 lastPos;
    private Vector3 currentPos;
    private bool hasStarted;
    static public bool hasEnded;
    static private Scene thisScene;
    

    // Use this for initialization
    void Start() {

        // Check global settings:
        // string text = PlayerPrefs.GetString("ExperimenterInitials");
        Debug.Log(PlayerPrefs.GetInt("Arrows"));


        if (PlayerPrefs.GetInt("Arrows") == 1)
        {
            arrows.SetActive(true);
        }
        else if (PlayerPrefs.HasKey("Arrows") && PlayerPrefs.GetInt("Arrows") == 0)
        {
            text.text = "Please recreate the route.";
            arrows.SetActive(false);
        }

        if (PlayerPrefs.GetString("Direction") == "F")
        {
            Instantiate(player, startF.transform);
        }
        else if (PlayerPrefs.GetString("Direction") == "R")
        {

            Instantiate(player, startR.transform);
        }
        else {
            Instantiate(player, startF.transform);
        }

        GameObject subjectInstance = GameObject.Find("SubjectControllerWithTrail(Clone)");
        Debug.Log(subjectInstance);
        totalDistance = 0;
        totalTime = 0;
        lastPos = subjectInstance.transform.position;
        currentPos = subjectInstance.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space")) {
            StartCoroutine(Wait5MSecs());
        }
        if (Input.GetKey(KeyCode.Escape)) Application.Quit();
        if (Input.GetKey(KeyCode.E)) SceneManager.LoadScene("Menu");

        if (hasStarted && !hasEnded) {
            GameObject subjectInstance = GameObject.Find("SubjectControllerWithTrail(Clone)");
            currentPos = subjectInstance.transform.position;
            totalDistance += Vector3.Distance(currentPos, lastPos);
            totalTime += Time.deltaTime;
            lastPos = currentPos;
            Debug.Log(totalTime);
            Debug.Log(totalDistance);
        }
    }

    IEnumerator Wait5MSecs()
    {
        Debug.Log("Coroutine running");
        text.text = "+";
        yield return new WaitForSeconds(1);
        Image img = GameObject.Find("Panel").GetComponent<Image>();
        img.color = UnityEngine.Color.clear;
        hasStarted = true;


    }

    static public void MazeEnd() {
        //Debug.Log("MazeEnd() starting");

        hasEnded = true;
        float avgVelocity = totalDistance / totalTime;
        Scene scene = SceneManager.GetActiveScene();

        string[] lines = {
            "Participant ID: " + PlayerPrefs.GetString("ParticipantID"),
            "Experiment Type: " + PlayerPrefs.GetString("ExperimentType"),
            "Experimenter Initials: " + PlayerPrefs.GetString("ExperimenterInitials"),
            "Date: " + PlayerPrefs.GetString("Date"),
            "Distance: " + totalDistance,
            "Time: " + totalTime,
            "Avg. Velocity: " + avgVelocity
        };

        System.IO.File.WriteAllLines(@"C:\Users\james\Desktop\" + PlayerPrefs.GetString("ParticipantID") + "-" + scene.name + "-" + PlayerPrefs.GetString("Date") + ".txt", lines);
    }

    void OnApplicationQuit() {

        PlayerPrefs.DeleteAll();
    }

}