using System;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using DG.DemiEditor;
using Newtonsoft.Json;

public class TsvToJsonWindow : EditorWindow
{
    static readonly Dictionary<string, Type> TypeMap = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
    {
        { "string", typeof(string) },
        { "int", typeof(int) },
        { "float", typeof(float) },
        { "bool", typeof(bool) },
        { "object", typeof(object) }
    };

    Type GetTypeFromName(string name)
    {
        if (TypeMap.TryGetValue(name, out var type))
            return type;

        return Type.GetType(name);
    }
    
    [MenuItem("Window/TSV Tools/TSV → JSON Converter")]
    public static void Open()
    {
        var win = GetWindow<TsvToJsonWindow>("TSV → JSON Converter");
        win.minSize = new Vector2(420, 180);
    }

    string tsvPath = "";
    bool useJsonLine = false;
    bool useSingleObject = false;

    void OnGUI()
    {GUILayout.Label("TSV → JSON 변환기", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        tsvPath = EditorGUILayout.TextField("TSV 파일", tsvPath);
        if (GUILayout.Button("…", GUILayout.MaxWidth(25)))
            tsvPath = EditorUtility.OpenFilePanel("TSV 파일 선택", "", "tsv");
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(5);

// 토글 1
        bool newJsonLine = EditorGUILayout.Toggle("JsonLine 모드 사용", useJsonLine);
        if (newJsonLine != useJsonLine)
        {
            useJsonLine = newJsonLine;
            if (useJsonLine) useSingleObject = false; // 동시에 선택 불가
        }

// 토글 2
        bool newAnotherMode = EditorGUILayout.Toggle("Single Object 모드 사용", useSingleObject);
        if (newAnotherMode != useSingleObject)
        {
            useSingleObject = newAnotherMode;
            if (useSingleObject) useJsonLine = false; // 동시에 선택 불가
        }

        GUILayout.Space(10);

        GUI.enabled = File.Exists(tsvPath);
        if (GUILayout.Button("JSON으로 저장", GUILayout.Height(30)))
            ConvertAndSave(tsvPath);
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
            string[] lines = File.ReadAllLines(path);

            string json = useJsonLine
                ? TsvToJsonline(lines)
                : useSingleObject 
                    ? TsvToSingleJson(lines) 
                    : TsvToJsonDefault(lines);

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

    string TsvToJsonDefault(string[] lines)
    {
        return JsonConvert.SerializeObject(StringArrayToDictionaryDefault(lines), Formatting.Indented);
    }

    public List<Dictionary<string, object>> StringArrayToDictionaryDefault(string[] lines)
    {
        if (lines.Length < 3) return new List<Dictionary<string, object>>();

        string[] headers = lines[0].Split('\t');
        var typeStrings = lines[1].Split('\t');

        var rows = new List<Dictionary<string, object>>();
        for (int i = 2; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            string[] cells = lines[i].Split('\t');

            var obj = new Dictionary<string, object>();
            for (int c = 0; c < headers.Length && c < cells.Length; c++)
            {
                if (headers[c].StartsWith("#")) continue;

                obj[headers[c]] = Cast(cells[c], typeStrings[c]);
            }

            rows.Add(obj);
        }

        return rows;
    }

    private object Cast(string origin, string typeString)
    {
        if (typeString.StartsWith("object"))
        {
            if (typeString.EndsWith("[]"))
            {
                return JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(origin);
            }
            
            return JsonConvert.DeserializeObject<Dictionary<string, object>>(origin);
        }

        if (typeString.EndsWith("[]"))
        {
            var list = new List<object>();
            var newOrigin = origin.Trim(new char[] { '[', ']' });
            var cellList = newOrigin.Split(',');

            foreach (var cell in cellList)
            {
                list.Add(Cast(cell.Trim(), typeString.Substring(0, typeString.Length - 2)));
            }

            return list;
        }
        
        if(GetTypeFromName(typeString) == null)
            Debug.Log(typeString);

        return Convert.ChangeType(origin, GetTypeFromName(typeString));
    }   

    string TsvToJsonline(string[] lines)
    {
        return JsonConvert.SerializeObject(StringArrayToDictionaryJsonline(lines), Formatting.Indented);
    }

    string TsvToSingleJson(string[] lines)
    {
        return JsonConvert.SerializeObject(new List<Dictionary<string, object>>(){ StringArrayToSingleDictionary(lines) }, Formatting.Indented);
    }
    
    List<Dictionary<string, object>> StringArrayToDictionaryJsonline(string[] lines)
    {
        if (lines.Length < 3) return new List<Dictionary<string, object>>();

        var rows = new List<Dictionary<string, object>>();
        string[] headers = null;
        string[] typeStrings = null;

        var idx = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            idx++;

            if (idx % 3 == 1)
            {
                headers = lines[i].Split('\t');
            }
            else if (idx % 3 == 2)
            {
                typeStrings = lines[i].Split('\t');
            }
            else
            {
                string[] cells = lines[i].Split('\t');
                var obj = new Dictionary<string, object>();
                for (int c = 0; c < headers.Length && c < cells.Length; c++)
                {
                    if (headers[c].StartsWith("#")) continue;
                    if (headers[c].IsNullOrEmpty()) continue;
                    
                    obj[headers[c]] = Cast(cells[c], typeStrings[c]);
                }

                rows.Add(obj);
            }
        }
        return rows;
    }
    
    Dictionary<string, object> StringArrayToSingleDictionary(string[] lines)
    {
        if (lines[0].Length < 3) return new Dictionary<string, object>();

        string[] headers = lines[0].Split('\t');

        var obj = new Dictionary<string, object>();
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            string[] cells = lines[i].Split('\t');

            string key = "";
            string type = "";
            string value = "";
            
            
            for (int c = 0; c < headers.Length && c < cells.Length; c++)
            {
                if (headers[c].StartsWith("#")) continue;

                if (headers[c].Contains("Key"))
                {
                    key = cells[c].Trim();
                }
                else if (headers[c].Contains("Type"))
                {
                    type = cells[c].Trim();
                }
                else if (headers[c].Contains("Value"))
                {
                    value = cells[c].Trim();
                }
            }
            
            obj[key] = Cast(value, type);
        }

        return obj;
    }
}
