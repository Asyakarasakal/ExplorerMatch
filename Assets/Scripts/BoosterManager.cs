using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class BoosterManager : MonoBehaviour
{
    public static BoosterManager Instance;

    [Header("Booster Amounts")]
    public int defaultBoosterAmount = 3;

    [Header("UI Elements - Texts")]
    public TMP_Text hintCountText;
    public TMP_Text freezeCountText;
    public TMP_Text undoCountText;

    [Header("UI Elements - Buttons")]
    public Button hintButton;
    public Button freezeButton;
    public Button undoButton;

    [Header("Particle FX")]
    public GameObject hintSparklePrefab;

    private int hintCount;
    private int freezeCount;
    private int undoCount;

    private bool isFrozen = false;
    private bool isHintActive = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        LoadBoosterCounts();
        UpdateUI();
    }

    public void LoadBoosterCounts()
    {
        hintCount = PlayerPrefs.GetInt("Booster_Hint", defaultBoosterAmount);
        freezeCount = PlayerPrefs.GetInt("Booster_Freeze", defaultBoosterAmount);

        if (PlayerPrefs.HasKey("Booster_Magnet") && !PlayerPrefs.HasKey("Booster_Undo"))
        {
            undoCount = PlayerPrefs.GetInt("Booster_Magnet", defaultBoosterAmount);
            SaveBoosterCount("Booster_Undo", undoCount);
        }
        else
        {
            undoCount = PlayerPrefs.GetInt("Booster_Undo", defaultBoosterAmount);
        }
    }

    public void UpdateUI()
    {
        if (hintCountText != null) hintCountText.text = hintCount.ToString();
        if (freezeCountText != null) freezeCountText.text = freezeCount.ToString();
        if (undoCountText != null) undoCountText.text = undoCount.ToString();
    }

    public void UseHintBooster()
    {
        if (Time.timeScale == 0 || (LoseManager.Instance != null && LoseManager.Instance.IsGameOver) || isHintActive) return;

        if (hintCount > 0)
        {
            ExecuteHintLogic();
        }
    }

    public void UseFreezeBooster()
    {
        if (Time.timeScale == 0 || (LoseManager.Instance != null && LoseManager.Instance.IsGameOver)) return;

        if (freezeCount > 0 && !isFrozen)
        {
            freezeCount--;
            SaveBoosterCount("Booster_Freeze", freezeCount);
            UpdateUI();

            StartCoroutine(ExecuteFreezeRoutine(10f));
        }
    }

    public void UseUndoBooster()
    {
        if (Time.timeScale == 0 || (LoseManager.Instance != null && LoseManager.Instance.IsGameOver)) return;

        if (undoCount > 0)
        {
            if (TrayManager.Instance != null && TrayManager.Instance.UndoLastObject())
            {
                undoCount--;
                SaveBoosterCount("Booster_Undo", undoCount);
                UpdateUI();
            }
        }
    }

    private void ExecuteHintLogic()
    {
        if (GoalManager.Instance == null)
        {
            return;
        }

        List<string> activeGoalIDs = GoalManager.Instance.GetActiveGoalIDs();

        if (activeGoalIDs == null || activeGoalIDs.Count == 0)
        {
            return;
        }

        Rigidbody[] allRigidbodies = FindObjectsOfType<Rigidbody>();
        Dictionary<string, List<GameObject>> objectGroups = new Dictionary<string, List<GameObject>>();

        foreach (Rigidbody rb in allRigidbodies)
        {
            if (!rb.isKinematic)
            {
                string rawName = rb.gameObject.name;
                string cleanName = rawName.Replace("(Clone)", "").Replace("(1)", "").Replace("(2)", "").Trim();

                if (!objectGroups.ContainsKey(cleanName))
                {
                    objectGroups[cleanName] = new List<GameObject>();
                }

                objectGroups[cleanName].Add(rb.gameObject);
            }
        }

        List<GameObject> targetObjects = null;

        foreach (string goalID in activeGoalIDs)
        {
            if (objectGroups.ContainsKey(goalID) && objectGroups[goalID].Count > 0)
            {
                targetObjects = objectGroups[goalID];
                break;
            }
            else
            {
                foreach (var pair in objectGroups)
                {
                    if ((pair.Key.Contains(goalID) || goalID.Contains(pair.Key)) && pair.Value.Count > 0)
                    {
                        targetObjects = pair.Value;
                        break;
                    }
                }
                if (targetObjects != null) break;
            }
        }

        if (targetObjects == null || targetObjects.Count == 0)
        {
            return;
        }

        hintCount--;
        SaveBoosterCount("Booster_Hint", hintCount);
        UpdateUI();

        int highlightCount = Mathf.Min(targetObjects.Count, 3);
        StartCoroutine(ExecuteHintRoutine(targetObjects, highlightCount, 8f));
    }

    private IEnumerator ExecuteHintRoutine(List<GameObject> targetObjects, int highlightCount, float duration)
    {
        isHintActive = true;

        List<Coroutine> activeHighlights = new List<Coroutine>();

        for (int i = 0; i < highlightCount; i++)
        {
            if (targetObjects[i] != null)
            {
                activeHighlights.Add(StartCoroutine(HighlightObjectRoutine(targetObjects[i], duration)));
            }
        }

        yield return new WaitForSeconds(duration);

        isHintActive = false;
    }

    private IEnumerator HighlightObjectRoutine(GameObject obj, float duration)
    {
        if (obj == null) yield break;

        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers == null || renderers.Length == 0) yield break;

        Dictionary<Renderer, Color> originalColors = new Dictionary<Renderer, Color>();
        Dictionary<Renderer, Color> originalEmissionColors = new Dictionary<Renderer, Color>();

        GameObject spawnedParticle = null;
        if (hintSparklePrefab != null)
        {
            spawnedParticle = Instantiate(hintSparklePrefab, obj.transform.position, Quaternion.identity, obj.transform);
        }

        foreach (Renderer rend in renderers)
        {
            if (rend != null && rend.material != null)
            {
                if (rend.material.HasProperty("_Color"))
                {
                    originalColors[rend] = rend.material.color;

                    if (rend.material.HasProperty("_EmissionColor"))
                    {
                        originalEmissionColors[rend] = rend.material.GetColor("_EmissionColor");
                        rend.material.EnableKeyword("_EMISSION");
                    }
                }
            }
        }

        // --- RENK RENK DÖNÜÞÜM DÖNGÜSÜ (Rainbow / Color Cycle) ---
        float timer = 0f;
        float colorSpeed = 2f; // Renk geçiþ hýzý (arttýrdýkça daha hýzlý renk deðiþtirir)

        while (timer < duration)
        {
            if (obj == null) yield break; // Obje o sýrada seçilip yok edilirse hata vermesin

            timer += Time.deltaTime;

            // 0 ile 1 arasýnda sürekli dönen bir Hue (renk tonu) deðeri hesaplýyoruz
            float hue = Mathf.Repeat(Time.time * colorSpeed, 1f);
            Color rainbowColor = Color.HSVToRGB(hue, 1f, 1f); // Rengârenk canlý renk üretir

            foreach (Renderer rend in renderers)
            {
                if (rend != null && rend.material != null)
                {
                    if (rend.material.HasProperty("_Color"))
                    {
                        rend.material.color = rainbowColor;
                    }

                    if (rend.material.HasProperty("_EmissionColor"))
                    {
                        // Parlamayý da renge uygun þekilde güçlendiriyoruz
                        rend.material.SetColor("_EmissionColor", rainbowColor * 2.5f);
                    }
                }
            }

            yield return null; // Her karede rengi güncelle
        }

        if (spawnedParticle != null)
        {
            Destroy(spawnedParticle);
        }

        // --- ORÝJÝNAL RENKLERÝNE GERÝ DÖNDÜR ---
        foreach (Renderer rend in renderers)
        {
            if (rend != null && rend.material != null)
            {
                if (originalColors.ContainsKey(rend))
                {
                    rend.material.color = originalColors[rend];
                }

                if (rend.material.HasProperty("_EmissionColor"))
                {
                    if (originalEmissionColors.ContainsKey(rend))
                    {
                        rend.material.SetColor("_EmissionColor", originalEmissionColors[rend]);
                    }
                    else
                    {
                        rend.material.SetColor("_EmissionColor", Color.black);
                        rend.material.DisableKeyword("_EMISSION");
                    }
                }
            }
        }
    }

    private IEnumerator ExecuteFreezeRoutine(float duration)
    {
        isFrozen = true;

        if (TimerManager.Instance != null)
        {
            TimerManager.Instance.SetBoosterFreeze(true);
        }

        yield return new WaitForSecondsRealtime(duration);

        if (TimerManager.Instance != null)
        {
            TimerManager.Instance.SetBoosterFreeze(false);
        }

        isFrozen = false;
    }

    private void SaveBoosterCount(string key, int count)
    {
        PlayerPrefs.SetInt(key, count);
        PlayerPrefs.Save();
    }
}