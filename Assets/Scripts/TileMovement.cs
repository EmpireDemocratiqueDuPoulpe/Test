using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TileMovement : MonoBehaviour
{
    private GameManager _gameManager;

    private EventTrigger _eventTrigger;
    private Shadow _shadow;
    
    private TilesManager _tilesCreator;
    private RectTransform _rectTransform;
    private RectTransform _parentRectTransform;

    private bool _isDragged;
    private Vector3 _lastPosition;
    private int _lastSiblingIndex;
    
    private void Start()
    {
        // Game manager
        _gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        
        // Event trigger and shadow
        _eventTrigger = GetComponent<EventTrigger>();
        _shadow = GetComponent<Shadow>();
        
        // Tiles container
        _tilesCreator = GetComponentInParent<TilesManager>();
        _rectTransform = GetComponent<RectTransform>();
        _parentRectTransform = transform.parent.GetComponent<RectTransform>();
        
        // Tile position
        _lastPosition = transform.localPosition;
        _lastSiblingIndex = transform.GetSiblingIndex();
    }

    private void Update()
    {
        if (!_gameManager.IsGameFinished) return;
        
        _eventTrigger.enabled = false;
        _shadow.enabled = false;

        // Needed to put every tile in the right place
        if (_isDragged)
        {
            ResetTileLocalPos();
            
        }
        else
        {
            ResetTilePos();
        }
    }

    /********************************************************************
     * Drag event
     ********************************************************************/
    
    public void OnStartDrag(BaseEventData eventData)
    {
        _isDragged = true;
        _shadow.enabled = true;
        
        // Save the position before any movement
        _lastPosition = transform.position;
        _lastSiblingIndex = transform.GetSiblingIndex();
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
        _isDragged = false;
        _shadow.enabled = false;
        
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
        if (!_tilesCreator.MoveTileTo(gameObject, localPos, _lastPosition, _lastSiblingIndex))
        {
            ResetTilePos();
        }
        else
        {
            _lastPosition = transform.position;
        }
    }
    
    /********************************************************************
     * Pointer event
     ********************************************************************/

    public void OnPointerEnter(BaseEventData eventData)
    {
        transform.localScale = new Vector3(1.05f, 1.05f);
        _shadow.enabled = true;
    }
    
    public void OnPointerExit(BaseEventData eventData)
    {
        transform.localScale = new Vector3(1f, 1f);
        _shadow.enabled = false;
    }
    
    /********************************************************************
     * Tile position
     ********************************************************************/

    public void ResetTilePos()
    {
        transform.position = _lastPosition;
    }
    
    public void ResetTileLocalPos()
    {
        transform.localPosition = _lastPosition;
    }
}
