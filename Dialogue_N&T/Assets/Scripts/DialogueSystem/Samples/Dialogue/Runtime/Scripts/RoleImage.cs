using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoleImage : MonoBehaviour
{
    public Image _RoleImage;
    public Sprite test1;
    public void Init()
    {
        _RoleImage.sprite = test1;// 这里接资源加载
        _RoleImage.SetNativeSize();
        White();
        _RoleImage.GetComponent<CanvasGroup>().DOFade(1, 1);
    }
    public void White()
    {
        _RoleImage.DOColor(Color.white, 1);
    }
    public void Dark()
    {
        _RoleImage.DOColor(new Color(0.4f, 0.4f, 0.4f), 1);
    }
    public void Hide()
    {
        _RoleImage.GetComponent<CanvasGroup>().DOFade(0,1);
    }
    
}
