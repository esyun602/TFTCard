using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(GridSelector))]
public class GridSelectorDrawer : PropertyDrawer
{
    private const float cellSize = 20f;
    private const float padding = 2f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // 기본 프로퍼티 드로잉 시작
        EditorGUI.BeginProperty(position, label, property);
        EditorGUILayout.Space();
        
        // 상단 레이블 (필요시)
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // GridSelector의 하위 프로퍼티 접근
        SerializedProperty rowsProp = property.FindPropertyRelative("rows");
        SerializedProperty columnsProp = property.FindPropertyRelative("columns");
        SerializedProperty cellsProp = property.FindPropertyRelative("cells");

        int rows = Mathf.Max(1, rowsProp.intValue);
        int cols = Mathf.Max(1, columnsProp.intValue);

        // 격자 전체 크기 계산
        float gridWidth = cols * (cellSize + padding) - padding;
        float gridHeight = rows * (cellSize + padding) - padding;
        // position에서 고정 크기로 Rect 생성 (강제로 gridWidth, gridHeight 사용)
        Rect gridRect = new Rect(position.x, position.y, gridWidth, gridHeight);

        // 마우스 이벤트 처리 (드래그&드롭 대신 각 셀의 오브젝트 필드를 사용하므로 별도의 클릭 처리 없이 ObjectField를 사용)
        // 각 셀을 그립니다.
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                Rect cellRect = new Rect(
                    gridRect.x + col * (cellSize + padding),
                    gridRect.y + row * (cellSize + padding),
                    cellSize,
                    cellSize
                );

                // cells 리스트에서 해당 (row, col)의 데이터를 찾습니다.
                int index = FindCellIndex(cellsProp, row, col);
                SerializedProperty cellDataProp = null;
                if (index >= 0)
                {
                    cellDataProp = cellsProp.GetArrayElementAtIndex(index);
                }
                else
                {
                    // 만약 해당 좌표의 데이터가 없다면, 새로 추가할지 선택(여기서는 기본적으로 null을 표시)
                }

                // 배경색을 그립니다.
                Color bgColor = Color.gray;
                if (cellDataProp != null)
                {
                    SerializedProperty linkedObjProp = cellDataProp.FindPropertyRelative("linkedObject");
                    if (linkedObjProp.objectReferenceValue != null)
                        bgColor = Color.green;
                }
                EditorGUI.DrawRect(cellRect, bgColor);

                // 각 셀에 오브젝트 필드를 오버레이로 표시합니다.
                EditorGUI.BeginChangeCheck();
                UnityEngine.Object newObj = EditorGUI.ObjectField(cellRect, 
                    cellDataProp != null ? cellDataProp.FindPropertyRelative("linkedObject").objectReferenceValue : null, 
                    typeof(UnityEngine.Object), true);
                if (EditorGUI.EndChangeCheck())
                {
                    // 변경된 경우, 만약 셀 데이터가 없다면 추가
                    if (cellDataProp == null)
                    {
                        cellsProp.arraySize++;
                        cellDataProp = cellsProp.GetArrayElementAtIndex(cellsProp.arraySize - 1);
                        cellDataProp.FindPropertyRelative("row").intValue = row;
                        cellDataProp.FindPropertyRelative("col").intValue = col;
                    }
                    cellDataProp.FindPropertyRelative("linkedObject").objectReferenceValue = newObj;
                    property.serializedObject.ApplyModifiedProperties();
                }
            }
        }

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
        return rows * (cellSize + padding) - padding;
    }
}
