using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarCollider : MonoBehaviour
{
    private ObjectManager obj;
    // Start is called before the first frame update
    void Start()
    {
        obj = FindAnyObjectByType<ObjectManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ActivateColliders()
    {
        if (obj != null)
        {
            foreach (var starObject in obj.starList)
            {
                GameObject tmp = starObject.GO;
                if (tmp != null)
                {
                    Collider collider = gameObject.GetComponent<Collider>();
                    if (collider == null)
                    {
                        collider = gameObject.AddComponent<SphereCollider>(); // Collider 추가
                        
                    }
                    collider.enabled = true;
                }
            }
        }
    }

    public void DeactivateColliders()
    {
        if (obj != null)
        {
            foreach (var starObject in obj.starList)
            {
                GameObject gameObject = starObject.GO;
                if (gameObject != null)
                {
                    Collider collider = gameObject.GetComponent<Collider>();
                    if (collider != null)
                    {
                        collider.enabled = false; // 비활성화
                    }
                }
            }
        }
    }
}
