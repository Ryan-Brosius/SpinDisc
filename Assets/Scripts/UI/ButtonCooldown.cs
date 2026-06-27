using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ButtonCooldown : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Button myButton;
    [SerializeField] private Image fillImage;

    [Header("Settings")]
    [SerializeField] private float duration = 5f;

    private void Awake()
    {
        if (myButton == null) myButton = GetComponent<Button>();
        if (fillImage == null) fillImage = GetComponent<Image>();
    }

    public void OnButtonClick()
    {
        StartCoroutine(AnimateButtonFill());
    }

    private IEnumerator AnimateButtonFill()
    {
        myButton.interactable = false;

        fillImage.fillAmount = 0f;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            fillImage.fillAmount = Mathf.Clamp01(elapsedTime / duration);

            yield return null;
        }

        fillImage.fillAmount = 1f;
        myButton.interactable = true;
    }
}
