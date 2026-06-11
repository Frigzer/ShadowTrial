using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class ShadowUIStyle
{
    private const float ButtonWidth = 300f;
    private const float ButtonHeight = 70f;
    private const string MenuFontName = "Shojumaru-Regular SDF";

    private static readonly Color Ink = new Color(0.035f, 0.039f, 0.051f, 0.94f);
    private static readonly Color InkSoft = new Color(0.075f, 0.082f, 0.105f, 0.92f);
    private static readonly Color Bone = new Color(0.91f, 0.86f, 0.75f, 1f);
    private static readonly Color MutedBone = new Color(0.74f, 0.69f, 0.59f, 1f);
    private static readonly Color Blood = new Color(0.48f, 0.08f, 0.075f, 1f);
    private static readonly Color BloodBright = new Color(0.68f, 0.15f, 0.11f, 1f);
    private static readonly Color Gold = new Color(0.92f, 0.63f, 0.23f, 1f);
    private static readonly Color TransparentBlack = new Color(0f, 0f, 0f, 0.68f);
    private static readonly Color OverlayBlack = new Color(0f, 0f, 0f, 0.58f);

    private static TMP_FontAsset menuFont;

    public static void StyleSceneCanvases()
    {
        Canvas[] canvases = Object.FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (Canvas canvas in canvases)
        {
            StyleRoot(canvas.gameObject);
        }
    }

    public static void StyleRoot(GameObject root)
    {
        if (root == null)
        {
            return;
        }

        foreach (Image image in root.GetComponentsInChildren<Image>(true))
        {
            StyleImage(image);
        }

        foreach (Button button in root.GetComponentsInChildren<Button>(true))
        {
            StyleButton(button);
        }

        foreach (TextMeshProUGUI text in root.GetComponentsInChildren<TextMeshProUGUI>(true))
        {
            StyleText(text);
        }

        foreach (TMP_InputField input in root.GetComponentsInChildren<TMP_InputField>(true))
        {
            StyleInput(input);
        }
    }

    public static void StylePanel(GameObject panel)
    {
        if (panel == null)
        {
            return;
        }

        Image image = panel.GetComponent<Image>();
        if (image != null)
        {
            image.color = IsFullScreenOverlay(panel) ? OverlayBlack : Ink;
        }

        RectTransform rect = panel.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.localScale = Vector3.one;
        }

        if (!IsFullScreenOverlay(panel))
        {
            AddOrUpdateOutline(panel, Gold, new Vector2(2f, -2f));
            AddOrUpdateShadow(panel, TransparentBlack, new Vector2(10f, -10f));
        }

        StyleRoot(panel);
    }

    private static void StyleImage(Image image)
    {
        if (image == null)
        {
            return;
        }

        string name = image.gameObject.name.ToLowerInvariant();

        if (name.Contains("background"))
        {
            image.color = new Color(0.025f, 0.028f, 0.036f, 1f);
            return;
        }

        if (IsFullScreenOverlay(image.gameObject))
        {
            image.color = OverlayBlack;
            return;
        }

        if (name.Contains("panel"))
        {
            image.color = Ink;
            AddOrUpdateOutline(image.gameObject, Gold, new Vector2(2f, -2f));
            AddOrUpdateShadow(image.gameObject, TransparentBlack, new Vector2(10f, -10f));
            return;
        }

        if (image.GetComponent<Button>() == null && image.GetComponent<TMP_InputField>() == null)
        {
            image.color = InkSoft;
        }
    }

    private static void StyleButton(Button button)
    {
        if (button == null)
        {
            return;
        }

        Image image = button.GetComponent<Image>();
        if (image != null)
        {
            image.color = Blood;
        }

        ColorBlock colors = button.colors;
        colors.normalColor = Blood;
        colors.highlightedColor = BloodBright;
        colors.pressedColor = Gold;
        colors.selectedColor = BloodBright;
        colors.disabledColor = new Color(0.18f, 0.16f, 0.15f, 0.65f);
        colors.colorMultiplier = 1f;
        colors.fadeDuration = 0.12f;
        button.colors = colors;

        RectTransform rect = button.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.sizeDelta = new Vector2(ButtonWidth, ButtonHeight);
        }

        AddOrUpdateOutline(button.gameObject, Gold, new Vector2(1f, -1f));
        AddOrUpdateShadow(button.gameObject, TransparentBlack, new Vector2(5f, -5f));
    }

    private static void StyleText(TextMeshProUGUI text)
    {
        if (text == null)
        {
            return;
        }

        string name = text.gameObject.name.ToLowerInvariant();
        TMP_FontAsset font = GetMenuFont();
        if (font != null)
        {
            text.font = font;
        }

        text.textWrappingMode = TextWrappingModes.Normal;
        text.overflowMode = TextOverflowModes.Overflow;

        if (name.Contains("title"))
        {
            text.color = Gold;
            text.fontStyle = FontStyles.Bold;
            text.fontSize = Mathf.Max(text.fontSize, 42f);
            text.characterSpacing = 2.5f;
            AddOrUpdateShadow(text.gameObject, TransparentBlack, new Vector2(4f, -4f));
            return;
        }

        if (text.GetComponentInParent<Button>() != null)
        {
            text.color = Bone;
            text.fontStyle = FontStyles.Bold;
            text.fontSize = Mathf.Max(text.fontSize, 24f);
            text.characterSpacing = 1.2f;
            return;
        }

        if (name.Contains("score") || name.Contains("result") || name.Contains("death") || name.Contains("time"))
        {
            text.color = Bone;
            text.fontSize = Mathf.Max(text.fontSize, 24f);
            return;
        }

        text.color = MutedBone;
        text.fontSize = Mathf.Max(text.fontSize, 20f);
    }

    private static void StyleInput(TMP_InputField input)
    {
        if (input == null)
        {
            return;
        }

        Image image = input.GetComponent<Image>();
        if (image != null)
        {
            image.color = InkSoft;
        }

        if (input.textComponent != null)
        {
            TMP_FontAsset font = GetMenuFont();
            if (font != null)
            {
                input.textComponent.font = font;
            }

            input.textComponent.color = Bone;
            input.textComponent.fontSize = Mathf.Max(input.textComponent.fontSize, 24f);
        }

        if (input.placeholder is TextMeshProUGUI placeholder)
        {
            TMP_FontAsset font = GetMenuFont();
            if (font != null)
            {
                placeholder.font = font;
            }

            placeholder.color = MutedBone;
            placeholder.fontStyle = FontStyles.Italic;
        }

        AddOrUpdateOutline(input.gameObject, Gold, new Vector2(1f, -1f));
    }

    private static bool IsFullScreenOverlay(GameObject target)
    {
        RectTransform rect = target.GetComponent<RectTransform>();
        if (rect == null)
        {
            return false;
        }

        bool stretchesAcrossParent =
            rect.anchorMin.x <= 0.01f &&
            rect.anchorMin.y <= 0.01f &&
            rect.anchorMax.x >= 0.99f &&
            rect.anchorMax.y >= 0.99f;

        if (stretchesAcrossParent)
        {
            return true;
        }

        string name = target.name.ToLowerInvariant();
        return name.Contains("overlay") || name.Contains("dim") || name.Contains("fade");
    }

    private static TMP_FontAsset GetMenuFont()
    {
        if (menuFont != null)
        {
            return menuFont;
        }

        foreach (TextMeshProUGUI text in Object.FindObjectsByType<TextMeshProUGUI>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (text.font != null && text.font.name == MenuFontName)
            {
                menuFont = text.font;
                return menuFont;
            }
        }

#if UNITY_EDITOR
        menuFont = UnityEditor.AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/Shojumaru-Regular SDF.asset");
        if (menuFont != null)
        {
            return menuFont;
        }
#endif

        return TMP_Settings.defaultFontAsset;
    }

    private static void AddOrUpdateShadow(GameObject target, Color color, Vector2 distance)
    {
        Shadow shadow = target.GetComponent<Shadow>();
        if (shadow == null)
        {
            shadow = target.AddComponent<Shadow>();
        }

        shadow.effectColor = color;
        shadow.effectDistance = distance;
        shadow.useGraphicAlpha = true;
    }

    private static void AddOrUpdateOutline(GameObject target, Color color, Vector2 distance)
    {
        Outline outline = target.GetComponent<Outline>();
        if (outline == null)
        {
            outline = target.AddComponent<Outline>();
        }

        outline.effectColor = color;
        outline.effectDistance = distance;
        outline.useGraphicAlpha = true;
    }
}
