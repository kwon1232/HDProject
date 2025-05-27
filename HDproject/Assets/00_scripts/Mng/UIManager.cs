using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using WebSocketSharp;

public class UIManager : MonoBehaviour
{
    public static UIManager instance { get; private set; }
    public static Canvas outCanvas { get; private set; }
    public static int maxOrder { get; private set; }
    public static int minOrder { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        Canvas[] allCanvas = Object.FindObjectsOfType<Canvas>();
        foreach (var c in allCanvas)
        {
            if (c.sortingOrder > maxOrder) maxOrder = c.sortingOrder;
            if(c.sortingOrder < minOrder) minOrder = c.sortingOrder;
        }
    }

    /// <summary>
    /// 씬에서 이름이 canvasName인 Canvas를 찾거나,
    /// canvasName이 null이면 첫 번째 Canvas를 반환.
    /// 없으면 CreateCanvas()를 호출해 새로 생성
    /// </summary>
    public static Canvas GetCurrentSceneCanvas(string canvasName = null)
    {
        if (string.IsNullOrEmpty(canvasName))
        {
            // 캔버스 이름을 딱히 정하지 않았다면,
            Canvas canvas = Object.FindObjectOfType<Canvas>();
            // DynamicCanvas 이름 체크 후 있다면 리턴
            if (canvas != null && canvas.gameObject.name == "DynamicCanvas")
                return canvas;

            // DynamicCanvas 없다면 만들어줌
            return CreateCanvas(canvasName);
        }

        Canvas[] allCanvas = Object.FindObjectsOfType<Canvas>();
        foreach (var c in allCanvas)
        {
            if (c.sortingOrder > maxOrder) maxOrder = c.sortingOrder;
            if (c.sortingOrder < minOrder) minOrder = c.sortingOrder;
            if (c.gameObject.name == canvasName)
            {
                return c;
            }
        }
        return CreateCanvas(canvasName);
    }

    public static void RangeCanvasRatio(Canvas canvas, Vector2? referenceVector = null)
    {
        // 설정된 기본 해상도 체크 후 없다면 1920 x 1080으로 초기화
        Vector2 refResVec = referenceVector ?? new Vector2(1920, 1080);

        CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = refResVec;
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
    }

    public static void SetUIParent(
        GameObject parent, GameObject child,
        Vector2? childLocalPos = null, Vector2?
        anchorMin = null, Vector2? anchorMax = null, Vector2? anchoredPos = null,
        Vector3? localScale = null, Quaternion? localRotation = null
        )
    {
        Vector2 localPos = childLocalPos ?? new Vector2(0, 0);

        child.transform.SetParent(parent.transform, false);
        
        // RectTransform 초기화
        RectTransform rt = child.GetComponent<RectTransform>();
        if(rt != null)
        {
            // 앵커를 중앙에 둘지, 스트레치로 둘지 필요에 따라 조정
            rt.anchorMin = anchorMin ?? new Vector2(0.5f, 0.5f);
            rt.anchorMax = anchorMax ?? new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = anchoredPos ?? Vector2.zero;
            rt.localScale = localScale ?? Vector3.one;
            rt.localRotation = localRotation ?? Quaternion.identity;
        }
    }

    /// <summary>
    /// 씬에서 이름이 canvasName인 Canvas생성
    /// canvasName이 없다면 Canvas로 name으로 설정 
    /// </summary>
    private static Canvas CreateCanvas(string canvasName = null)
    {
        canvasName = string.IsNullOrEmpty(canvasName) ? "DynamicCanvas" : canvasName;

        GameObject canvas = new GameObject(
            canvasName,
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster)
        );

        outCanvas = canvas.GetComponent<Canvas>();
        outCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

        // Canvas 일정 비율 유지
        RangeCanvasRatio(outCanvas);

        return outCanvas;
    }
}
