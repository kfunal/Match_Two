#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class LevelEditor : EditorWindow
{
    private EditorHelper helper => EditorHelper.Instance;

    private string header = "Level Editor";
    private const string levelsPath = "Assets/Match_2/ScriptableObjects/Levels";
    private const int redElementIndex = 0;
    private const int blueElementIndex = 1;
    private const int yellowElementIndex = 2;
    private const int greenElementIndex = 3;
    private const int noneIndex = 4;
    private const int woodBoxIndex = 10;
    private const int giftBoxIndex = 11;

    private int createLevelNo;
    private int createWidth;
    private int createHeight;
    private int loadLevelNo;
    private int loadWidth;
    private int loadHeight;
    private int moveCount;

    private Level currentLevel;

    private Texture blueElementTexture;
    private Texture redElementTexture;
    private Texture yellowElementTexture;
    private Texture greenElementTexture;
    private Texture woodBoxTexture;
    private Texture giftBoxTexture;
    private Texture noneTexture;
    private Texture2D bgTileTexture;

    private SerializedProperty endConditionList;
    private SerializedObject currentLevelObject;

    private Vector2 scrollPosition;

    private List<BoardTile> board;

    private int selectedElementIndex = 0;

    [MenuItem("Custom Editors/Level Editor")]
    public static void OpenWindow()
    {
        LevelEditor window = GetWindow<LevelEditor>("Level Editor");
        window.minSize = new Vector2(512, 512);
    }

    private void OnEnable()
    {
        createLevelNo = 0;
        createWidth = 0;
        createHeight = 0;
        currentLevel = null;
        currentLevelObject = null;
        board = null;
        selectedElementIndex = 0;

        GetElementSprites();
    }

    private void GetElementSprites()
    {
        bgTileTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/Sprites/BG_Tile.png");
        woodBoxTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/Sprites/WoodBox.png");
        giftBoxTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/Sprites/GiftBox.png");
        blueElementTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/Sprites/Blue.png");
        greenElementTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/Sprites/Green.png");
        redElementTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/Sprites/Red.png");
        yellowElementTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/Sprites/Yellow.png");
        noneTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/Sprites/None.png");
    }

    private void OnGUI()
    {
        helper.CenteredHorizontalLayout(() => GUILayout.Label(header));

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        EditorGUILayout.Space(15);
        CreateLevelArea();
        EditorGUILayout.Space(15);
        LoadLevelArea();
        EditorGUILayout.Space(15);
        BGTileInfoArea();
        EditorGUILayout.Space(15);
        EndConditionArea();
        EditorGUILayout.Space(15);
        ElementButtons();
        EditorGUILayout.Space(15);
        ButtonGrid();
        EditorGUILayout.EndScrollView();
        EditorGUILayout.Space(15);
    }

    private void ElementButtons()
    {
        if (currentLevel == null)
            return;

        if (currentLevelObject == null)
            return;

        if (board == null)
            return;

        helper.CenteredBox(IndexToTexture(selectedElementIndex), 60, 60);
        EditorGUILayout.Space(5);
        helper.CenteredHorizontalLayout(() =>
        {
            helper.Button(redElementTexture, 60, 60, redElementIndex, OnElementButtonClick);
            helper.Button(blueElementTexture, 60, 60, blueElementIndex, OnElementButtonClick);
            helper.Button(yellowElementTexture, 60, 60, yellowElementIndex, OnElementButtonClick);
            helper.Button(greenElementTexture, 60, 60, greenElementIndex, OnElementButtonClick);
            helper.Button(woodBoxTexture, 60, 60, woodBoxIndex, OnElementButtonClick);
            helper.Button(giftBoxTexture, 60, 60, giftBoxIndex, OnElementButtonClick);
            helper.Button(noneTexture, 60, 60, noneIndex, OnElementButtonClick);
        });
    }

    private void OnElementButtonClick(int _index) => selectedElementIndex = _index;

    private void ButtonGrid()
    {
        if (currentLevel == null)
            return;

        if (currentLevelObject == null)
            return;

        if (board == null)
            return;

        helper.ButtonGrid(currentLevel.Height - 1, currentLevel.Width, (int _row, int _column) =>
        {
            GUIContent buttonContent = new GUIContent(TileElementTexture(_row, _column));
            GUIStyle style = new GUIStyle(GUI.skin.box)
            {
                fixedHeight = 50,
                fixedWidth = 50,
                normal = new GUIStyleState() { background = bgTileTexture }
            };

            if (GUILayout.Button(buttonContent, style))
            {
                GetTile(_row, _column).ElementType = helper.IntToEnum<PoolType>(selectedElementIndex);
            }
        });
    }

    private void BGTileInfoArea()
    {
        if (currentLevel == null)
            return;

        if (currentLevelObject == null)
            return;

        helper.CenteredHorizontalLayout(() =>
        {
            EditorGUILayout.HelpBox("  Background Tiles will be created automatically.  \n  Just select elements.\n  If you want tile to be empty, use X.", MessageType.Warning);
        });
    }

    private void EndConditionArea()
    {
        if (currentLevel == null)
            return;

        if (currentLevelObject == null)
            return;

        endConditionList = currentLevelObject.FindProperty("endConditions");
        EditorGUILayout.PropertyField(endConditionList, true);
    }

    private void LoadLevelArea()
    {
        helper.LabelWithoutSpace("Load Level", 100, 15);
        EditorGUILayout.Space(10);

        helper.CenteredHorizontalLayout(600, 30, () =>
        {
            helper.LabelWithoutSpace("Level No: ", 60, 15);
            loadLevelNo = EditorGUILayout.IntField(loadLevelNo, new GUIStyle(GUI.skin.textField)
            {
                fixedHeight = 20,
                fixedWidth = 60,
                alignment = TextAnchor.MiddleCenter
            });

            helper.LabelWithoutSpace("Width: ", 40, 15);
            loadWidth = EditorGUILayout.IntField(loadWidth, new GUIStyle(GUI.skin.textField)
            {
                fixedHeight = 20,
                fixedWidth = 60,
                alignment = TextAnchor.MiddleCenter
            });

            helper.LabelWithoutSpace("Height: ", 45, 15);
            loadHeight = EditorGUILayout.IntField(loadHeight, new GUIStyle(GUI.skin.textField)
            {
                fixedHeight = 20,
                fixedWidth = 60,
                alignment = TextAnchor.MiddleCenter
            });

            helper.LabelWithoutSpace("Move Count: ", 75, 15);
            moveCount = EditorGUILayout.IntField(moveCount, new GUIStyle(GUI.skin.textField)
            {
                fixedHeight = 20,
                fixedWidth = 60,
                alignment = TextAnchor.MiddleCenter
            });
        });

        helper.CenteredHorizontalLayout(() =>
            {
                if (GUILayout.Button("Load Level"))
                    LoadLevel(loadLevelNo, true);

                if (GUILayout.Button("Save Changes"))
                    SaveLevelChanges();
            });
    }

    private void CreateLevelArea()
    {
        helper.LabelWithoutSpace("Create Level", 100, 15);
        EditorGUILayout.Space(10);

        helper.CenteredHorizontalLayout(450, 30, () =>
        {
            helper.LabelWithoutSpace("Level No: ", 60, 15);
            createLevelNo = EditorGUILayout.IntField(createLevelNo, new GUIStyle(GUI.skin.textField)
            {
                fixedHeight = 20,
                fixedWidth = 60,
                alignment = TextAnchor.MiddleCenter
            });

            helper.LabelWithoutSpace("Width: ", 40, 15);
            createWidth = EditorGUILayout.IntField(createWidth, new GUIStyle(GUI.skin.textField)
            {
                fixedHeight = 20,
                fixedWidth = 60,
                alignment = TextAnchor.MiddleCenter
            });

            helper.LabelWithoutSpace("Height: ", 45, 15);
            createHeight = EditorGUILayout.IntField(createHeight, new GUIStyle(GUI.skin.textField)
            {
                fixedHeight = 20,
                fixedWidth = 60,
                alignment = TextAnchor.MiddleCenter
            });

            if (GUILayout.Button("Create"))
                CreateLevel();
        });
    }

    private void CreateLevel()
    {
        if (createLevelNo == 0)
        {
            EditorUtility.DisplayDialog("Invalid Level No", "Please Check Level No", "OK");
            return;
        }

        if (createWidth == 0)
        {
            EditorUtility.DisplayDialog("Invalid Width", "Please Check Width", "OK");
            return;
        }

        if (createHeight == 0)
        {
            EditorUtility.DisplayDialog("Invalid Height", "Please Check Height", "OK");
            return;
        }

        try
        {
            if (File.Exists($"{levelsPath}/Level{createLevelNo}.asset"))
            {
                LoadLevel(createLevelNo, false);
                EditorUtility.DisplayDialog($"Already exist", $"Level {createLevelNo} exist and loaded", "OK");
                createLevelNo = 0;
                createHeight = 0;
                createWidth = 0;
                return;
            }

            Level asset = CreateInstance<Level>();
            AssetDatabase.CreateAsset(asset, $"{levelsPath}/Level{createLevelNo}.asset");
            AssetDatabase.SaveAssets();

            board = new List<BoardTile>();

            for (int row = 0; row < createHeight; row++)
            {
                for (int column = 0; column < createWidth; column++)
                {
                    BoardTile tile = new BoardTile(row, column, helper.IntToEnum<PoolType>(Random.Range(0, 4)));
                    board.Add(tile);
                }
            }

            moveCount = asset.MoveCount;

            asset.SaveChanges(createLevelNo, createWidth, createHeight, moveCount, board);
            EditorUtility.SetDirty(asset);
            EditorApplication.ExecuteMenuItem("File/Save Project");
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;

            LoadLevel(createLevelNo, false);
            EditorUtility.DisplayDialog($"Successful", $"Level {createLevelNo} cerated and loaded", "OK");
        }
        catch
        {
            EditorUtility.DisplayDialog("Something Wrong", "Something went wrong. Please Check The Values", "OK");
        }
    }

    private void LoadLevel(int _levelNo, bool _showSuccessDialog)
    {
        if (_levelNo == 0)
        {
            EditorUtility.DisplayDialog("Invalid Level No", "Please Check Level No", "OK");
            return;
        }

        if (!File.Exists($"{levelsPath}/Level{_levelNo}.asset"))
        {
            EditorUtility.DisplayDialog("Invalid Level No", $"There is no level {_levelNo}. Please Check Level No", "OK");
            return;
        }

        try
        {
            currentLevel = AssetDatabase.LoadAssetAtPath<Level>($"{levelsPath}/Level{_levelNo}.asset");

            loadLevelNo = _levelNo;
            loadWidth = currentLevel.Width;
            loadHeight = currentLevel.Height;
            moveCount = currentLevel.MoveCount;

            currentLevelObject = new SerializedObject(currentLevel);

            board = new List<BoardTile>();

            for (int i = 0; i < currentLevel.Elements.Count; i++)
            {
                BoardTile tile = currentLevel.Elements[i];
                board.Add(new BoardTile(tile.Row, tile.Column, tile.ElementType));
            }

            if (_showSuccessDialog)
                EditorUtility.DisplayDialog($"Successful", $"Level {_levelNo} Loaded", "OK");
        }
        catch (System.Exception)
        {
            EditorUtility.DisplayDialog($"Failed", $"Level {_levelNo} not found. Please Check Level Number", "OK");
        }
    }

    private void SaveLevelChanges()
    {
        if (currentLevel == null)
            return;

        if (currentLevelObject == null)
            return;

        if (board == null)
            return;

        if (loadLevelNo == 0)
            return;

        if (currentLevel.EndConditions != null && currentLevel.EndConditions.Count > 5)
        {
            EditorUtility.DisplayDialog("End Condition Problem", "There can be maximum 5 different end conditions. Please Check Them", "OK");
            return;
        }

        currentLevelObject.ApplyModifiedProperties();
        currentLevel.SaveChanges(loadLevelNo, loadWidth, loadHeight, moveCount, board);

        EditorUtility.SetDirty(currentLevel);
        EditorApplication.ExecuteMenuItem("File/Save Project");
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = currentLevel;

        LoadLevel(loadLevelNo, false);
    }

    private Texture TileElementTexture(int _row, int _column)
    {
        for (int i = 0; i < board.Count; i++)
        {
            BoardTile tile = board[i];

            if (tile.Row == _row && tile.Column == _column)
                return ElementTypeToTexture(tile.ElementType);
        }

        return null;
    }

    private Texture ElementTypeToTexture(PoolType _type)
    {
        return _type switch
        {
            PoolType.BlueElement => blueElementTexture,
            PoolType.RedElement => redElementTexture,
            PoolType.YellowElement => yellowElementTexture,
            PoolType.GreenElement => greenElementTexture,
            PoolType.WoodBox => woodBoxTexture,
            PoolType.GiftBox => giftBoxTexture,
            PoolType.None => noneTexture,
            _ => null
        };
    }

    private Texture IndexToTexture(int _index)
    {
        return _index switch
        {
            blueElementIndex => blueElementTexture,
            greenElementIndex => greenElementTexture,
            redElementIndex => redElementTexture,
            yellowElementIndex => yellowElementTexture,
            woodBoxIndex => woodBoxTexture,
            giftBoxIndex => giftBoxTexture,
            noneIndex => noneTexture,
            _ => null
        };
    }

    private BoardTile GetTile(int _row, int _column)
    {
        for (int i = 0; i < board.Count; i++)
        {
            BoardTile tile = board[i];

            if (tile.Row == _row && tile.Column == _column)
                return tile;
        }

        return null;
    }
}
#endif