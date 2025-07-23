using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/PaintAction")]
public class PaintActionSO : DraggableItemActionSO
{
  public override IEnumerator ExecuteAction(Item item)
  {
    {
      item.ImageMakeup.sprite = item.SpriteMakeupColor;

      Color startColor = new Color(1f, 1f, 1f, 0f);
      float startAlpha = startColor.a;

      float elapsed = 0f;
      while (elapsed < item.DurationMakeup)
      {
        elapsed += Time.deltaTime;
        float time = Mathf.Clamp01(elapsed / item.DurationMakeup);

        Color currentColor = startColor;
        currentColor.a = Mathf.Lerp(startAlpha, 1f, time);
        item.ImageMakeup.color = currentColor;

        yield return null;
      }

      Color finalColor = item.ImageMakeup.color;
      finalColor.a = 1f;
      item.ImageMakeup.color = finalColor;
    }
  }
}
