using System.Collections;
using UnityEngine;

public abstract class DraggableItemActionSO : ScriptableObject
{
  public abstract IEnumerator ExecuteAction(Item item);
}
