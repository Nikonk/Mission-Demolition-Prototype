using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum GameMode {
    idle,
    playing,
    levelEnd
}

public class MissionDemolition : MonoBehaviour {
    static private MissionDemolition    S;

    [Header("Set in Inspector")]
    public TMP_Text         uitLevel;
    public TMP_Text         uitShots;
    public TMP_Text         uitBestResult;
    public TMP_Text         uitButton;
    public Vector3          castlePos;
    public GameObject[]     castles;

    [Header("Set Dynamically")]
    public int              level;
    public int              levelMax;
    public int              shotsTaken;
    public GameObject       castle;
    public GameMode         mode = GameMode.idle;
    public string           showing = "Show Slingshot";

    private void Start() {     
        UpdateBestResult();

        S = this;
        level = 0;
        levelMax = castles.Length;
        StartLevel();
    }

    private void Update() {
        UpdateGUI();

        if ((mode == GameMode.playing) && Goal.goalMet) {
            mode = GameMode.levelEnd;
            SwitchView("Show Both");
            Invoke("NextLevel", 2f);
        }
    }

    void StartLevel() {
        if (castle != null) {
            Destroy(castle);
        }

        GameObject[] gos = GameObject.FindGameObjectsWithTag("Projectile");
        foreach (GameObject pTemp in gos) {
            Destroy(pTemp);
        }

        castle = Instantiate<GameObject>(castles[level]);
        castle.transform.position = castlePos;
        shotsTaken = 0;

        SwitchView("Show Both");
        ProjectileLine.S.Clear();

        Goal.goalMet = false;

        UpdateGUI();

        mode = GameMode.playing;
    }

    void NextLevel() {
        PlayerPrefs.SetInt(level + " level", S.shotsTaken);

        level++;
        if (level == levelMax) {
            level = 0;
        }
        StartLevel();
    }

    void UpdateGUI() {
        uitLevel.text = "Level: " + level + " of " + (levelMax - 1);
        uitShots.text = "Shots Taken: " + shotsTaken;

        UpdateBestResult();
    }

    void UpdateBestResult() {
        uitBestResult.text = "Best score: \n";
        for (int i = 0; i < levelMax; i++) {
            uitBestResult.text += "Level " + i + ": "
                                    + PlayerPrefs.GetInt(i + " level", 0) + "\n";
        }
    }

    public void SwitchView(string eView = "") {
        if (eView == "") {
            eView = uitButton.text;
        }
        showing = eView;
        switch (showing) {
            case "Show Slingshot":
                FollowCam.POI = null;
                uitButton.text = "Show Castle";
                break;
            
            case "Show Castle":
                FollowCam.POI = S.castle;
                uitButton.text = "Show Both";
                break;

            case "Show Both":
                FollowCam.POI = GameObject.Find("ViewBoth");
                uitButton.text = "Show Slingshot";
                break;
        }
    }

    public static void ShotFired() {
        S.shotsTaken++;
    }
}
