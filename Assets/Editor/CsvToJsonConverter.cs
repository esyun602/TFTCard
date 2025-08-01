using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

public class CsvToJsonWindow : EditorWindow
{
    [MenuItem("Window/CSV Tools/CSV → JSON Converter")]
    public static void Open()
    {
        // 창을 열고 제목 설정
        var win = GetWindow<CsvToJsonWindow>("CSV → JSON Converter");
        win.minSize = new Vector2(420, 160);
    }

    string csvPath = "";

    void OnGUI()
    {
        GUILayout.Label("CSV → JSON 변환기", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        csvPath = EditorGUILayout.TextField("CSV 파일", csvPath);
        if (GUILayout.Button("…", GUILayout.MaxWidth(25)))
            csvPath = EditorUtility.OpenFilePanel("CSV 파일 선택", "", "csv");
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUI.enabled = File.Exists(csvPath);
        if (GUILayout.Button("JSON으로 저장", GUILayout.Height(30)))
            ConvertAndSave(csvPath);
        GUI.enabled = true;
    }

    void ConvertAndSave(string path)
    {
        string savePath = EditorUtility.SaveFilePanel("JSON 저장 위치", "", 
                                                      Path.GetFileNameWithoutExtension(path) + ".json",
                                                      "json");
        if (string.IsNullOrEmpty(savePath)) return;

        try
        {
            string json = CsvToJson(File.ReadAllLines(path));
            File.WriteAllText(savePath, json);
            EditorUtility.DisplayDialog("완료", "JSON 파일 저장이 완료되었습니다.", "OK");
            EditorUtility.RevealInFinder(savePath);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
            EditorUtility.DisplayDialog("오류", "변환 중 문제가 발생했습니다.\n" + e.Message, "OK");
        }
    }


    //todo: string이 아니라 object 타입으로
    string CsvToJson(string[] lines)
    {
        if (lines.Length < 2) return "[]";

        string[] headers = lines[0].Split(',');

        var rows = new List<Dictionary<string, string>>();
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            string[] cells = lines[i].Split(',');

            if (cells.Length > 2)
            {
                var mergedStr = "";
                for (var j = 1; j < cells.Length; j++)
                {
                    mergedStr += cells[j];
                }

                cells = new[] { cells[0], mergedStr.Trim('\"') };
            }

            var obj = new Dictionary<string, string>();
            for (int c = 0; c < headers.Length && c < cells.Length; c++)
                obj[headers[c]] = cells[c]; 

            rows.Add(obj);
        }
        return JsonConvert.SerializeObject(rows, Formatting.Indented);
    }
}
