using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HandController : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
  private RectTransform _rtHandRoot;
  private Vector3 _startPosition;
  private Canvas _canvas;
  private CanvasGroup _canvasGroup;

  [SerializeField] private float _durationMoveHand = 0.5f;

  private Item _draggedItem;

  [SerializeField] GameObject _placeholderPrefab;
  private GameObject _placeholder;

  private bool isDrag = false;

  [SerializeField] private Image _raycastBlock;

  private void Awake()
  {
    _rtHandRoot = transform.parent.GetComponent<RectTransform>();
    _canvas = GetComponentInParent<Canvas>();
    _canvasGroup = GetComponent<CanvasGroup>();
  }

  private void Start()
  {
    _startPosition = _rtHandRoot.position;
  }

  void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
  {
    if (!isDrag) return;

    // чтобы не перекрывал предметы под собой для взаимодействия, когда передвигаем
    _canvasGroup.blocksRaycasts = false;
  }

  public void OnDrag(PointerEventData eventData)
  {
    if (!isDrag) return;

    _rtHandRoot.anchoredPosition += eventData.delta / _canvas.scaleFactor;
  }

  public void OnEndDrag(PointerEventData eventData)
  {
    if (!isDrag) return;

    isDrag = false;
    _canvasGroup.blocksRaycasts = true;

    StartCoroutine(_draggedItem.HandleDrop());
  }

  public void EndInteract()
  {
    isDrag = true;
  }

  public void SetItem(Item item)
  {
    if (_draggedItem != null) return;
    _draggedItem = item;

    // блокируем возможность нажатие на другие предметы
    _raycastBlock.raycastTarget = true;

    _draggedItem.SaveItemlPosition();
    StartCoroutine(_draggedItem.InteractWithHand());
  }

  public IEnumerator MoveToItem(Vector3 targetPos, float duration)
  {
    yield return StartCoroutine(MoveHand(targetPos, duration));
  }

  private IEnumerator MoveHand(Vector3 targetPos, float duration)
  {
    Vector3 startPos = _rtHandRoot.position;

    float elapsed = 0f;

    while (elapsed < duration)
    {
      elapsed += Time.deltaTime;
      float time = Mathf.Clamp01(elapsed / duration);
      _rtHandRoot.position = Vector3.Lerp(startPos, targetPos, time);
      yield return null;
    }

    _rtHandRoot.position = targetPos;
  }

  public void TakeItem(RectTransform itemRect)
  {
    // при работе с GridLayoutGroup и т.п. нужно подменять взятый предмет на пустышку, чтобы не сдвигались другие
    GameObject placeholder = Instantiate(_placeholderPrefab, itemRect.parent);
    RectTransform placeholderRT = placeholder.GetComponent<RectTransform>();
    placeholderRT.SetSiblingIndex(itemRect.GetSiblingIndex());
    placeholderRT.sizeDelta = itemRect.sizeDelta;
    _placeholder = placeholder;

    itemRect.SetParent(_rtHandRoot, true);
    itemRect.SetAsFirstSibling();
  }

  public void DropItem(RectTransform itemRect, Transform parent, Vector2 anchoredPosition)
  {
    // чтобы вернуть на своё место, а не в конец
    int index = _placeholder.transform.GetSiblingIndex();

    itemRect.SetParent(parent, true);
    itemRect.SetSiblingIndex(index);
    itemRect.anchoredPosition = anchoredPosition;

    Destroy(_placeholder);
    _placeholder = null;

    _raycastBlock.raycastTarget = false;
  }

  // т.к. предмет в руке торчит из неё, надо учитывать именно его точку, а не руки
  public Vector3 CalculateGripPositionWithOffset(RectTransform targetRect, RectTransform triggerItemRect)
  {
    Vector3 topOffset = triggerItemRect.up * (triggerItemRect.rect.height / 2f) * triggerItemRect.lossyScale.y;
    Vector3 triggerTopPoint = triggerItemRect.position + topOffset;
    Vector3 gripOffset = _rtHandRoot.position - triggerTopPoint;
    return targetRect.position + gripOffset;
  }

  public IEnumerator MoveToHalfCoroutine(Vector3 targetPos, float duration)
  {
    duration /= 2;
    Vector3 halfwayPos = Vector3.Lerp(_rtHandRoot.position, targetPos, 0.5f);

    yield return StartCoroutine(MoveHand(halfwayPos, duration));
  }

  public IEnumerator BackItemCoroutine(Vector3 targetPos, float duration)
  {
    yield return StartCoroutine(MoveHand(targetPos, duration));

    _draggedItem = null;

    StartCoroutine(BackHandCoroutine(_startPosition, _durationMoveHand));
  }

  private IEnumerator BackHandCoroutine(Vector3 target, float duration)
  {
    yield return StartCoroutine(MoveHand(target, duration));
  }

  public IEnumerator PatrolMovementCoroutine(RectTransform triggerTargetZone, RectTransform triggerItemZone, float duration)
  {
    Vector3 gripPosition = CalculateGripPositionWithOffset(triggerTargetZone, triggerItemZone);
    float yPos = gripPosition.y;
    float zPos = triggerItemZone.position.z;

    (Vector3 left, Vector3 right) = GetZoneHorizontalPoints(triggerTargetZone, yPos, zPos);

    yield return MoveHandPatrol(left, right, duration, 2);
  }

  private (Vector3 left, Vector3 right) GetZoneHorizontalPoints(RectTransform zone, float yPos, float zPos)
  {
    Vector3[] corners = new Vector3[4];
    zone.GetWorldCorners(corners);

    float leftX = corners[0].x;
    float rightX = corners[2].x;

    Vector3 left = new Vector3(leftX, yPos, zPos);
    Vector3 right = new Vector3(rightX, yPos, zPos);

    return (left, right);
  }

  private IEnumerator MoveHandPatrol(Vector3 leftPoint, Vector3 rightPoint, float totalDuration, int cycles)
  {
    int totalHalfCycles = cycles * 2;
    float oneWayDuration = totalDuration / totalHalfCycles;

    for (int i = 0; i < totalHalfCycles; i++)
    {
      Vector3 start = (i % 2 == 0) ? leftPoint : rightPoint;
      Vector3 end = (i % 2 == 0) ? rightPoint : leftPoint;

      float elapsed = 0f;
      while (elapsed < oneWayDuration)
      {
        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / oneWayDuration);
        _rtHandRoot.position = Vector3.Lerp(start, end, t);
        yield return null;
      }
    }
  }
}