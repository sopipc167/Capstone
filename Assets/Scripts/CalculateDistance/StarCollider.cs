using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarCollider : MonoBehaviour
{
    private ObjectManager obj;
    private float ColliderRadius = 5.0f;
    // Start is called before the first frame update
    void Start()
    {
        obj = FindAnyObjectByType<ObjectManager>();
        if (obj == null)
            Debug.Log("Can't Load Stars");
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
                    SphereCollider collider = tmp.GetComponent<SphereCollider>();
                    if (collider == null)
                    {
                        collider = tmp.AddComponent<SphereCollider>(); // Collider 추가
                        
                        
                    }
                    collider.radius = ColliderRadius;
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
                    SphereCollider collider = gameObject.GetComponent<SphereCollider>();
                    if (collider != null)
                    {
                        collider.enabled = false; // 비활성화
                    }
                }
            }
        }
    }
}
