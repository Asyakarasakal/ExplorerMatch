using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; // UI üzerine týklama kontrolü için eklendi

public class ObjectSelector : MonoBehaviour
{
    void Update()
    {
        if (Time.timeScale == 0f)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            // 1. Týklama anýnda imleç herhangi bir UI elemanýnýn (Goal Card, Buton vs.) üzerindeyse Raycast ATMA!
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            // 2. Mobil/Dokunmatik ekranlar için UI kontrolü (Önemli)
            if (Input.touchCount > 0 && EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                return;
            }

            // 3. LevelStartPanel sahnede açýksa nesne seçilmesin
            if (LevelManager.Instance != null && LevelManager.Instance.levelStartPanel != null && LevelManager.Instance.levelStartPanel.activeSelf)
            {
                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.collider.gameObject.name);

                SelectableObject selectable = hit.collider.GetComponent<SelectableObject>();
                if (selectable != null)
                {
                    selectable.Highlight();
                }
            }
        }
    }
}