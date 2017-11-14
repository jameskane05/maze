using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MazeCtrl : MonoBehaviour {

    public Text text;
    public GameObject player;
    public GameObject arrows;
    public GameObject startHall;
    public GameObject endHall;
    public GameObject above;
    public Transform startLoc;
    public Transform endLoc;
    static public bool hasEnded;
    public bool hasStarted;
    static public Material pathColor;
    static public GameObject subjectInstance;
    static public int onCorner = 0;
    static public float totalTime;
    static public bool cornerEvent = false;
    public Transform[] Rotations;
    public Sprite[] introImgs;

    private Vector3 lastPos;
    private Vector3 currentPos;
    private int picCounter;
    private bool startCorner = false;
    static private float totalDistance;
    static private float avgVelocity;
    static private Scene thisScene;
    static private List<string> path;
    static public Transform cornerTransform;

    void Start() {

        // Arrow settings
        if (PlayerPrefs.HasKey("Arrows") && PlayerPrefs.GetInt("Arrows") == 1) arrows.SetActive(true);
        else if (PlayerPrefs.HasKey("Arrows") && PlayerPrefs.GetInt("Arrows") == 0)
        {
            text.text = "Please recreate the route.";
            arrows.SetActive(false);
        }

        // Start & end position settings
        if (PlayerPrefs.GetString("Direction") == "F")
        {
            Instantiate(player, startLoc.transform);  // create the player at startLoc
            endHall.AddComponent<End>();  // add End script to end hallway
        }
        else if (PlayerPrefs.GetString("Direction") == "R")
        {
            Instantiate(player, endLoc.transform); // create the player at the endHall
            startHall.AddComponent<End>(); // add the End script to the startHall
        }
        else {  // must be during dev, go w/ default
            Instantiate(player, startLoc.transform);
            endHall.AddComponent<End>();
        }

        GameObject subjectInstance = GameObject.Find("SubjectControllerWithTrail(Clone)");
        totalDistance = 0;
        totalTime = 0;
        lastPos = subjectInstance.transform.position;
        currentPos = subjectInstance.transform.position;
        path = new List<string>();
        hasEnded = false;
        hasStarted = false;
        picCounter = 0;
        //UnityStandardAssets.Characters.FirstPerson.FirstPersonController controller = subjectInstance.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>();
        //controller.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space") && !hasStarted) {

            //if statements checking for intro section once space has been pressed

            if (PlayerPrefs.GetString("Intro") == "8Pics")
            {
                StartCoroutine(EightPicsIntro());

                InvokeRepeating("TrackPathEverySecond", 0, 1.0f);
            }
            else if (PlayerPrefs.GetString("Intro") == "1Pic")
            {
                StartCoroutine(OnePicIntro());
                InvokeRepeating("TrackPathEverySecond", 0, 1.0f);
            }
            else
            {
                StartCoroutine(Wait5MSecsAndStart());
                InvokeRepeating("TrackPathEverySecond", 0, 1.0f);
            }
        }

        if (Input.GetKeyDown(KeyCode.E) && !hasEnded)
        {
            Debug.Log("E pressed, calling MazeEnd() and loading menu");
            MazeEnd();
            SceneManager.LoadScene("Menu");
        }
        else if (Input.GetKeyDown(KeyCode.E) && hasEnded)
        {
            Debug.Log("E pressed, not calling MazeEnd() and loading menu");
            SceneManager.LoadScene("Menu");
        }

        if (hasStarted && !hasEnded) {
            GameObject subjectInstance = GameObject.Find("SubjectControllerWithTrail(Clone)");
            currentPos = subjectInstance.transform.position;
            totalDistance += Vector3.Distance(currentPos, lastPos);
            totalTime += Time.deltaTime;
            lastPos = currentPos;
        }
        

        // "RotateOrPause" is set and the player has encountered a corner event
        if (cornerEvent)
        {
            DontDestroyOnLoad(cornerTransform);
            cornerEvent = false; // Set it to false immediately

            if (!string.IsNullOrEmpty(PlayerPrefs.GetString("RotateOrPause")))
            {


                if (PlayerPrefs.GetString("RotateOrPause") == "Pause")
                {
                    StartCoroutine(Pause());
                }
                if (PlayerPrefs.GetString("RotateOrPause") == "Rotate")
                {
                    Debug.Log(onCorner);
                    startCorner = true;
                }
            }
        }

        if (startCorner)
        {
            GameObject subjectInstance = GameObject.Find("SubjectControllerWithTrail(Clone)");
            UnityStandardAssets.Characters.FirstPerson.FirstPersonController controller = 
                subjectInstance.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>();

            controller.enabled = false;

            subjectInstance.transform.position = Vector3.MoveTowards(
                subjectInstance.transform.position,
                new Vector3(cornerTransform.transform.position.x, cornerTransform.transform.position.y + 1f, cornerTransform.transform.position.z),
                3f * Time.deltaTime
            );
            
            subjectInstance.transform.rotation = Quaternion.Lerp(
                subjectInstance.transform.rotation,
                Rotations[onCorner].rotation,
                3f * Time.deltaTime
            );

            Debug.Log("subject: " +subjectInstance.transform.rotation.eulerAngles.y);
            Debug.Log("corner: " +Rotations[onCorner].rotation.eulerAngles.y);

            if (Vector3.Distance(subjectInstance.transform.rotation.eulerAngles, Rotations[onCorner].rotation.eulerAngles) <= 0.1f)
            {
                startCorner = false;
                StartCoroutine(Pause());
            }
        }
        
    }

    IEnumerator OnePicIntro() {
        text.text = "";
        Image img = GameObject.Find("Panel").GetComponent<Image>();
        img.color = UnityEngine.Color.white;
        img.sprite = introImgs[picCounter];
        yield return new WaitForSeconds(12);
        img.sprite = null;
        img.color = UnityEngine.Color.black;
        StartCoroutine(Wait5MSecsAndStart());
    }

    IEnumerator EightPicsIntro()
    {
        text.text = "";
    
        Image img = GameObject.Find("Panel").GetComponent<Image>();
        while (picCounter < introImgs.Length)
        {
            img.color = UnityEngine.Color.white;
            img.sprite = introImgs[picCounter];
            yield return new WaitForSeconds(4);
            picCounter++;
        }

        img.color = UnityEngine.Color.black;
        StartCoroutine(Wait5MSecsAndStart());
    }

    IEnumerator Wait5MSecsAndStart()
    {
        text.text = "+";
        yield return new WaitForSeconds(1);
        Image img = GameObject.Find("Panel").GetComponent<Image>();
        img.color = UnityEngine.Color.clear;
        hasStarted = true;
        text.text = "";
    }

    IEnumerator Wait2Seconds()
    {
        yield return new WaitForSeconds(2);
    }

    public void TrackPathEverySecond()
    {
        GameObject subjectInstance = GameObject.Find("SubjectControllerWithTrail(Clone)");
        int second = Mathf.RoundToInt(totalTime);
        path.Add( second.ToString() + ": " + subjectInstance.transform.position);
    }

    IEnumerator Pause()
    {
        Debug.Log("Pausing");
        GameObject subjectInstance = GameObject.Find("SubjectControllerWithTrail(Clone)");
        UnityStandardAssets.Characters.FirstPerson.FirstPersonController controller = subjectInstance.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>();
        controller.enabled = false;
        yield return new WaitForSeconds(8);
        controller.enabled = true;
    }

    static public void MazeEnd()
    {
        hasEnded = true;

        Text text = GameObject.Find("Text").GetComponent<Text>();
        text.text = "You have reached the end.";
        TrailRenderer trail = GameObject.Find("Trail").GetComponent<TrailRenderer>();
        Debug.Log(trail);
        Debug.Log(trail.material);
        Material mat = Resources.Load("TurquoiseSmooth") as Material;
        Debug.Log(mat);
        trail.material = mat;

        Debug.Log(trail.material);

        float avgVelocity = totalDistance / totalTime;
        Scene scene = SceneManager.GetActiveScene();

        Debug.Log("Time:" + totalTime);
        Debug.Log("Distance:" + totalDistance);


        string[] lines = {
            "Participant ID: " + PlayerPrefs.GetString("ParticipantID"),
            "Experiment Type: " + PlayerPrefs.GetString("ExperimentType"),
            "Experimenter Initials: " + PlayerPrefs.GetString("ExperimenterInitials"),
            "Maze: " + scene.name,
            "Date: " + PlayerPrefs.GetString("Date"),
            "Distance: " + totalDistance,
            "Time: " + totalTime,
            "Avg. Velocity: " + avgVelocity,
        };

        if (!string.IsNullOrEmpty(PlayerPrefs.GetString("dir")))
        {
            string localDir = PlayerPrefs.GetString("dir") + "\\";
            localDir += PlayerPrefs.GetString("ParticipantID") + "-" + PlayerPrefs.GetString("ExperimentType") + "-";
            localDir += scene.name + "-";

            if (!string.IsNullOrEmpty(PlayerPrefs.GetInt("Arrows").ToString()))
            {
                if (PlayerPrefs.GetInt("Arrows") == 0) localDir += "NoArrows-";
                else localDir += "WithArrows-";
            }

            if (!string.IsNullOrEmpty(PlayerPrefs.GetString("Direction")))
            {
                if (PlayerPrefs.GetString("Direction") == "F") localDir += "Fwd";
                else localDir += "Rev";
            }

            if (!string.IsNullOrEmpty(PlayerPrefs.GetString("RotateOrPause"))) localDir += "-" + PlayerPrefs.GetString("RotateOrPause");

            if (!string.IsNullOrEmpty(PlayerPrefs.GetString("Intro"))) localDir += "-" + PlayerPrefs.GetString("RotateOrPause");

            if (System.IO.Directory.Exists(localDir))
            {
                System.IO.File.WriteAllLines(localDir + "-coords.txt", path.ToArray());
                System.IO.File.WriteAllLines(localDir + ".txt", lines);
                TakePhoto(scene.name);
            }

            else
            {
                MenuScript.SetDir();
                System.IO.File.WriteAllLines(localDir + "-coords.txt", path.ToArray());
                System.IO.File.WriteAllLines(localDir + ".txt", lines);
                TakePhoto(scene.name);
            }

            PlayerPrefs.DeleteKey("Arrows");
            PlayerPrefs.DeleteKey("RotateOrPause");
            PlayerPrefs.DeleteKey("Direction");
            PlayerPrefs.DeleteKey("Rotate");
        }
    }

    static public void TakePhoto(string sceneName)
    {
        Camera cam = GameObject.Find("Overhead Cam").GetComponent<Camera>();
        RenderTexture currentRT = RenderTexture.active;
        var rTex = new RenderTexture(cam.pixelHeight, cam.pixelHeight, 16);
        cam.targetTexture = rTex;
        RenderTexture.active = cam.targetTexture;
        cam.Render();
        Texture2D tex = new Texture2D(cam.targetTexture.width, cam.targetTexture.height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), 0, 0);
        tex.Apply(false);
        RenderTexture.active = currentRT;
        byte[] bytes = tex.EncodeToPNG();
        Destroy(tex);
        string localDir = PlayerPrefs.GetString("dir") + "\\";
        localDir += PlayerPrefs.GetString("ParticipantID") + "-" + PlayerPrefs.GetString("ExperimentType") + "-";
        localDir += sceneName + "-";

        if (!string.IsNullOrEmpty(PlayerPrefs.GetInt("Arrows").ToString()))
        {
            if (PlayerPrefs.GetInt("Arrows") == 0) localDir += "NoArrows-";
            else localDir += "WithArrows-";
        }

        if (!string.IsNullOrEmpty(PlayerPrefs.GetString("Direction")))
        {
            if (PlayerPrefs.GetString("Direction") == "F") localDir += "Fwd";
            else localDir += "Rev";
        }

        if (!string.IsNullOrEmpty(PlayerPrefs.GetString("RotateOrPause"))) localDir += "-" + PlayerPrefs.GetString("RotateOrPause");

        if (!string.IsNullOrEmpty(PlayerPrefs.GetString("Intro"))) localDir += "-" + PlayerPrefs.GetString("RotateOrPause");

        System.IO.File.WriteAllBytes(localDir + "-path.png", bytes);
    }

    void OnApplicationQuit() {
        PlayerPrefs.DeleteAll();
    }

}