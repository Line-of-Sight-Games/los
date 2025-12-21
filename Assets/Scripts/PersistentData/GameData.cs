using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public bool lozMode;
    public int currentRound;
    public int currentTeam;
    public int currentTurn;
    public int maxRounds;
    public int maxTeams;
    public float playTimeTotal, turnTime;
    public Vector3 mapPosition;
    public Vector3 mapDimensions;
    public Vector3 bottomPlanePosition;
    public Vector3 bottomPlaneDimensions;
    public Vector3 outlineAreaPosition;
    public Vector3 outlineAreaDimensions;
    public Vector3 camPosition;
    public float camOrthoSize;
    public Vector3 sunPosition;
    public int maxTurnTime;
    public int maxX, maxY, maxZ;
    public List<string> savedWeather;
    public List<string> allSoldiersIds;
    public Dictionary<string, Dictionary<string, object>> allSoldiersDetails;
    public List<string> allItemIds;
    public Dictionary<string, Dictionary<string, object>> allItemDetails;
    public List<string> allPOIIds;
    public Dictionary<string, Dictionary<string, object>> allPOIDetails;
    public int numberBasicZombies, numberBruteZombies;

    public GameData()
    {
        lozMode = false;
        currentRound = 0;
        currentTeam = 0;
        currentTurn = 0;
        maxRounds = 0;
        maxTeams = 0;
        playTimeTotal = 0;
        mapPosition = Vector3.zero;
        mapDimensions = Vector3.zero;
        bottomPlanePosition = Vector3.zero;
        bottomPlaneDimensions = Vector3.zero;
        outlineAreaPosition = Vector3.zero;
        outlineAreaDimensions = Vector3.zero;
        camPosition = Vector3.zero;
        camOrthoSize = 0;
        sunPosition = Vector3.zero;
        turnTime = 0;
        maxTurnTime = 0;
        maxX = 0;
        maxY = 0;
        maxZ = 0;
        savedWeather = new List<string>();

        allSoldiersIds = new List<string>();
        allSoldiersDetails = new Dictionary<string, Dictionary<string, object>>();

        allItemIds = new List<string>();
        allItemDetails = new Dictionary<string, Dictionary<string, object>>();

        allPOIIds = new List<string>();
        allPOIDetails = new Dictionary<string, Dictionary<string, object>>();
    }
}
