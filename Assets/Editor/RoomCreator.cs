using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomCreator : EditorWindow
{
    private int roomWidth = 10;
    private int roomHeight = 8;
    private float tileOffset = 1;
    private Vector2 scrollPosition; // To track scrolling
    private Vector2 dbScrollPosition;
    private GameObject newRoom;

    private string defaultRoom = "00000000110060000L040000000P110000000L110000000L11000000001100000000111112222111";
    private string defaultChunk = "011100222000000";
    private string roomName = "New Room";
    private Tile tilePrefab;
    private Sprite ladderSprite;
    private Sprite ladderPlatformSprite;
    private Sprite[] backgroundSprites = new Sprite[0];
    private Sprite[] wallSprites = new Sprite[0];
    private Sprite stoneBlockSprite;
    private List<Tile> tiles = new List<Tile>();

    // This attribute makes the window accessible from the menu
    [MenuItem("Window/RoomCreator")]
    public static void ShowWindow()
    {
        GetWindow<RoomCreator>("RoomCreator");
    }

    void OnGUI()
    {
        // Your custom editor controls and logic go here using IMGUI
        GUILayout.Label("Room Name:", EditorStyles.boldLabel);
        roomName = EditorGUILayout.TextField(roomName);

        GUILayout.Label("Room Code:", EditorStyles.boldLabel);
        defaultRoom = EditorGUILayout.TextArea(defaultRoom, GUILayout.Height(130));

        GUILayout.Label("Chunk Code:", EditorStyles.boldLabel);
        defaultChunk = EditorGUILayout.TextArea(defaultChunk, GUILayout.Height(130));

        GUILayout.Label("Prefab Field:", EditorStyles.boldLabel);
        tilePrefab = (Tile)EditorGUILayout.ObjectField("Tile Prefab", tilePrefab, typeof(Tile), false);

        DrawSpriteSection();

        if (GUILayout.Button("Generate Room"))
        {
            GenerateRoom();
        }

        if (GUILayout.Button("Export As Prefab"))
        {
            ExportRoom();
        }
    }

    private void DrawSpriteSection()
    {
        GUILayout.Label("Sprites:", EditorStyles.boldLabel);
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition); // Begin the scroll region

        // Adjust size as needed
        int bgArraySize = EditorGUILayout.IntField("BG Sprites Array Size", backgroundSprites.Length);

        if (bgArraySize != backgroundSprites.Length)
        {
            Array.Resize(ref backgroundSprites, bgArraySize);
        }

        for (int i = 0; i < backgroundSprites.Length; i++)
        {
            backgroundSprites[i] =
                (Sprite)EditorGUILayout.ObjectField($"BG Sprite {i + 1}", backgroundSprites[i], typeof(Sprite), false);
        }

        // Adjust size as needed
        int wallArraySize = EditorGUILayout.IntField("Wall Sprites Array Size", wallSprites.Length);

        if (wallArraySize != wallSprites.Length)
        {
            Array.Resize(ref wallSprites, wallArraySize);
        }

        for (int i = 0; i < wallSprites.Length; i++)
        {
            wallSprites[i] =
                (Sprite)EditorGUILayout.ObjectField($"Wall Sprite {i + 1}", wallSprites[i], typeof(Sprite), false);
        }

        ladderSprite = (Sprite)EditorGUILayout.ObjectField("Ladder Sprite", ladderSprite, typeof(Sprite), false);
        ladderPlatformSprite =
            (Sprite)EditorGUILayout.ObjectField("Ladder Platform Sprite", ladderPlatformSprite, typeof(Sprite), false);
        stoneBlockSprite =
            (Sprite)EditorGUILayout.ObjectField("Stone Block Sprite", stoneBlockSprite, typeof(Sprite), false);
        EditorGUILayout.EndScrollView(); // End the scroll region
    }

    private void GenerateRoom()
    {
        if (newRoom)
        {
            DestroyImmediate(newRoom);
        }

        tiles = new List<Tile>();
        newRoom = new GameObject(roomName);
        newRoom.transform.position = Vector3.zero;

        for (int i = 0; i < defaultRoom.Length; i++)
        {
            Tile newTile = Instantiate(tilePrefab, Vector3.zero, Quaternion.identity);
            int y = i / roomWidth;

            // Calculate x coordinate (column)
            int x = i % roomWidth;
            int index = y * roomWidth + x;
            newTile.name = "Tile: " + index;
            tiles.Add(newTile);
            newTile.transform.parent = newRoom.transform;
        }

        for (int i = 0; i < defaultRoom.Length; i++)
        {
            // Do something with currentChar
            char currentChar = defaultRoom[i];
            // Calculate y coordinate (row)
            int y = i / roomWidth;

            // Calculate x coordinate (column)
            int x = i % roomWidth;
            int index = y * roomWidth + x;
            Tile newTile = tiles[index];
            newTile.name = "Tile: " + index;
            if (!newTile.IsPartofChunk)
            {
                newTile.transform.position = new Vector3(x, roomHeight - y, 0);
                CreateTile(currentChar, newTile, x, y, false); 
            }
            else
            {
                newTile.transform.position = new Vector3(x, roomHeight - y, newTile.transform.position.z);
            }
        }
    }

    private void CreateTile(char currentChar, Tile newTile, int x, int y, bool overrideChunk = false)
    {
        switch (currentChar)
        {
            case '0':
                CreateBGTile(newTile, overrideChunk);
                break;
            case '1':
                CreateWallTile(newTile, overrideChunk);
                break;
            case '2':
                int wallChance = Random.Range(0, 100);
                if (wallChance <= 50)
                {
                    CreateWallTile(newTile, overrideChunk);
                }
                else
                {
                    CreateBGTile(newTile, overrideChunk);
                }

                break;
            case '4':
                newTile.UpdateTile(stoneBlockSprite, true, overrideChunk);
                break;
            case '6':
                CreateChunk(newTile, x, y);
                break;
            case 'L':
                newTile.UpdateTile(ladderSprite, true, overrideChunk);
                break;
            case 'P':
                newTile.UpdateTile(ladderPlatformSprite, true, overrideChunk);
                break;
            default:
                newTile.UpdateTile(null, false, overrideChunk);
                break;
        }
    }

    private void CreateChunk(Tile newTile, int startX, int startY)
    {
        for (int i = 0; i < defaultChunk.Length; i++)
        {
            // Do something with currentChar
            char currentChar = defaultChunk[i];
            // Calculate y coordinate (row)
            int y = i / 5;

            // Calculate x coordinate (column)
            int x = i % 5;

            if (i == 0)
            {
                CreateTile(currentChar, newTile, x, y);
            }
            else
            {
                int roomX = startX + x;
                int roomY = startY + y;
                int index = roomY * roomWidth + roomX;
                CreateTile(currentChar, tiles[index], roomX, roomY, true);
            }
        }
    }

    private void CreateWallTile(Tile newTile, bool overrideChunk = false)
    {
        int wallTileIndex = Random.Range(0, wallSprites.Length);
        newTile.UpdateTile(wallSprites[wallTileIndex], true, overrideChunk);
    }

    private void CreateBGTile(Tile newTile, bool overrideChunk = false)
    {
        int bgTileIndex = Random.Range(0, backgroundSprites.Length);
        newTile.UpdateTile(backgroundSprites[bgTileIndex], false, overrideChunk);
    }

    private void ExportRoom()
    {
        if (!newRoom)
        {
            return;

        }
        // Example save path
        string prefabPath = "Assets/Prefabs/" + roomName + ".prefab";
        PrefabUtility.SaveAsPrefabAsset(newRoom, prefabPath);
    }
}
