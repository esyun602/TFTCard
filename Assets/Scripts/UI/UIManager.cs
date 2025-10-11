using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using MessageSystem;
using UnityEngine.InputSystem.UI;

public class UIManager
{
	/// <summary>
	/// 
	/// UI usecase
	/// 특정 UI의 생성
	/// -> UI 타입에 따라 constant position(screen space 기반 박혀있는 값), position 가 존재할 수 있음
	/// ---> constant position이 존재한다면 position과 무시됨
	/// ---> parent가 존재한다면 position은 screen pos 기반으로, 존재한다면 paren
	/// 
	/// -> 특정 UI 타입이 cascade되어 여러개가 생성될 수 있음
	/// -> UI Instance는 각자의 UI 종류에 따라 property를 가짐
	/// ---> UI instance의 property는 각각 Notice를 받든 하여 Update해줌
	/// 
	/// 특정 UI의 제거
	/// -> UI instance의 ID를 제공하여 제거하거나
	/// -> UI Type를 받아 일괄적 제거
	/// ---> UI Type에 따라 cascade되어 여러개가 동시에 제거될 수 있음
	/// 
	/// 특정 UI의 클릭 관리 ( 마우스 enter, exit, clickdown, clickup, drag 등등 )
	/// 특정 UI의 상태 변화 ( position, size, rotation, material 등등) ==> 있어야됨
	/// 
	/// Player는 자신의 행위에 따라 UI가 실시간으로 변화하는 것을 직접 볼 수 있어야됨
	/// ex) 버프 UI의 경우 생성, 타이머에 따른 색 변화, 제거 등의 행동을 할 수 있어야됨
	/// ex) 플레이어 메뉴 UI의 경우 생성, 제거가 구현되어 있어야됨
	/// 
	/// 모든 UI의 제거
	/// 
	/// 특정 UI의 Hide
	/// Hide된 UI의 Activation
	/// 
	/// </summary>
	private Dictionary<UIType, Canvas> canvasMap;

	private Transform UIRoot;
	private List<UIInstance> uiInstanceList;

	private Camera currentUICamera;
	public Camera CurrentUICamera
	{
		get => currentUICamera;
		set
		{
			foreach (var kvp in canvasMap)
			{
				if (kvp.Value != null)
				{
					kvp.Value.worldCamera = value;
				}
			}

			currentUICamera = value;
		} 
	}

	public UIManager()
	{
		UIRoot = new GameObject("UIRoot").transform;
		uiInstanceList = new();
		canvasMap = new();
		if (GameObject.Find(UIType.DontDestroyUI.ToString() + "Canvas") == null)
		{
			canvasMap[UIType.DontDestroyUI] = GenerateTargetCanvas(UIType.DontDestroyUI);
			Object.DontDestroyOnLoad(canvasMap[UIType.DontDestroyUI].gameObject);
		}

		if (EventSystem.current == null)
		{
			GenerateEventSystem();
		}

		//Tool.ToolBox.Instance.LateUpdateCallback += LateUpdate;
	}

	public T GenerateUI<T>(object param = null, Vector3? position = null, Quaternion? rotation = null, Vector3? scale = null, Transform followTarget = null) where T : UIInstance
	{
		var instance =
			UIInstance.Instantiate<T>(param, position, rotation, scale, followTarget, (instance) => GetTargetCanvasByType(instance.UIType).transform);
		uiInstanceList.Add(instance);
		return instance;
	}

	private Canvas GetTargetCanvasByType(UIType type)
	{
		if (canvasMap.TryGetValue(type, out var canvas) && canvas != null)
		{
			return canvas;
		}

		canvas = GenerateTargetCanvas(type);
		canvas.transform.SetParent(UIRoot, false);
		canvasMap[type] = canvas;
		return canvas;
	}

