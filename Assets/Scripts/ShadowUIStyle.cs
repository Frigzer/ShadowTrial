using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class ShadowUIStyle
{
    private const string MenuFontName = "Shojumaru-Regular SDF";
    private const string FrameRootName = "ShadowUIFrame";

    private static readonly Color Ink = new Color(0.035f, 0.039f, 0.051f, 0.94f);
    private static readonly Color InkSoft = new Color(0.075f, 0.082f, 0.105f, 0.92f);
    private static readonly Color Bone = new Color(0.91f, 0.86f, 0.75f, 1f);
    private static readonly Color MutedBone = new Color(0.74f, 0.69f, 0.59f, 1f);
    private static readonly Color Blood = new Color(0.48f, 0.08f, 0.075f, 1f);
    private static readonly Color BloodBright = new Color(0.68f, 0.15f, 0.11f, 1f);
    private static readonly Color Gold = new Color(0.92f, 0.63f, 0.23f, 1f);
    private static readonly Color FrameGold = new Color(0.98f, 0.70f, 0.24f, 1f);
    private static readonly Color FrameBrown = new Color(0.28f, 0.14f, 0.055f, 1f);
    private static readonly Color TransparentBlack = new Color(0f, 0f, 0f, 0.68f);
    private static readonly Color SolidBlack = new Color(0f, 0f, 0f, 1f);

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
            if (IsDeathOverlay(panel))
            {
                image.color = SolidBlack;
            }
            else if (!IsFullScreenOverlay(panel))
            {
                image.color = Ink;
            }
        }

        if (!IsFullScreenOverlay(panel))
        {
            AddOrUpdateOutline(panel, Gold, new Vector2(2f, -2f));
            RemoveShadow(panel);
            AddOrUpdateFrame(panel);
        }

        StyleRoot(panel);

        if (IsPauseOrFinishPanel(panel))
        {
            StyleModalFrames(panel);
        }
    }

    public static void StyleHud(TextMeshProUGUI deathsText, TextMeshProUGUI timeText)
    {
        GameObject hudPanel = FindSharedParent(deathsText, timeText);
        if (hudPanel != null)
        {
            Image panelImage = hudPanel.GetComponent<Image>();
            if (panelImage != null)
            {
                panelImage.color = new Color(0.035f, 0.039f, 0.051f, 0.82f);
            }

            AddOrUpdateOutline(hudPanel, Gold, new Vector2(1f, -1f));
            RemoveShadow(hudPanel);
            AddOrUpdateFrame(hudPanel);
        }

        StyleHudText(deathsText);
        StyleHudText(timeText);
    }

    private static void StyleImage(Image image)
    {
        if (image == null)
        {
            return;
        }

        if (IsFrameElement(image.gameObject))
        {
            return;
        }

        string name = image.gameObject.name.ToLowerInvariant();

        if (name.Contains("background"))
        {
            image.color = new Color(0.025f, 0.028f, 0.036f, 1f);
            return;
        }

        if (IsDeathOverlay(image.gameObject))
        {
            image.color = SolidBlack;
            return;
        }

        if (IsFullScreenOverlay(image.gameObject))
        {
            return;
        }

        if (name.Contains("panel") || name.Contains("window"))
        {
            image.color = Ink;
            AddOrUpdateOutline(image.gameObject, Gold, new Vector2(2f, -2f));
            RemoveShadow(image.gameObject);
            AddOrUpdateFrame(image.gameObject);
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

        if (IsHudText(text))
        {
            text.color = Bone;
            text.fontStyle = FontStyles.Bold;
            text.characterSpacing = 0.5f;
            text.alignment = TextAlignmentOptions.Left;
            text.textWrappingMode = TextWrappingModes.NoWrap;
            text.overflowMode = TextOverflowModes.Overflow;
            return;
        }

        if (IsLeaderboardScoresText(text))
        {
            text.color = Bone;
            text.fontStyle = FontStyles.Normal;
            text.characterSpacing = 0f;
            text.textWrappingMode = TextWrappingModes.NoWrap;
            text.overflowMode = TextOverflowModes.Overflow;

            return;
        }

        text.textWrappingMode = TextWrappingModes.Normal;
        text.overflowMode = TextOverflowModes.Overflow;

        if (name.Contains("title"))
        {
            text.color = Gold;
            text.fontStyle = FontStyles.Bold;
            text.characterSpacing = 2.5f;
            AddOrUpdateShadow(text.gameObject, TransparentBlack, new Vector2(4f, -4f));
            return;
        }

        if (text.GetComponentInParent<Button>() != null)
        {
            text.color = Bone;
            text.fontStyle = FontStyles.Bold;
            text.characterSpacing = 1.2f;
            return;
        }

        if (name.Contains("score") || name.Contains("result") || name.Contains("death") || name.Contains("time"))
        {
            text.color = Bone;
            return;
        }

        text.color = MutedBone;
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

    private static void StyleHudText(TextMeshProUGUI text)
    {
        if (text == null)
        {
            return;
        }

        TMP_FontAsset font = GetMenuFont();
        if (font != null)
        {
            text.font = font;
        }

        text.color = Bone;
        text.fontStyle = FontStyles.Bold;
        text.characterSpacing = 0.5f;
        text.alignment = TextAlignmentOptions.Left;
        text.textWrappingMode = TextWrappingModes.NoWrap;
        text.overflowMode = TextOverflowModes.Overflow;
    }

    private static GameObject FindSharedParent(TextMeshProUGUI first, TextMeshProUGUI second)
    {
        if (first == null || second == null)
        {
            return null;
        }

        Transform current = first.transform.parent;
        while (current != null)
        {
            if (second.transform.IsChildOf(current))
            {
                return current.gameObject;
            }

            current = current.parent;
        }

        return first.transform.parent != null ? first.transform.parent.gameObject : null;
    }

    private static bool IsHudText(TextMeshProUGUI text)
    {
        string name = text.gameObject.name.ToLowerInvariant();
        if (name == "deathstext" || name == "timetext")
        {
            return true;
        }

        Transform parent = text.transform.parent;
        return parent != null && parent.name.ToLowerInvariant().Contains("hud");
    }

    private static bool IsLeaderboardPanel(GameObject target)
    {
        return target.name.ToLowerInvariant().Contains("leaderboardpanel");
    }

    private static bool IsPauseOrFinishPanel(GameObject target)
    {
        string name = target.name.ToLowerInvariant();
        return name.Contains("pausepanel") || name.Contains("finishpanel");
    }

    private static void StyleModalFrames(GameObject root)
    {
        foreach (Image image in root.GetComponentsInChildren<Image>(true))
        {
            if (!IsModalFrameCandidate(image))
            {
                continue;
            }

            image.color = Ink;
            AddOrUpdateOutline(image.gameObject, Gold, new Vector2(3f, -3f));
            RemoveShadow(image.gameObject);
            AddOrUpdateFrame(image.gameObject);
        }
    }

    private static bool IsModalFrameCandidate(Image image)
    {
        if (image == null || IsFrameElement(image.gameObject) || image.GetComponent<Button>() != null || image.GetComponent<TMP_InputField>() != null)
        {
            return false;
        }

        if (IsFullScreenOverlay(image.gameObject) || IsDeathOverlay(image.gameObject))
        {
            return false;
        }

        RectTransform rect = image.GetComponent<RectTransform>();
        if (rect == null)
        {
            return false;
        }

        return rect.rect.width >= 320f && rect.rect.height >= 220f;
    }

    private static bool IsLeaderboardScoresText(TextMeshProUGUI text)
    {
        string name = text.gameObject.name.ToLowerInvariant();
        return name.Contains("scorestext") || name.Contains("scoreslist");
    }

    private static bool IsLeaderboardButton(Button button)
    {
        string name = button.gameObject.name.ToLowerInvariant();
        return name.Contains("closeleaderboard") ||
               name.Contains("clearscores") ||
               name.Contains("clearrecords") ||
               HasParentNamed(button.transform, "leaderboardpanel");
    }

    private static bool IsLeaderboardButtonText(TextMeshProUGUI text)
    {
        Button parentButton = text.GetComponentInParent<Button>();
        return parentButton != null && IsLeaderboardButton(parentButton);
    }

    private static bool HasParentNamed(Transform transform, string partialName)
    {
        Transform current = transform.parent;
        while (current != null)
        {
            if (current.name.ToLowerInvariant().Contains(partialName))
            {
                return true;
            }

            current = current.parent;
        }

        return false;
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

    private static bool IsDeathOverlay(GameObject target)
    {
        return target.name.ToLowerInvariant().Contains("deathpanel");
    }

    private static bool IsFrameElement(GameObject target)
    {
        if (target == null)
        {
            return false;
        }

        return target.name.StartsWith(FrameRootName) ||
               HasParentNamed(target.transform, FrameRootName.ToLowerInvariant());
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

    private static void RemoveShadow(GameObject target)
    {
        if (target == null)
        {
            return;
        }

        Shadow shadow = target.GetComponent<Shadow>();
        if (shadow == null)
        {
            return;
        }

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            Object.DestroyImmediate(shadow);
            return;
        }
#endif

        Object.Destroy(shadow);
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
        outline.useGraphicAlpha = false;
    }

    private static void AddOrUpdateFrame(GameObject target)
    {
        if (target == null || IsFrameElement(target) || IsFullScreenOverlay(target) || IsDeathOverlay(target))
        {
            return;
        }

        RectTransform targetRect = target.GetComponent<RectTransform>();
        if (targetRect == null)
        {
            return;
        }

        Transform existingFrame = target.transform.Find(FrameRootName);
        RectTransform frameRect;
        if (existingFrame == null)
        {
            GameObject frameObject = new GameObject(FrameRootName, typeof(RectTransform));
            frameObject.layer = target.layer;
            frameRect = frameObject.GetComponent<RectTransform>();
            frameRect.SetParent(target.transform, false);
        }
        else
        {
            frameRect = existingFrame.GetComponent<RectTransform>();
        }

        if (frameRect == null)
        {
            return;
        }

        frameRect.anchorMin = Vector2.zero;
        frameRect.anchorMax = Vector2.one;
        frameRect.pivot = new Vector2(0.5f, 0.5f);
        frameRect.offsetMin = Vector2.zero;
        frameRect.offsetMax = Vector2.zero;
        frameRect.localScale = Vector3.one;
        frameRect.SetAsLastSibling();

        AddOrUpdateFrameStrip(frameRect, "ShadowUIFrame_OuterTop", FrameBrown, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), Vector2.zero, new Vector2(0f, 7f));
        AddOrUpdateFrameStrip(frameRect, "ShadowUIFrame_OuterBottom", FrameBrown, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0.5f, 0f), Vector2.zero, new Vector2(0f, 7f));
        AddOrUpdateFrameStrip(frameRect, "ShadowUIFrame_OuterLeft", FrameBrown, new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(0f, 0.5f), Vector2.zero, new Vector2(7f, 0f));
        AddOrUpdateFrameStrip(frameRect, "ShadowUIFrame_OuterRight", FrameBrown, new Vector2(1f, 0f), new Vector2(1f, 1f), new Vector2(1f, 0.5f), Vector2.zero, new Vector2(7f, 0f));

        RemoveFrameStrip(frameRect, "ShadowUIFrame_CornerTopLeft");
        RemoveFrameStrip(frameRect, "ShadowUIFrame_CornerTopRight");
        RemoveFrameStrip(frameRect, "ShadowUIFrame_CornerBottomLeft");
        RemoveFrameStrip(frameRect, "ShadowUIFrame_CornerBottomRight");

        AddOrUpdateFrameStrip(frameRect, "ShadowUIFrame_InnerTop", FrameGold, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -3f), new Vector2(-6f, 2f));
        AddOrUpdateFrameStrip(frameRect, "ShadowUIFrame_InnerBottom", FrameGold, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 3f), new Vector2(-6f, 2f));
        AddOrUpdateFrameStrip(frameRect, "ShadowUIFrame_InnerLeft", FrameGold, new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(0f, 0.5f), new Vector2(3f, 0f), new Vector2(2f, -6f));
        AddOrUpdateFrameStrip(frameRect, "ShadowUIFrame_InnerRight", FrameGold, new Vector2(1f, 0f), new Vector2(1f, 1f), new Vector2(1f, 0.5f), new Vector2(-3f, 0f), new Vector2(2f, -6f));
    }

    private static void AddOrUpdateFrameStrip(
        RectTransform parent,
        string stripName,
        Color color,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 pivot,
        Vector2 anchoredPosition,
        Vector2 sizeDelta)
    {
        Transform existingStrip = parent.Find(stripName);
        Image image;
        bool created = existingStrip == null;

        if (created)
        {
            GameObject stripObject = new GameObject(stripName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            stripObject.layer = parent.gameObject.layer;
            RectTransform stripRect = stripObject.GetComponent<RectTransform>();
            stripRect.SetParent(parent, false);
            image = stripObject.GetComponent<Image>();
        }
        else
        {
            image = existingStrip.GetComponent<Image>();
        }

        if (image == null)
        {
            return;
        }

        RectTransform rect = image.GetComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = pivot;
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = sizeDelta;
        rect.localScale = Vector3.one;

        image.raycastTarget = false;
        if (created || image.color.a < 0.99f)
        {
            image.color = color;
        }
    }

    private static void RemoveFrameStrip(RectTransform parent, string stripName)
    {
        Transform existingStrip = parent.Find(stripName);
        if (existingStrip == null)
        {
            return;
        }

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            Object.DestroyImmediate(existingStrip.gameObject);
            return;
        }
#endif

        Object.Destroy(existingStrip.gameObject);
    }
}
