using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSelector : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0f)
            return;

        if (Input.GetMouseButtonDown(0))
        {
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
