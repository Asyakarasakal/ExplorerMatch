using System.Collections;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectableObject : MonoBehaviour
{
    private Outline outline;
    private Vector3 originalScale;
    private bool isSelected = false;

    [SerializeField] private ParticleSystem selectionEffectPrefab;

    void Start()
    {
        outline = GetComponent<Outline>();
        outline.enabled = false;

        originalScale = transform.localScale;
    }
    public void Highlight()
    {
        if (isSelected)
            return;

        isSelected = true;
        GetComponent<Collider>().enabled = false;
        outline.enabled = true;

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

        pickupSequence.Append(
            transform.DOScale(originalScale * 1.1f, 0.12f)
        );

        pickupSequence.Append(
            transform.DOScale(originalScale, 0.12f)
        );

        pickupSequence.Join(
            transform.DOScale(originalScale * 0.75f, 0.40f)
             .SetEase(Ease.InOutQuad)
        );

        pickupSequence.Join(
           transform.DOMove(
           transform.position + new Vector3(0f, 0.25f, -4.8f),
            0.40f)
           .SetEase(Ease.InCubic)
        );

        pickupSequence.Join(
           transform.DORotate(
           new Vector3(0f, 0f, 15f),
            0.45f
          ).SetEase(Ease.OutSine)
        );

        pickupSequence.OnComplete(() =>
        {
            outline.enabled = false;
            Destroy(gameObject);
        });

        Debug.Log(gameObject.name + " highlighted.");
    }

    
}
