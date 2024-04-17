using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelGenerator : EditorWindow
{
    private GameObject[] roomPrefabs = new GameObject[0];

    private int widthOffset = 10;
    private int heightOffset = 8;
    private int levelWidth = 4;
    private int levelHeight = 4;
    private Vector2 scrollPosition;
    private GameObject newLevel = null;

    // This attribute makes the window accessible from the menu
    [MenuItem("Window/LevelCreator")]
    public static void ShowWindow()
    {
        GetWindow<LevelGenerator>("Level Creator");
    }

    void OnGUI()
    {
        GUILayout.Label("Rooms:", EditorStyles.boldLabel);
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition); // Begin the scroll region

        // Adjust size as needed
        int arraySize = EditorGUILayout.IntField("Rooms Array Size", roomPrefabs.Length);

        if (arraySize != roomPrefabs.Length)
        {
            Array.Resize(ref roomPrefabs, arraySize);
        }

        for (int i = 0; i < roomPrefabs.Length; i++)
        {
            roomPrefabs[i] =
                (GameObject)EditorGUILayout.ObjectField($"Room {i + 1}", roomPrefabs[i], typeof(GameObject), false);
        }
        EditorGUILayout.EndScrollView(); // End the scroll region

        if (GUILayout.Button("Generate Level"))
        {
            GenerateLevel();
        }

    }

    private void GenerateLevel()
    {
        if (newLevel)
        {
            DestroyImmediate(newLevel);
        }

        newLevel = new GameObject("Level");
        newLevel.transform.position = Vector3.zero;

        for (int x = 0; x < levelWidth; x++)
        {
            for (int y = 0; y < levelHeight; y++)
            {
                int roomIndex = Random.Range(0, roomPrefabs.Length);
                Vector3 position = new Vector3(x * widthOffset, y * heightOffset, 0);
                GameObject newRoom = Instantiate(roomPrefabs[roomIndex], position, Quaternion.identity);
                newRoom.transform.parent = newLevel.transform;
            }
        }
    }
}
