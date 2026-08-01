using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SelectableObject : MonoBehaviour
{
    private Outline outline;
    private Vector3 originalScale;
    private bool isSelected = false;

    public string objectID;
    public Sprite objectIcon;

    [SerializeField] private ParticleSystem selectionEffectPrefab;

    void Start()
    {
        outline = GetComponent<Outline>();

        if (outline != null)
            outline.enabled = false;

        originalScale = transform.localScale;

        if (BoardManager.Instance != null)
        {
            BoardManager.Instance.RegisterObject(this);
        }
    }

    public void Highlight()
    {
        if (isSelected || IsGameFinished())
            return;

        Transform targetSlot = TrayManager.Instance.GetFirstEmptySlot();

        if (targetSlot == null)
        {
            Debug.Log("Tray dolu!");
            return;
        }

        isSelected = true;

        GetComponent<Collider>().enabled = false;
        if (outline != null) outline.enabled = false;

        HapticManager.Instance?.Vibrate();

        if (selectionEffectPrefab != null)
        {
            ParticleSystem effect = Instantiate(
                selectionEffectPrefab,
                transform.position,
                Quaternion.identity);

            effect.Play();
            Destroy(effect.gameObject, 1f);
        }

        Sequence pickupSequence = DOTween.Sequence();

        pickupSequence.Append(transform.DOScale(originalScale * 1.1f, 0.06f));
        pickupSequence.Append(transform.DOScale(originalScale * 0.30f, 0.15f).SetEase(Ease.InOutQuad));

        Vector3 targetPosition = TrayManager.Instance.GetSlotWorldPosition(targetSlot);

        pickupSequence.Join(transform.DOMove(targetPosition, 0.18f).SetEase(Ease.InCubic));
        pickupSequence.Join(transform.DORotate(new Vector3(0f, 0f, 15f), 0.18f).SetEase(Ease.OutSine));

        // BURADAKÝ CollectGoal ÇAÐRISI REMOVE EDÝLDÝ (Aþaðýdaki OnComplete içine taþýndý)

        pickupSequence.OnComplete(() =>
        {
            TraySlot traySlot = targetSlot.GetComponent<TraySlot>();

            if (traySlot != null && traySlot.IconImage != null)
            {
                traySlot.IsReserved = false;
                traySlot.IsOccupied = true;
                traySlot.CurrentObject = this;
                traySlot.ObjectID = objectID;

                traySlot.IconImage.sprite = objectIcon;
                traySlot.IconImage.enabled = true;
            }

            BoardManager.Instance?.RemoveObject(this);

            // OBJE TRAY'E YERLEÞTÝKTEN SONRA HEDEFÝ SAYIYORUZ:
            if (GoalManager.Instance != null)
            {
                GoalManager.Instance.CollectGoal(objectID);
            }

            TrayManager.Instance.CheckForMatch();

            Destroy(gameObject);
        });
    }

    private void OnMouseEnter()
    {
        if (Time.timeScale == 0f || IsGameFinished())
            return;

        if (outline == null || isSelected)
            return;

        outline.enabled = true;
    }

    private void OnMouseExit()
    {
        if (Time.timeScale == 0f || IsGameFinished())
            return;

        if (outline == null || isSelected)
            return;

        outline.enabled = false;
    }

    private bool IsGameFinished()
    {
        bool isLost = LoseManager.Instance != null && LoseManager.Instance.IsGameOver;
        bool isWon = GoalManager.Instance != null && GoalManager.Instance.IsGameWon;

        bool isStartPanelActive = LevelManager.Instance != null &&
                                  LevelManager.Instance.levelStartPanel != null &&
                                  LevelManager.Instance.levelStartPanel.activeSelf;

        bool isMatching = TrayManager.Instance != null && TrayManager.Instance.IsMatching();

        return isLost || isWon || isStartPanelActive || isMatching;
    }
}