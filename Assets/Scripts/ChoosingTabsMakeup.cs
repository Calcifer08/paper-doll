using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoosingTabsMakeup : MonoBehaviour
{
  [System.Serializable]
  public class Tab
  {
    public Button buttonTab;
    public Sprite normalButtonSprite;
    public Sprite selectedButtonSprite;

    public GameObject tool;

    public GameObject prefabItem;
    public List<Sprite> spritesItemColor = new List<Sprite>();
    public List<Sprite> spritesMakeupColor = new List<Sprite>();

    public Vector2 SizeCell = new Vector2(100f, 100f);
  }

  [SerializeField] private List<Tab> _tabs = new List<Tab>();

  [SerializeField] private GridLayoutGroup _grid;

  [SerializeField] private float _scaleButtonHeight = 1.2f;
  [SerializeField] private int _currentTab = 0;

  private void Start()
  {
    for (int i = 0; i < _tabs.Count; i++)
    {
      int index = i;
      _tabs[i].buttonTab.onClick.AddListener(() => OnTabClicked(index));
    }

    Tab defaultTab = _tabs[_currentTab];

    UpdateTab(defaultTab, true);
    ActivateTool(defaultTab, true);
    RefreshGrid(defaultTab);
  }

  private void OnTabClicked(int newIndex)
  {
    if (_currentTab == newIndex) return;

    SwitchTab(newIndex);
  }

  private void SwitchTab(int newIndex)
  {
    Tab tabOld = _tabs[_currentTab];
    Tab tabNew = _tabs[newIndex];

    UpdateTab(tabOld, false);
    UpdateTab(tabNew, true);

    ActivateTool(tabOld, false);
    ActivateTool(tabNew, true);

    _currentTab = newIndex;

    RefreshGrid(tabNew);
  }

  private void ActivateTool(Tab tab, bool isActive)
  {
    if (tab.tool != null)
      tab.tool.SetActive(isActive);
  }

  private void UpdateTab(Tab tab, bool isSelected)
  {
    RectTransform rtNew = tab.buttonTab.GetComponent<RectTransform>();

    float heigh = isSelected ? rtNew.sizeDelta.y * _scaleButtonHeight : rtNew.sizeDelta.y / _scaleButtonHeight;
    Sprite sprite = isSelected ? tab.selectedButtonSprite : tab.normalButtonSprite;

    rtNew.sizeDelta = new Vector2(rtNew.sizeDelta.x, heigh);

    Image imageNew = tab.buttonTab.GetComponent<Image>();
    imageNew.sprite = sprite;
  }

  private void RefreshGrid(Tab tab)
  {
    ClearGrid();

    _grid.cellSize = tab.SizeCell;

    for (int i = 0; i < tab.spritesItemColor.Count; i++)
    {
      GameObject gameObject = Instantiate(tab.prefabItem, _grid.transform);
      gameObject.SetActive(true);
      Image image = gameObject.GetComponent<Image>();
      image.sprite = tab.spritesItemColor[i];

      CompositeItemActivator activator = gameObject.GetComponent<CompositeItemActivator>();
      if (activator != null)
        activator.Initialize(tab.tool?.GetComponent<CompositeItem>(), tab.spritesMakeupColor[i]);

      SimpleItem simpleItem = gameObject.GetComponent<SimpleItem>();
      if (simpleItem != null)
        simpleItem.SetMakeupSprite(tab.spritesMakeupColor[i]);
    }
  }

  public void ClearGrid()
  {
    foreach (Transform child in _grid.transform)
      Destroy(child.gameObject);
  }
}
