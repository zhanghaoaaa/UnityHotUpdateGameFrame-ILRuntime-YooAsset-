using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Globalization;

public class FontToolEditor : EditorWindow
{
    private const string MENU_MAIN_WINDOW = "Tools/Font替换";

    private List<Font> fontList = new List<Font>() { null };
    public Font toChange;
    static Font toChangeFont;
    public string path = "Assets/";
    static string path1;
    public int num = 0;
    [MenuItem(MENU_MAIN_WINDOW)]
    public static void Open()
    {
        EditorWindow.GetWindow(typeof(FontToolEditor), true);
    }

    void OnGUI()
    {
        GUILayout.Space(5);
        GUILayout.Label("选择老字体", GUILayout.Width(100f));
        num = EditorGUILayout.IntField("字体个数", num);
        AddMonsScriptList();

        GUILayout.Space(20);

        toChange = (Font)EditorGUILayout.ObjectField("选择更换新字体", toChange, typeof(Font), true, GUILayout.MinWidth(100));
        toChangeFont = toChange;

        GUILayout.Space(20);

        GUILayout.Label("路径", GUILayout.Width(50f));

        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("浏览"))
            {
                path = EditorUtility.OpenFolderPanel("窗口标题", Application.streamingAssetsPath, "");
                int index = path.IndexOf("Assets");
                path = path.Substring(index, path.Length - index) + "/";
            }
        }

        GUILayout.EndHorizontal();

        path1 = path;
        GUILayout.TextField(path);
        if (GUILayout.Button("确认更换"))
        {
            Init();
        }
    }
    private void AddMonsScriptList()
    {
        int scriptListCount = fontList.Count - 1;
        for (int i = 0; i < num; i++)
        {
            if (scriptListCount < i)
            {
                //给脚本添加一个空值,为了界面好看,也为了下面缓存脚本
                fontList.Add(null);
            }
            fontList[i] = (Font)EditorGUILayout.ObjectField(fontList[i], typeof(Font), true);
        }
    }

    private void Init()
    {
        List<GameObject> prefabs = new List<GameObject>();
        var resourcesPath = path1;
        var absolutePaths = System.IO.Directory.GetFiles(resourcesPath, "*.prefab", System.IO.SearchOption.AllDirectories);

        for (int i = 0; i < absolutePaths.Length; i++)
        {
            EditorUtility.DisplayProgressBar("字体转换中...", "字体转换中...", (float)i / absolutePaths.Length);
            string path = absolutePaths[i].Replace("\\", "/");
            GameObject prefab = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;

            //替换font
            Text[] labels = prefab.GetComponentsInChildren<Text>(true);
            for (int q = 0; q < labels.Length; q++)
            {
                if (labels[q].font)
                {
                    for (int w = 0; w < fontList.Count; w++)
                    {
                        if (fontList[w].name == labels[q].font.name)
                        {
                            int fontSize = labels[q].fontSize;
                            var fonStyle = labels[q].fontStyle;
                            labels[q].font = toChangeFont;
                            labels[q].fontSize = fontSize;
                            labels[q].fontStyle = fonStyle;
                            EditorUtility.SetDirty(labels[q]);
                        }
                    }

                }
            }
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }
}
