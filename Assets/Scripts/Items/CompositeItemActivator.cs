using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CompositeItemActivator : MonoBehaviour, IPointerClickHandler
{
  private CompositeItem _compositeItem;
  private RectTransform _rectTransform;
  private Sprite _sprite;

  private void Awake()
  {
    _rectTransform = GetComponent<RectTransform>();
  }

  // ���������� ��� ������
  public void Initialize(CompositeItem item, Sprite sprite)
  {
    if (item == null)
      Debug.LogWarning("�� ������� CompositeItem");
    else
      _compositeItem = item;

    _sprite = sprite;
  }

  public void OnPointerClick(PointerEventData eventData)
  {
    if (_compositeItem == null)
    {
      Debug.LogWarning("��� ������ �� CompositeItem");
      return;
    }

    Color color = GetCenterColor(GetComponent<Image>().sprite);

    _compositeItem.Activate(_rectTransform, _sprite, color);
  }

  // �.�. �� ���� �������� ��� ������� ������� �����, �������� ����� ����� �� ��������
  private Color GetCenterColor(Sprite sprite)
  {
    Texture2D texture = sprite.texture;
    Rect rect = sprite.textureRect;

    int x = Mathf.RoundToInt(rect.x + rect.width / 2f);
    int y = Mathf.RoundToInt(rect.y + rect.height / 2f);

    return texture.GetPixel(x, y);
  }
}
