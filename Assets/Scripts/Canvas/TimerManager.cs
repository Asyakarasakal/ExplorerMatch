using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening; // DOTween kütüphanesi

public class TimerManager : MonoBehaviour
{
    public static TimerManager Instance;

    [Header("UI References")]
    public TMP_Text timerText;
    public GameObject freezeImageUI; // Saatin üzerindeki dondurma ikonu / spinner

    [Header("Freeze FX (Buz Kaplama)")]
    public Image freezeFlashPanel;   // Ekraný kaplayan buz görseli (Image)

    public float timeRemaining = 60f;

    private bool isGamePaused = false;   // Oyuncu Pause menüsünü açtý mý?
    private bool isBoosterFrozen = false; // Freeze Booster aktif mi?

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Baþlangýçta görsellere zemin hazýrlayalým
        if (freezeImageUI != null)
        {
            freezeImageUI.SetActive(false);
        }

        if (freezeFlashPanel != null)
        {
            freezeFlashPanel.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        // Oyun duraklatýldýysa VEYA booster ile zaman dondurulduysa süreyi akýtma!
        if (isGamePaused || isBoosterFrozen) return;

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;

            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                isGamePaused = true; // Süre bittiðinde durdur

                Debug.Log("TIME UP!");

                if (LoseManager.Instance != null)
                {
                    LoseManager.Instance.LoseGame();
                }
            }

            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);

            if (timerText != null)
            {
                timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }
        }
    }

    /// <summary>
    /// Pause butonu veya Pause Paneli tarafýndan çaðrýlýr
    /// </summary>
    public void TogglePause(bool pauseState)
    {
        isGamePaused = pauseState;
    }

    /// <summary>
    /// Freeze Booster tarafýndan çaðrýlýr
    /// </summary>
    public void SetBoosterFreeze(bool freezeState)
    {
        isBoosterFrozen = freezeState;

        if (freezeImageUI != null)
        {
            if (freezeState)
            {
                // Freeze baþladýðýnda: DOKill ile varsa önceki animasyonu durdur
                freezeImageUI.transform.DOKill();

                Image img = freezeImageUI.GetComponent<Image>();
                if (img != null)
                {
                    img.DOKill();
                    Color c = img.color;
                    c.a = 1f;
                    img.color = c;
                }

                CanvasGroup cg = freezeImageUI.GetComponent<CanvasGroup>();
                if (cg != null)
                {
                    cg.DOKill();
                    cg.alpha = 1f;
                }

                freezeImageUI.SetActive(true);

                // Ekran kaplama efektini tetikle
                if (freezeFlashPanel != null)
                {
                    TriggerFreezeEffect();
                }
            }
            else
            {
                // Freeze BÝTTÝÐÝNDE: Yumuþakça silikleþerek kaybol (Fade Out)
                Image img = freezeImageUI.GetComponent<Image>();
                CanvasGroup cg = freezeImageUI.GetComponent<CanvasGroup>();

                if (cg != null)
                {
                    cg.DOFade(0f, 0.4f).SetEase(Ease.OutQuad).OnComplete(() =>
                    {
                        freezeImageUI.SetActive(false);
                        cg.alpha = 1f; // Bir sonraki freeze için sýfýrla
                    });
                }
                else if (img != null)
                {
                    img.DOFade(0f, 0.4f).SetEase(Ease.OutQuad).OnComplete(() =>
                    {
                        freezeImageUI.SetActive(false);
                        Color c = img.color;
                        c.a = 1f; // Bir sonraki freeze için sýfýrla
                        img.color = c;
                    });
                }
                else
                {
                    freezeImageUI.SetActive(false);
                }
            }
        }
    }

    private void TriggerFreezeEffect()
    {
        freezeFlashPanel.gameObject.SetActive(true);

        Color flashColor = freezeFlashPanel.color;
        flashColor.a = 0.8f;
        freezeFlashPanel.color = flashColor;

        freezeFlashPanel.DOFade(0f, 1.5f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                freezeFlashPanel.gameObject.SetActive(false);
            });
    }

    /// <summary>
    /// Eski kodlarla uyumluluk için (Win/Lose durumunda çaðrýlýr)
    /// </summary>
    public void StopTimer()
    {
        isGamePaused = true;
    }
}