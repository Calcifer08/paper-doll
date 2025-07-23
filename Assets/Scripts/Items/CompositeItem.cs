using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CompositeItem : Item
{
  private RectTransform _rectMakeupOption;
  public Color ColorMakeup { get; private set; }

  protected override void InitializeAwake() { }

  protected override void InitializeStart() { }

  public override IEnumerator InteractWithHand()
  {
    yield return StartCoroutine(_handController.MoveToItem(_rectTrans.position, _durationMoveHand));

    _handController.TakeItem(_rectTrans);

    Vector3 targetPos = _handController.CalculateGripPositionWithOffset(_rectMakeupOption, _triggerItemZone);
    yield return StartCoroutine(_handController.MoveToItem(targetPos, _durationMoveHand));
    
    StartCoroutine(DrawTriggerItemZone());
    yield return StartCoroutine(_handController.PatrolMovementCoroutine(_rectMakeupOption, _triggerItemZone, _durationMoveHand));

    Vector3 target = GameManager.Instance.FaceZone.position;
    yield return StartCoroutine(_handController.MoveToHalfCoroutine(target, _durationMoveHand));
  }

  public override IEnumerator HandleDrop()
  {
    if (CheckOverlapWithTarget(_triggerTargetZone, _triggerItemZone))
    {
      StartCoroutine(ItemAction());
    }
    else
    {
      yield return StartCoroutine(_handController.BackItemCoroutine(_originalWorldPosition, _durationMoveHand));

      _handController.DropItem(_rectTrans, _originalParent, _originalAnchoredPosition);

      ClearItemColor();
    }
  }

  protected override IEnumerator ItemAction()
  {
    StartCoroutine(_handController.PatrolMovementCoroutine(_triggerTargetZone, _triggerItemZone, _durationMakeup));

    StartCoroutine(_action.ExecuteAction(this));

    yield return new WaitForSeconds(_durationMakeup);

    yield return StartCoroutine(_handController.BackItemCoroutine(_originalWorldPosition, _durationMoveHand));
    _handController.DropItem(_rectTrans, _originalParent, _originalAnchoredPosition);

    ClearItemColor();
  }

  public void Activate(RectTransform rectMakeupOption, Sprite sprite, Color color)
  {
    _handController.SetItem(this);

    _rectMakeupOption = rectMakeupOption;
    _spriteMakeupColor = sprite;
    ColorMakeup = color;

    StartCoroutine(InteractWithHand());
  }

  private IEnumerator DrawTriggerItemZone()
  {
    Image image = _triggerItemZone.gameObject.GetComponent<Image>();
    image.color = ColorMakeup;

    Color startColor = image.color;
    float startAlpha = 0f;

    float elapsed = 0f;
    while (elapsed < _durationMakeup)
    {
      elapsed += Time.deltaTime;
      float time = Mathf.Clamp01(elapsed / _durationMakeup);

      Color currentColor = startColor;
      currentColor.a = Mathf.Lerp(startAlpha, 1f, time);
      image.color = currentColor;

      yield return null;
    }

    Color finalColor = image.color;
    finalColor.a = 1f;
    image.color = finalColor;
  }

  private void ClearItemColor()
  {
    Image image = _triggerItemZone.gameObject.GetComponent<Image>();
    image.color = new Color(1f, 1f, 1f, 0f);
  }
}
