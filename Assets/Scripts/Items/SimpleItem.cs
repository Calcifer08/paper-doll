using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SimpleItem : Item, IPointerClickHandler
{
  protected override void InitializeAwake() { }

  protected override void InitializeStart() { }

  public void OnPointerClick(PointerEventData eventData)
  {
    _handController.SetItem(this);
  }

  public override IEnumerator InteractWithHand()
  {
    yield return StartCoroutine(_handController.MoveToItem(_rectTrans.position, _durationMoveHand));

    _handController.TakeItem(_rectTrans);

    Vector3 target = GameManager.Instance.FaceZone.position;
    yield return StartCoroutine(_handController.MoveToHalfCoroutine(target, _durationMoveHand));

    _handController.EndInteract();
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
    }
  }

  protected override IEnumerator ItemAction()
  {
    StartCoroutine(_handController.PatrolMovementCoroutine(_triggerTargetZone, _triggerItemZone, _durationMakeup));

    StartCoroutine(_action.ExecuteAction(this));

    yield return new WaitForSeconds(_durationMakeup);

    yield return StartCoroutine(_handController.BackItemCoroutine(_originalWorldPosition, _durationMoveHand));
    _handController.DropItem(_rectTrans, _originalParent, _originalAnchoredPosition);
  }

  // если предмет может окрасить
  public void SetMakeupSprite(Sprite sprite)
  {
    _spriteMakeupColor = sprite;
  }
}
