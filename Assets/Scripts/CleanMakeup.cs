using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CleanMakeup : MonoBehaviour, IPointerClickHandler
{
  [SerializeField] private Image _showSprite;

  [SerializeField] private List<Image> _hideSprites = new List<Image>();

  public void OnPointerClick(PointerEventData eventData)
  {
    ShowSprite();
    HideSprites();
  }

  private void ShowSprite()
  {
    Color color = _showSprite.color;
    color.a = 1f;
    _showSprite.color = color;
  }

  private void HideSprites()
  {
    Color color = new Color(1f, 1f, 1f, 0f);

    foreach (var image in _hideSprites)
    {
      image.color = color;
      image.sprite = null;
    }
  }
}
