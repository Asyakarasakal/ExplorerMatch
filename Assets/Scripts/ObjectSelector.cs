using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSelector : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)) 
        { 
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit) )
            {
                Debug.Log(hit.collider.gameObject.name);

                SelectableObject selectable = hit.collider.GetComponent<SelectableObject>();
                if(selectable != null)
                {
                    selectable.Highlight();
                }
            }
        }
    }
}
