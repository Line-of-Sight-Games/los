using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
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
    public Vector3 groundPosition;
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
        groundPosition = Vector3.zero;
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
