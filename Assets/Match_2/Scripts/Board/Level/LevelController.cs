using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Level Controller", menuName = "ScriptableObjects/Board/Levels")]
public class LevelController : ScriptableObject
{
    [SerializeField] private List<Level> levels;

    public List<Level> Levels => levels;

    private Level tempLevel;
    public Level CurrentLevel(int _levelNo)
    {
        if (_levelNo == 0)
            return levels[0];

        for (int i = 0; i < levels.Count; i++)
        {
            tempLevel = levels[i];
            if (tempLevel.LevelNo == _levelNo)
                return tempLevel;
        }

        return levels[^1];
    }


#if UNITY_EDITOR
    private const string path = "Assets/Match_2/ScriptableObjects/Levels";

    [ContextMenu("Get Levels From Folder")]
    public void GetLevelsFromFolder()
    {
        int count = GetDataFileCount(path);
        levels.Clear();

        for (int i = 0; i < count; i++)
        {
            Level level = AssetDatabase.LoadAssetAtPath<Level>($"{path}/Level{i + 1}.asset");
            levels.Add(level);
        }
        EditorUtility.SetDirty(this);
        EditorApplication.ExecuteMenuItem("File/Save Project");
    }


    private int GetDataFileCount(string _path)
    {
        int count = 0;
        DirectoryInfo dir = new DirectoryInfo(_path);
        FileInfo[] info = dir.GetFiles();

        foreach (FileInfo f in info)
        {
            if (!f.FullName.Contains("meta") && !f.FullName.Contains(".DS"))
                count++;
        }
        return count;
    }
#endif
}
