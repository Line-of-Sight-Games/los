using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TabunCloud : POI, IDataPersistence
{
    readonly int segments = 100;
    readonly int tabunRadius = 20;
    public int turnsUntilDissipation;
    public List<string> alliesAffected, enemiesAffected;
    public string placedById;
    public Soldier placedBy;
    private void Start()
    {
        poiType = "tabun";
        menu = FindObjectOfType<MainMenu>();
        game = FindObjectOfType<MainGame>();
        poiManager = FindObjectOfType<POIManager>();
    }

    public TabunCloud Init(Tuple<Vector3, string> location, string thrownBy)
    {
        id = GenerateGuid();
        x = (int)location.Item1.x;
        y = (int)location.Item1.y;
        z = (int)location.Item1.z;
        terrainOn = location.Item2;
        GenerateCircleMesh(tabunRadius);
        transform.position = new Vector3(x - 0.5f, z + 0.2f, y - 0.5f);

        turnsUntilDissipation = 3;
        placedById = thrownBy;
        placedBy = menu.soldierManager.FindSoldierById(placedById);

        SpawnCloud();

        return this;
    }
    private void Update()
    {
        placedBy = menu.soldierManager.FindSoldierById(placedById);
    }
    void GenerateCircleMesh(int radius)
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = new();
        meshFilter.mesh = mesh;

        float angleStep = 2 * Mathf.PI / segments;

        Vector3[] vertices = new Vector3[segments + 1]; // +1 for the center vertex
        int[] triangles = new int[segments * 3];

        // Central vertex
        vertices[0] = Vector3.zero;

        // Create vertices in a circle
        for (int i = 1; i <= segments; i++)
        {
            float x = Mathf.Cos(angleStep * (i - 1)) * radius;
            float z = Mathf.Sin(angleStep * (i - 1)) * radius;
            vertices[i] = new Vector3(x, 0.1f, z);
        }

        // Create triangles
        for (int i = 0; i < segments - 1; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        // Close the loop with the last triangle
        triangles[(segments - 1) * 3] = 0;
        triangles[(segments - 1) * 3 + 1] = segments;
        triangles[(segments - 1) * 3 + 2] = 1;

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    public override void LoadData(GameData data)
    {
        if (data.allPOIDetails.TryGetValue(id, out details))
        {
            //load position
            poiType = (string)details["poiType"];
            x = Convert.ToInt32(details["x"]);
            y = Convert.ToInt32(details["y"]);
            z = Convert.ToInt32(details["z"]);
            terrainOn = (string)details["terrainOn"];
            GenerateCircleMesh(tabunRadius);
            transform.position = new Vector3(x - 0.5f, z + 0.2f, y - 0.5f);

            turnsUntilDissipation = Convert.ToInt32(details["turnsUntilDissipation"]);
            alliesAffected = (details["alliesAffected"] as JArray).Select(token => token.ToString()).ToList();
            enemiesAffected = (details["enemiesAffected"] as JArray).Select(token => token.ToString()).ToList();
            placedById = (string)details["placedById"];
        }
    }

    public override void SaveData(ref GameData data)
    {
        details = new()
        {
            { "poiType", poiType },
            { "x", x },
            { "y", y },
            { "z", z },
            { "terrainOn", terrainOn },
            { "turnsUntilDissipation", turnsUntilDissipation },
            { "alliesAffected", alliesAffected },
            { "enemiesAffected", enemiesAffected },
            { "placedById", placedById }
        };

        //add the item in
        if (data.allPOIDetails.ContainsKey(id))
            data.allPOIDetails.Remove(id);

        data.allPOIDetails.Add(id, details);
    }
    public void SpawnCloud()
    {
        game.CheckAllTabunClouds();
    }
    public void DissipateCloud()
    {
        //pay xp
        int xp = enemiesAffected.Count - alliesAffected.Count;
        if (xp > 0)
            menu.AddXpAlert(placedBy, xp, $"Tabun grenade affected {enemiesAffected.Count} enemies and {alliesAffected.Count} allies.", false);

        //dissipate and recheck
        game.CheckAllTabunClouds();
    }
    public int TurnsUntilDissipation
    {
        get { return turnsUntilDissipation; }
        set
        {
            turnsUntilDissipation = value;
            if (turnsUntilDissipation == 0)
                DissipateCloud();
        }
    }
}
