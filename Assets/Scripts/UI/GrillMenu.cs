using System.Collections;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.Rendering.DebugUI;

public class GrillMenu : MonoBehaviour,
    
    IPointerEnterHandler,
    IPointerExitHandler
{
    [SerializeField] RectTransform grillPanel;
    [SerializeField] RectTransform buttonPanel;
    [SerializeField] RectTransform lidImage;
    [SerializeField] float offsetPercent = 0.9f;

    [SerializeField] RectTransform openMarker;
    [SerializeField] RectTransform closedMarker;

    GameManager gameManager;
    Vector2 closedPos;
    Vector2 openPos;

    bool isOpen;

    void Awake()
    {
        openPos = openMarker.anchoredPosition;
        closedPos = closedMarker.anchoredPosition;

        grillPanel.anchoredPosition = closedPos;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isOpen)
            Open();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isOpen)
            Close();
    }

    IEnumerator MovePanel(Vector2 target)
    {
        Vector2 start = grillPanel.anchoredPosition;

        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * 5f;

            float easing = 1f - Mathf.Pow(1f - t, 2f);

            grillPanel.anchoredPosition = Vector2.Lerp(start, target, easing);

            yield return null;
        }
    }

    public void Open()
    {
        isOpen = true;
        StartCoroutine(MovePanel(openPos));

        //Debug.Log("Opening");
    }

    public void Close()
    {
        isOpen = false;
        StartCoroutine(MovePanel(closedPos));
        //Debug.Log("Closing");
    }
}
