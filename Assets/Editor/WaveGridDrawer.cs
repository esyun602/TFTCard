using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(WaveGrid))]
public class WaveGridDrawer : PropertyDrawer
{
    private const float cellSize = 40f;
    private const float referenceCellSize = 20f;
    private const float padding = 2f;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // 기본 프로퍼티 드로잉 시작
        EditorGUI.BeginProperty(position, label, property);
        
        
        // 상단 레이블 (필요시)
       EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

       position.position += new Vector2(0, 20f);
       
        // GridSelector의 하위 프로퍼티 접근
        SerializedProperty rowsProp = property.FindPropertyRelative("rows");
        SerializedProperty columnsProp = property.FindPropertyRelative("columns");
        var mapData = (property.serializedObject.targetObject as StageSpec)?.MapData;
        HashSet<(int, int)> layerTiles = null;
        if (mapData != null)
        {
            (rowsProp.intValue, columnsProp.intValue) = mapData.GetRowColOfLayer("Enemy", out layerTiles);
        }

        SerializedProperty cellsProp = property.FindPropertyRelative("cells");

        int rows = Mathf.Max(1, rowsProp.intValue);
        int cols = Mathf.Max(1, columnsProp.intValue);

        // 격자 전체 크기 계산
        float gridWidth = cols * (cellSize + padding) - padding;
        float gridHeight = rows * (cellSize + padding) - padding;
        // position에서 고정 크기로 Rect 생성 (강제로 gridWidth, gridHeight 사용)
        Rect gridRect = new Rect(position.x, position.y, gridWidth, gridHeight);

        // 마우스 이벤트 처리 (드래그&드롭 대신 각 셀의 오브젝트 필드를 사용하므로 별도의 클릭 처리 없이 ObjectField를 사용)

        var dict = new Dictionary<(int, int), SerializedProperty>();
        for (int i = 0; i < cellsProp.arraySize; i++)
        {
            var cellData = cellsProp.GetArrayElementAtIndex(i);
            dict[(cellData.FindPropertyRelative("row").intValue, cellData.FindPropertyRelative("col").intValue)] =
                cellData;
        }

        List<int> toDelete = new();
        
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                Rect cellRect = new Rect(
                    gridRect.x + col * (cellSize + referenceCellSize + padding),
                    gridRect.y + (rows - 1 - row) * (cellSize + referenceCellSize + padding),
                    cellSize,
                    cellSize
                );

                dict.TryGetValue((row, col), out var cellDataProp);
/*
                if (cellDataProp != null)
                {
                    SerializedProperty linkedObjProp = cellDataProp.FindPropertyRelative("cardObject");
                    var rect = new Rect(cellRect.position.x, cellRect.position.y + cellSize, cellSize + referenceCellSize + padding, referenceCellSize);
                    EditorGUI.DrawPreviewTexture(cellRect, ((CardSpec)linkedObjProp.objectReferenceValue).cardResource.texture);
                    EditorGUI.LabelField(rect, linkedObjProp.objectReferenceValue.name);
                }
                else
                {
                    EditorGUI.DrawRect(cellRect, Color.gray);
                }
*/
                if (layerTiles?.Contains((row, col)) != true) continue;
                // 각 셀에 오브젝트 필드를 오버레이로 표시합니다.
                EditorGUI.BeginChangeCheck();
                
                var referenceRect = new Rect(cellRect.position.x + cellSize, cellRect.position.y,
                    referenceCellSize, referenceCellSize);
                UnityEngine.Object newObj = EditorGUI.ObjectField(referenceRect,
                    cellDataProp != null ? cellDataProp.FindPropertyRelative("cardObject").objectReferenceValue : null, 
                    typeof(CardSpec), true);
                
                if (EditorGUI.EndChangeCheck())
                {
                    if (newObj == null && cellDataProp != null)
                    {
                        toDelete.Add(FindCellIndex(cellsProp, row, col));
                    }
                    // 변경된 경우, 만약 셀 데이터가 없다면 추가
                    else if (newObj != null)
                    {
                        if (cellDataProp == null)
                        {
                            cellsProp.arraySize++;
                            cellDataProp = cellsProp.GetArrayElementAtIndex(cellsProp.arraySize - 1);
                            cellDataProp.FindPropertyRelative("row").intValue = row;
                            cellDataProp.FindPropertyRelative("col").intValue = col;
                        }
                        
                        cellDataProp.FindPropertyRelative("cardObject").objectReferenceValue = newObj;
                    }
                }
            }
        }

        
        //fix - object picker와 objectField 간의 control id 관련 이슈
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                Rect cellRect = new Rect(
                    gridRect.x + col * (cellSize + referenceCellSize + padding),
                    gridRect.y + (rows - 1 - row) * (cellSize + referenceCellSize + padding),
                    cellSize,
                    cellSize
                );

                dict.TryGetValue((row, col), out var cellDataProp);

                if (cellDataProp != null)
                {
                    SerializedProperty linkedObjProp = cellDataProp.FindPropertyRelative("cardObject");
                    var rect = new Rect(cellRect.position.x, cellRect.position.y + cellSize,
                        cellSize + referenceCellSize + padding, referenceCellSize);
                    EditorGUI.DrawPreviewTexture(cellRect,
                        ((CardSpec)linkedObjProp.objectReferenceValue).cardResource.texture);
                    EditorGUI.LabelField(rect, linkedObjProp.objectReferenceValue.name);
                }
                else
                {
                    EditorGUI.DrawRect(cellRect, Color.gray);
                }
            }
        }

        foreach (var idx in toDelete)
        {
            cellsProp.DeleteArrayElementAtIndex(idx);
        }
        
        property.serializedObject.ApplyModifiedProperties();
        EditorGUI.EndProperty();
    }

    // cells 리스트에서 (row, col) 좌표의 셀이 있는 인덱스를 찾는 함수
    private int FindCellIndex(SerializedProperty cellsProp, int row, int col)
    {
        for (int i = 0; i < cellsProp.arraySize; i++)
        {
            SerializedProperty element = cellsProp.GetArrayElementAtIndex(i);
            if (element.FindPropertyRelative("row").intValue == row &&
                element.FindPropertyRelative("col").intValue == col)
            {
                return i;
            }
        }
        return -1;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty rowsProp = property.FindPropertyRelative("rows");
        int rows = Mathf.Max(1, rowsProp.intValue);
        return rows * (cellSize + padding + referenceCellSize) - padding + 20f;
    }
}
