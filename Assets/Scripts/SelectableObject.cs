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

    public string objectID;

    [SerializeField] private ParticleSystem selectionEffectPrefab;
   

    void Start()
    {
        outline = GetComponent<Outline>();
        

        outline.enabled = false;
       

        originalScale = transform.localScale;

        BoardManager.Instance.RegisterObject(this);

    }

    public void Highlight()
    {
       

        if (isSelected) return;
       

        isSelected = true;

        Transform targetSlot = TrayManager.Instance.GetFirstEmptySlot();

        if (targetSlot == null)
        {
            Debug.Log("GAME OVER! Tray is full.");
            return;
        }

        TraySlot traySlot = targetSlot.GetComponent<TraySlot>();

        traySlot.IsOccupied = true;
        traySlot.CurrentObject = this;

        traySlot.ObjectID = objectID;

        Debug.Log("Slot: " + targetSlot.name + " | ObjectID: " + traySlot.ObjectID);

       // TrayManager.Instance.CheckForMatch();


        if (targetSlot != null)
        {
            Vector3 worldPos = TrayManager.Instance.GetSlotWorldPosition(targetSlot);

            Debug.Log("World Position: " + worldPos);
        }

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
            transform.DOScale(originalScale * 0.30f, 0.3f)
             .SetEase(Ease.InOutQuad)
        );


        if (targetSlot == null)
            return;

        Vector3 targetPosition = TrayManager.Instance.GetSlotWorldPosition(targetSlot);

        pickupSequence.Join(
            transform.DOMove(targetPosition, 0.33f)
                .SetEase(Ease.InCubic)
        );
        

         pickupSequence.Join(
           transform.DORotate(
           new Vector3(0f, 0f, 15f),
            0.45f
         ).SetEase(Ease.OutSine)
        );


        pickupSequence.Insert(
          0.35f,
          transform.DOScale(originalScale * 0.45f, 0.10f)
          .SetEase(Ease.InQuad)
       );

        pickupSequence.OnComplete(() =>
        {
            BoardManager.Instance.RemoveObject(this);

            TrayManager.Instance.CheckForMatch();

            outline.enabled = false;

            Destroy(gameObject);
        });

        Debug.Log(gameObject.name + " highlighted.");
       
    }
}
