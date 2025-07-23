using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class Item : MonoBehaviour
{
  protected HandController _handController;

  protected RectTransform _rectTrans;
  protected Transform _originalParent;
  protected Vector3 _originalAnchoredPosition;
  protected Vector3 _originalWorldPosition;

  [SerializeField] protected RectTransform _triggerItemZone;
  [SerializeField] protected RectTransform _triggerTargetZone;

  [SerializeField] protected float _durationMakeup = 1f;
  public float DurationMakeup => _durationMakeup;

  [SerializeField] protected float _durationMoveHand = 1f;

  [SerializeField] protected Image _imageMakeup;
  public Image ImageMakeup => _imageMakeup;

  [SerializeField] protected DraggableItemActionSO _action;

  [SerializeField] protected Sprite _spriteMakeupColor;
  public Sprite SpriteMakeupColor => _spriteMakeupColor;


  protected virtual void Awake()
  {
    _rectTrans = GetComponent<RectTransform>();
    if (_triggerItemZone == null)
    {
      _triggerItemZone = GameObject.Find("TriggerZone").GetComponent<RectTransform>();
    }

    InitializeAwake();
  }

  protected virtual void Start()
  {
    _originalWorldPosition = _rectTrans.position;
    _originalAnchoredPosition = _rectTrans.anchoredPosition;
    _originalParent = _rectTrans.parent;

    _handController = GameManager.Instance.HandController;

    InitializeStart();
  }

  protected abstract void InitializeAwake();

  protected abstract void InitializeStart();

  protected virtual bool CheckOverlapWithTarget(RectTransform triggerItemZone, RectTransform triggerTargetZone)
  {
    Vector3[] itemCorners = new Vector3[4];
    Vector3[] targetCorners = new Vector3[4];

    triggerItemZone.GetWorldCorners(itemCorners);

    triggerTargetZone.GetWorldCorners(targetCorners);

    Rect rectItem = new Rect(itemCorners[0], itemCorners[2] - itemCorners[0]);
    Rect rectTarget = new Rect(targetCorners[0], targetCorners[2] - targetCorners[0]);

    return rectItem.Overlaps(rectTarget);
  }

  public abstract IEnumerator InteractWithHand();

  public abstract IEnumerator HandleDrop();

  protected abstract IEnumerator ItemAction();

  public void SaveItemlPosition()
  {
    _originalParent = _rectTrans.parent;
    _originalAnchoredPosition = _rectTrans.anchoredPosition;
    _originalWorldPosition = _rectTrans.position;
  }
}
