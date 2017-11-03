using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class End : MonoBehaviour {
   
    public Material colorMat;

    private void OnTriggerEnter(Collider other)
    {
        TrailRenderer trail = GameObject.Find("Trail").GetComponent<TrailRenderer>();
        GameObject player = GameObject.Find("SubjectControllerWithTrail(Clone)").GetComponent<GameObject>();
        Debug.Log(player);
        Text text = GameObject.Find("Text").GetComponent<Text>();
        Debug.Log(text.text);
        text.text = "You have reached the end.";
        trail.material = colorMat;
        MazeCtrl.MazeEnd();
    }
    
}
