using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager
{
    private int _order = 10;

    private Stack<UI_Popup> _popupStack = new Stack<UI_Popup>();

    private UI_Scene _sceneUI = null;

    public UI_Scene SceneUI
    {
        set { _sceneUI = value; }
        get { return _sceneUI; }
    }

    public GameObject Root
    {
        get
        {
            GameObject root = GameObject.Find("@UI_Root");
            if (root == null)
            {
                root = new GameObject() { name = "@UI_Root" };
            }

            return root;
        }
    }

    public void SetCanvas(GameObject go, bool sort = true, int sortOrder = 0)
    {
        Canvas canvas = Util.GetOrAddComponent<Canvas>(go);
        if (canvas != null)
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.overrideSorting = true;
        }

        CanvasScaler cs = go.GetOrAddComponent<CanvasScaler>();
        if (cs != null)
        {
            cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            cs.referenceResolution = new Vector2(1080, 1920);
        }

        go.GetOrAddComponent<GraphicRaycaster>();

        if (sort)
        {
            canvas.sortingOrder = _order;
            _order++;
        }
        else
        {
            canvas.sortingOrder = sortOrder;
        }
    }

    public T MakeSubItem<T>(Transform parent = null, string name = null, bool pooling = true) where T : UI_Base
    {
        if (string.IsNullOrEmpty(name))
        {
            name = typeof(T).Name;
        }

        GameObject go = Managers.Resource.Instantiate(name, parent, pooling);
        go.transform.SetParent(parent);

        return Util.GetOrAddComponent<T>(go);
    }

    public T ShowBaseUI<T>(string name = null) where T : UI_Base
    {
        if (string.IsNullOrEmpty(name))
        {
            name = typeof(T).Name;
        }

        GameObject go = Managers.Resource.Instantiate(name);
        T baseUI = Util.GetOrAddComponent<T>(go);
        
        go.transform.SetParent(Root.transform);
        return baseUI;
    }

    public T ShowSceneUI<T>(string name = null) where T : UI_Scene
    {
        if (string.IsNullOrEmpty(name))
        {
            name = typeof(T).Name;
        }

        GameObject go = Managers.Resource.Instantiate(name);
        T sceneUI = Util.GetOrAddComponent<T>(go);
        _sceneUI = sceneUI;
        
        go.transform.SetParent(Root.transform);
        return sceneUI;
    }
}
