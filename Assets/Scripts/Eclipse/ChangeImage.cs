using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeImage : MonoBehaviour
{
    [SerializeField]
    private Toggle toggle;

    [SerializeField]
    private Image targetImage; // 변경할 이미지 컴포넌트
    [SerializeField]
    private Sprite onSprite; // 켜졌을 때 이미지
    [SerializeField]
    private Sprite offSprite; // 꺼졌을 때 이미지
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(toggle.isOn)
        {
            
            if(targetImage != null)
            {
                targetImage.sprite = onSprite;
            }
        }
        else
        {
            
            if(targetImage != null)
            {
                targetImage.sprite = offSprite;
            }
        }
    }
}