	private Canvas GenerateTargetCanvas(UIType type)
	{
		var go = new GameObject(type.ToString() + "Canvas");
		var canvas = go.AddComponent<Canvas>();
		var scalar = go.AddComponent<CanvasScaler>();
		//todo: 분리 필요?
		scalar.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
		scalar.referenceResolution = new Vector2(1920, 1080);
		go.AddComponent<GraphicRaycaster>();
		canvas.renderMode = GetRenderModeFromType(type);
		//todo: 카메라 할당 픽스
		canvas.worldCamera = CurrentUICamera;
		if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
		{
			canvas.planeDistance = Constant.StageCameraHeight;
		}
		return canvas;
	}

	private RenderMode GetRenderModeFromType(UIType type)
	{
		return (RenderMode)((int)type & 0b11);
	}

	private void GenerateEventSystem()
	{
		var go = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
		Object.DontDestroyOnLoad(go);
	}

	#region Remove

	public bool RemoveUI<T>()
	{
		for (int i = uiInstanceList.Count - 1; i >= 0; i--)
		{
			if (uiInstanceList[i].GetType() == typeof(T))
			{
				var uiInstance = uiInstanceList[i];
				uiInstanceList.RemoveAt(i);
				uiInstance.Remove();

				return true;
			}
		}

		return false;
	}

	public void RemoveAllUI<T>()
	{
		for (int i = uiInstanceList.Count - 1; i >= 0; i--)
		{
			if (uiInstanceList[i].GetType() == typeof(T))
			{
				var uiInstance = uiInstanceList[i];
				uiInstanceList.RemoveAt(i);
				uiInstance.Remove();
			}
		}
	}

	public bool RemoveUI(int id)
	{
		for (int i = uiInstanceList.Count - 1; i >= 0; i--)
		{
			if (uiInstanceList[i].Id == id)
			{
				var uiInstance = uiInstanceList[i];
				uiInstanceList.RemoveAt(i);
				uiInstance.Remove();

				return true;
			}
		}

		return false;
	}

	public bool RemoveTopUI()
	{
		if (uiInstanceList != null && uiInstanceList.Count != 0)
		{
			var uiInstance = uiInstanceList[^1];
			uiInstanceList.RemoveAt(uiInstanceList.Count - 1);
			uiInstance.Remove();

			return true;
		}

		return false;
	}

	#endregion

	#region Hide

	public bool HideUI<T>()
	{
		for (int i = uiInstanceList.Count - 1; i >= 0; i--)
		{
			if (uiInstanceList[i].GetType() == typeof(T))
			{
				uiInstanceList[i].Hide();
				return true;
			}
		}

		return false;
	}

	public void HideAllUI<T>()
	{
		for (int i = uiInstanceList.Count - 1; i >= 0; i--)
		{
			if (uiInstanceList[i].GetType() == typeof(T))
			{
				uiInstanceList[i].Hide();
			}
		}
	}

	public bool HideUI(int id)
	{
		for (int i = uiInstanceList.Count - 1; i >= 0; i--)
		{
			if (uiInstanceList[i].Id == id)
			{
				uiInstanceList[i].Hide();

				return true;
			}
		}

		return false;
	}

	#endregion

	#region Activte

	public bool ActivateUI<T>()
	{
		for (int i = uiInstanceList.Count - 1; i >= 0; i--)
		{
			if (uiInstanceList[i].GetType() == typeof(T))
			{
				uiInstanceList[i].Activate();

				return true;
			}
		}

		return false;
	}

	public void ActivateAllUI<T>()
	{
		for (int i = uiInstanceList.Count - 1; i >= 0; i--)
		{
			if (uiInstanceList[i].GetType() == typeof(T))
			{
				uiInstanceList[i].Activate();
			}
		}
	}

	public bool ActivateUI(int id)
	{
		for (int i = uiInstanceList.Count - 1; i >= 0; i--)
		{
			if (uiInstanceList[i].Id == id)
			{
				uiInstanceList[i].Activate();

				return true;
			}
		}

		return false;
	}

	#endregion

	private void LateUpdate()
	{
	}
}