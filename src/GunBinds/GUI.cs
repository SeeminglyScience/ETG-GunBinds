// Ripped from https://modworkshop.net/mod/23701

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GunBinds;

public static class GUI
{
    public static void Init()
    {
        GUIController = new GameObject("GUIController").transform;
        Object.DontDestroyOnLoad(GUIController.gameObject);
        CreateCanvas();
        GUIRoot = m_canvas.transform;
        GUIRoot.SetParent(GUIController);
    }

    public static void CreateCanvas()
    {
        GameObject gameObject = new("Canvas");
        Object.DontDestroyOnLoad(gameObject);
        m_canvas = gameObject.AddComponent<Canvas>();
        m_canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        m_canvas.sortingOrder = 100000;
        gameObject.AddComponent<CanvasScaler>();
        gameObject.AddComponent<GraphicRaycaster>();
    }

    public static Text CreateText(Transform parent, Vector2 offset, string text, TextAnchor anchor = TextAnchor.MiddleCenter, int font_size = 20)
    {
        GameObject gameObject = new("Text");
        gameObject.transform.SetParent(parent ?? GUIRoot);
        RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
        rectTransform.SetTextAnchor(anchor);
        rectTransform.anchoredPosition = offset;
        Text text2 = gameObject.AddComponent<Text>();
        text2.horizontalOverflow = HorizontalWrapMode.Overflow;
        text2.verticalOverflow = VerticalWrapMode.Overflow;
        text2.alignment = anchor;
        text2.text = text;
        text2.font = ResourceManager.LoadAssetBundle("shared_auto_001").LoadAsset<Font>("04B_03__");
        text2.fontSize = font_size;
        text2.color = defaultTextColor;
        return text2;
    }

    public static void SetTextAnchor(this RectTransform r, TextAnchor anchor)
    {
        r.anchorMin = AnchorMap[anchor];
        r.anchorMax = AnchorMap[anchor];
        r.pivot = AnchorMap[anchor];
    }
    public static Font font;

    public static Transform GUIRoot;

    public static Transform GUIController;

    private static Canvas m_canvas;

    public static readonly Dictionary<TextAnchor, Vector2> AnchorMap = new()
    {
        {
            TextAnchor.LowerLeft,
            new Vector2(0f, 0f)
        },
        {
            TextAnchor.LowerCenter,
            new Vector2(0.5f, 0f)
        },
        {
            TextAnchor.LowerRight,
            new Vector2(1f, 0f)
        },
        {
            TextAnchor.MiddleLeft,
            new Vector2(0f, 0.5f)
        },
        {
            TextAnchor.MiddleCenter,
            new Vector2(0.5f, 0.5f)
        },
        {
            TextAnchor.MiddleRight,
            new Vector2(1f, 0.5f)
        },
        {
            TextAnchor.UpperLeft,
            new Vector2(0f, 1f)
        },
        {
            TextAnchor.UpperCenter,
            new Vector2(0.5f, 1f)
        },
        {
            TextAnchor.UpperRight,
            new Vector2(1f, 1f)
        }
    };

    private static Color defaultTextColor = new(1f, 1f, 1f, 0.5f);
}
