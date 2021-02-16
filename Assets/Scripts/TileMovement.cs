using UnityEngine;
using UnityEngine.EventSystems;

public class TileMovement : MonoBehaviour
{
    private TilesManager _tilesCreator;
    private RectTransform _rectTransform;
    private RectTransform _parentRectTransform;
    private Vector3 _lastPosition;
    
    private void Start()
    {
        _tilesCreator = GetComponentInParent<TilesManager>();
        _rectTransform = GetComponent<RectTransform>();
        _parentRectTransform = transform.parent.GetComponent<RectTransform>();
        _lastPosition = transform.localPosition;
    }
    
    public void OnStartDrag(BaseEventData eventData)
    {
        // Save the position before any movement
        _lastPosition = transform.position;
    }
    
    public void OnDrag(BaseEventData eventData)
    {
        var e = (PointerEventData) eventData;
        
        // Move the tile onto the mouse and set it as last child (prevent from being draw behind other tiles)
        transform.position = e.position;
        transform.SetSiblingIndex(transform.parent.childCount);
    }
    
    
    public void OnEndDrag(BaseEventData eventData)
    {
        var e = (PointerEventData) eventData;

        // Get the position relative to the tiles container
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _parentRectTransform,
            e.position,
            e.pressEventCamera,
            out var localPos
        ))
        {
            return;
        }
        
        // Check if the tile can be moved, else return it back to the old position
        if (!_tilesCreator.MoveTileTo(gameObject, localPos, _lastPosition))
        {
            transform.position = _lastPosition;
        }
    }
}
