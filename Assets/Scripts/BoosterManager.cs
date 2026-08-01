using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class BoosterManager : MonoBehaviour
{
    public static BoosterManager Instance;

    [Header("Booster Miktarlarý (PlayerPrefs Kayýtlý)")]
    public int defaultBoosterAmount = 3;

    [Header("UI Elemanlarý - Textler")]
    public TMP_Text hintCountText;
    public TMP_Text freezeCountText;
    public TMP_Text undoCountText;

    [Header("UI Elemanlarý - Butonlar")]
    public Button hintButton;
    public Button freezeButton;
    public Button undoButton;

    [Header("Particle FX")]
    public GameObject hintSparklePrefab;

    private int hintCount;
    private int freezeCount;
    private int undoCount;

    private bool isFrozen = false;
    private bool isHintActive = false; // Hint efekti sürerken tekrar basýlmasýný engeller

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

    // --- BOOSTER TIKLAMA METOTLARI ---

    public void UseHintBooster()
    {
        // Zaman durduysa, Game Over olduysa veya Hint efekti hâlâ devredeyse çalýþtýrma
        if (Time.timeScale == 0 || (LoseManager.Instance != null && LoseManager.Instance.IsGameOver) || isHintActive) return;

        if (hintCount > 0)
        {
            ExecuteHintLogic();
        }
        else
        {
            Debug.Log("Hint Booster kalmadý!");
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
        else if (isFrozen)
        {
            Debug.Log("Zaman zaten dondurulmuþ durumda!");
        }
        else
        {
            Debug.Log("Freeze Booster kalmadý!");
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

                Debug.Log($"BOOSTER: Undo kullanýldý! Kalan Undo: {undoCount}");
            }
            else
            {
                Debug.Log("Undo kullanýlamadý: Yuvada geri alýnacak obje yok!");
            }
        }
        else
        {
            Debug.Log("Undo Booster kalmadý!");
        }
    }

    // --- MEKANÝK MANTIKLARI ---

    private void ExecuteHintLogic()
    {
        Debug.Log("BOOSTER: Hint kullanýldý! Eksik Goal (Hedef) objeleri aranýyor...");

        if (GoalManager.Instance == null)
        {
            Debug.LogWarning("Hint: GoalManager sahnede bulunamadý!");
            return;
        }

        List<string> activeGoalIDs = GoalManager.Instance.GetActiveGoalIDs();

        if (activeGoalIDs == null || activeGoalIDs.Count == 0)
        {
            Debug.Log("Hint: Tamamlanmamýþ aktif hedef kalmadý!");
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
            Debug.LogWarning("Hint: Eksik hedeflere ait sahnede obje bulunamadý!");
            return;
        }

        // Objeler bulundu, Hint kullaným hakkýný düþ ve kilitle
        hintCount--;
        SaveBoosterCount("Booster_Hint", hintCount);
        UpdateUI();

        int highlightCount = Mathf.Min(targetObjects.Count, 3);
        StartCoroutine(ExecuteHintRoutine(targetObjects, highlightCount, 8f));

        Debug.Log($"Hint: Goal olan {highlightCount} adet '{targetObjects[0].name}' objesi parlatýldý!");
    }

    /// <summary>
    /// Hint süresince buton basýmýný engeller ve sürenin sonunda objelerin renklerini sýfýrlar.
    /// </summary>
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

        // Parlatma süresince bekle
        yield return new WaitForSeconds(duration);

        isHintActive = false;
    }

    private IEnumerator HighlightObjectRoutine(GameObject obj, float duration)
    {
        if (obj == null) yield break;

        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers == null || renderers.Length == 0) yield break;

        // Orijinal renkleri sakla
        Dictionary<Renderer, Color> originalColors = new Dictionary<Renderer, Color>();
        Dictionary<Renderer, Color> originalEmissionColors = new Dictionary<Renderer, Color>();

        Color brightNeonMagenta = new Color(3.0f, 0.0f, 1.5f, 1f);

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
                        rend.material.SetColor("_EmissionColor", new Color(1f, 0f, 0.5f) * 3f);
                    }

                    // DOColor animasyonunu materyale uygula
                    rend.material.DOColor(brightNeonMagenta, 0.25f)
                        .SetLoops(-1, LoopType.Yoyo)
                        .SetEase(Ease.InOutSine);
                }
            }
        }

        yield return new WaitForSeconds(duration);

        if (spawnedParticle != null)
        {
            Destroy(spawnedParticle);
        }

        // Orijinal renklerine geri döndür
        foreach (Renderer rend in renderers)
        {
            if (rend != null && rend.material != null)
            {
                // Animasyonlarý durdur
                rend.material.DOKill();

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
            Debug.Log("BOOSTER: Zaman " + duration + " saniyeliðine donduruldu!");
        }

        yield return new WaitForSecondsRealtime(duration);

        if (TimerManager.Instance != null)
        {
            TimerManager.Instance.SetBoosterFreeze(false);
            Debug.Log("BOOSTER: Zaman dondurma bitti, süre devam ediyor.");
        }

        isFrozen = false;
    }

    private void SaveBoosterCount(string key, int count)
    {
        PlayerPrefs.SetInt(key, count);
        PlayerPrefs.Save();
    }
}