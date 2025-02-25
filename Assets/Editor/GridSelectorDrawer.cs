using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(GridSelector))]
public class GridSelectorDrawer : PropertyDrawer
{
	// 격자 셀의 크기 및 패딩 (픽셀 단위)
	private const float cellSize = 20f;
	private const float padding = 2f;

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		// 프로퍼티 표시 시작
		EditorGUI.BeginProperty(position, label, property);

		// 상단 레이블 그리기 (옵션)
		property.isExpanded =
			EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
				property.isExpanded, label);

		if (!property.isExpanded)
		{
			EditorGUI.EndProperty();
			return;
		}

		position.position += new Vector2(0, 20f);

		EditorGUI.BeginChangeCheck();
		property.FindPropertyRelative("isAbsolute").boolValue = EditorGUI.Toggle(
			new Rect(position.position, new Vector2(20, 20)), "IsAbsolute",
			property.FindPropertyRelative("isAbsolute").boolValue);
		position.position += new Vector2(0, 20f);

		if (GUI.Button(new Rect(position.position, new Vector2(100, 20)), "SizeUp"))
		{
			property.FindPropertyRelative("rows").intValue += 2;
			property.FindPropertyRelative("columns").intValue += 2;
		}

		position.position += new Vector2(0, 20f);

		SerializedProperty selectedCellsProp = property.FindPropertyRelative("selectedCells");

		if (GUI.Button(new Rect(position.position, new Vector2(100, 20)), "SizeDown"))
		{
			property.FindPropertyRelative("rows").intValue =
				Mathf.Max(3, property.FindPropertyRelative("rows").intValue - 2);
			property.FindPropertyRelative("columns").intValue =
				Mathf.Max(3, property.FindPropertyRelative("columns").intValue - 2);

			for (int i = selectedCellsProp.arraySize - 1; i >= 0; i--)
			{
				SerializedProperty element = selectedCellsProp.GetArrayElementAtIndex(i);
				if (element.FindPropertyRelative("row").intValue + property.FindPropertyRelative("rows").intValue / 2 >=
				    property.FindPropertyRelative("rows").intValue ||
				    element.FindPropertyRelative("col").intValue +
				    property.FindPropertyRelative("columns").intValue / 2 >=
				    property.FindPropertyRelative("columns").intValue ||
				    element.FindPropertyRelative("row").intValue + property.FindPropertyRelative("rows").intValue / 2 <
				    0 ||
				    element.FindPropertyRelative("col").intValue +
				    property.FindPropertyRelative("columns").intValue / 2 < 0)
				{
					selectedCellsProp.DeleteArrayElementAtIndex(i);
				}
			}
		}

		position.position += new Vector2(0, 20f);
		if (EditorGUI.EndChangeCheck())
		{
			property.serializedObject.ApplyModifiedProperties();
		}

		SerializedProperty rowsProp = property.FindPropertyRelative("rows");
		SerializedProperty columnsProp = property.FindPropertyRelative("columns");

		int rows = Mathf.Max(1, rowsProp.intValue);
		int cols = Mathf.Max(1, columnsProp.intValue);

		// 고정된 gridWidth, gridHeight를 계산합니다.
		float gridWidth = cols * (cellSize + padding) - padding;
		float gridHeight = rows * (cellSize + padding) - padding;

		// position에서 원하는 크기로 gridRect를 강제 설정 (좌표는 position.x, position.y 사용)
		Rect gridRect = new Rect(position.x, position.y, gridWidth, gridHeight);

		// 마우스 이벤트 처리: gridRect 내부를 클릭하면 해당 셀의 선택 상태 토글
		Event e = Event.current;
		if (e.type == EventType.MouseDown && e.button == 0 && gridRect.Contains(e.mousePosition))
		{
			int clickedCol = (int)((e.mousePosition.x - gridRect.x) / (cellSize + padding));
			int clickedRow = rows - 1 - (int)((e.mousePosition.y - gridRect.y) / (cellSize + padding));

			// 클릭한 셀 좌표
			GridCell clickedCell = new GridCell(clickedRow, clickedCol);

			// selectedCells 배열 내에서 해당 셀이 이미 있는지 확인
			bool found = false;
			int removeIndex = -1;
			for (int i = 0; i < selectedCellsProp.arraySize; i++)
			{
				SerializedProperty element = selectedCellsProp.GetArrayElementAtIndex(i);
				if (element.FindPropertyRelative("row").intValue + rows / 2 == clickedRow &&
				    element.FindPropertyRelative("col").intValue + cols / 2 == clickedCol)
				{
					found = true;
					removeIndex = i;
					break;
				}
			}

			if (found)
			{
				selectedCellsProp.DeleteArrayElementAtIndex(removeIndex);
			}
			else
			{
				selectedCellsProp.arraySize++;
				SerializedProperty newElement =
					selectedCellsProp.GetArrayElementAtIndex(selectedCellsProp.arraySize - 1);
				newElement.FindPropertyRelative("row").intValue = clickedRow - rows / 2;
				newElement.FindPropertyRelative("col").intValue = clickedCol - cols / 2;
			}

			e.Use();
			property.serializedObject.ApplyModifiedProperties();
		}

	
		var selectedCache = new HashSet<(int, int)>();

		for (int i = 0; i < selectedCellsProp.arraySize; i++)
		{
			SerializedProperty element = selectedCellsProp.GetArrayElementAtIndex(i);
			selectedCache.Add((element.FindPropertyRelative("row").intValue + rows / 2,
				element.FindPropertyRelative("col").intValue + cols / 2));
		}

		// 격자 그리기
		for (int row = 0; row < rows; row++)
		{
			for (int col = 0; col < cols; col++)
			{
				Rect cellRect = new Rect(
					gridRect.x + col * (cellSize + padding),
					gridRect.y + (rows - 1 - row) * (cellSize + padding),
					cellSize,
					cellSize
				);

				// 현재 셀이 선택되었는지 확인
				bool selected = selectedCache.Contains((row, col));

				// 셀 색상: 선택되면 green, 아니면 gray
				EditorGUI.DrawRect(cellRect,
					col == cols / 2 && row == rows / 2 ? selected ? Color.green : Color.yellow :
					selected ? Color.green : Color.gray);
			}
		}


		EditorGUI.EndProperty();
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		if (!property.isExpanded)
		{
			// Foldout이 닫혔을 때, 기본 높이만 반환
			return EditorGUIUtility.singleLineHeight + padding;
		}
		else
		{
			SerializedProperty rowsProp = property.FindPropertyRelative("rows");
			int rows = Mathf.Max(1, rowsProp.intValue);
			return rows * (cellSize + padding) - padding + 80f + EditorGUIUtility.singleLineHeight;
		}
	}
}