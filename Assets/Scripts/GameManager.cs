using UnityEngine;

public class GameManager : MonoBehaviour
{
  public static GameManager Instance { get; private set; }

  [SerializeField] private HandController _handController;
  [SerializeField] private RectTransform _faceZone;

  public HandController HandController => _handController;
  public RectTransform FaceZone => _faceZone;

  private void Awake()
  {
    if (Instance != null && Instance != this)
    {
      Destroy(gameObject);
      return;
    }

    Instance = this;

    if (_handController == null)
      _handController = FindFirstObjectByType<HandController>();

    if (_faceZone == null)
      _faceZone = GameObject.Find("FaceZone").GetComponent<RectTransform>();
  }
}
