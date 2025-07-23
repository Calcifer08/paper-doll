using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/HideAcneAction")]
public class HideAcneActionSO : DraggableItemActionSO
{
  public override IEnumerator ExecuteAction(Item item)
  {
    Color startColor = item.ImageMakeup.color;
    float startAlpha = startColor.a;

    float elapsed = 0f;
    while (elapsed < item.DurationMakeup)
    {
      elapsed += Time.deltaTime;
      float time = Mathf.Clamp01(elapsed / item.DurationMakeup);

      Color currentColor = startColor;
      currentColor.a = Mathf.Lerp(startAlpha, 0f, time);
      item.ImageMakeup.color = currentColor;

      yield return null;
    }

    Color finalColor = item.ImageMakeup.color;
    finalColor.a = 0f;
    item.ImageMakeup.color = finalColor;
  }
}
