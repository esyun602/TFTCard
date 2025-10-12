using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using MessageSystem;
using UnityEngine.InputSystem.UI;

public class UIManager
{
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

		if (type == UIType.DontDestroyUI)
		{
			canvas.sortingOrder = 5;
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