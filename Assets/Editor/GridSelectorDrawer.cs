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

		var enemyView = property.FindPropertyRelative("isEnemyView").boolValue;
		if (GUI.Button(new Rect(position.position, new Vector2(100, 20)), "SwapView"))
		{
			enemyView = !enemyView;
			property.FindPropertyRelative("isEnemyView").boolValue = enemyView;
		}

		position.position += new Vector2(0, 30f);
		if (EditorGUI.EndChangeCheck())
		{
			property.serializedObject.ApplyModifiedProperties();
		}
		
		SerializedProperty columnsProp = property.FindPropertyRelative("columns");
		//todo: fix
		columnsProp.intValue = 4;
		
		int cols = Mathf.Max(1, columnsProp.intValue);

		// 고정된 gridWidth, gridHeight를 계산합니다.
		float gridWidth = cols * 2 * (cellSize + padding) - padding;
		float gridHeight = (cellSize + padding) - padding;

		// position에서 원하는 크기로 gridRect를 강제 설정 (좌표는 position.x, position.y 사용)
		Rect gridRect = new Rect(position.x, position.y, gridWidth, gridHeight);

		SerializedProperty triggerCellListProp = property.FindPropertyRelative("triggerCellList");
		SerializedProperty attackCellListProp = property.FindPropertyRelative("attackCellList");

		// 마우스 이벤트 처리: gridRect 내부를 클릭하면 해당 셀의 선택 상태 토글
		Event e = Event.current;
		if (e.type == EventType.MouseDown && e.button == 0 && gridRect.Contains(e.mousePosition))
		{
			// 0 ~ 2cols-1
			int clickedCol = (int)((e.mousePosition.x - gridRect.x) / (cellSize + padding));

			// selectedCells 배열 내에서 해당 셀이 이미 있는지 확인
			bool found = false;
			int removeIndex = -1;
			SerializedProperty targetProp;
			if (enemyView)
			{
				targetProp = clickedCol >= cols ? triggerCellListProp : attackCellListProp;
			}
			else
			{
				targetProp = clickedCol >= cols ? attackCellListProp : triggerCellListProp;
			}

			clickedCol = clickedCol >= cols ? cols - 1 - (clickedCol % cols) : clickedCol % cols;
			
			for (int i = 0; i < targetProp.arraySize; i++)
			{
				SerializedProperty element = targetProp.GetArrayElementAtIndex(i);
				if (element.intValue == clickedCol)
				{
					found = true;
					removeIndex = i;
					break;
				}
			}

			if (found)
			{
				targetProp.DeleteArrayElementAtIndex(removeIndex);
			}
			else
			{
				targetProp.arraySize++;
				SerializedProperty newElement =
					targetProp.GetArrayElementAtIndex(targetProp.arraySize - 1);
				newElement.intValue = clickedCol;
			}

			e.Use();
			property.serializedObject.ApplyModifiedProperties();
		}
		
		if (enemyView)
		{
			DrawProperty(attackCellListProp, 0, false, Color.red);
			DrawProperty(triggerCellListProp, cols + 0.2f, true, Color.green);
		}
		else
		{
			DrawProperty(triggerCellListProp, 0, false, Color.green);
			DrawProperty(attackCellListProp, cols + 0.2f, true, Color.red);
		}
		
		void DrawProperty(SerializedProperty target, float offset, bool isReverse, Color targetColor)
		{
			var selectedCache = new HashSet<int>();
			
			for (int i = 0; i < target.arraySize; i++)
			{
				SerializedProperty element = target.GetArrayElementAtIndex(i);
				selectedCache.Add(element.intValue);
			}
				
			//todo: trigger칸은 col*2로 확장할 필요가 있을 수도
			for (int col = 0; col < cols; col++)
			{
				var targetX = gridRect.x + (isReverse ? cols - 1 - col + offset : col + offset) * (cellSize + padding);
				Rect cellRect = new Rect(
					targetX,
					gridRect.y,
					cellSize,
					cellSize
				);

				// 현재 셀이 선택되었는지 확인
				bool selected = selectedCache.Contains(col);

				// 셀 색상: 선택되면 green, 아니면 gray
				EditorGUI.DrawRect(cellRect, selected ? targetColor : Color.gray );
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