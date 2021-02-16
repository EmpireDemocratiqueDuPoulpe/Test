using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TileMovement : MonoBehaviour
{
    private GameManager _gameManager;

    private Image _image;
    private EventTrigger _eventTrigger;
    private Shadow _shadow;
    
    private TilesManager _tilesCreator;
    private RectTransform _rectTransform;
    private RectTransform _parentRectTransform;

    private bool _canBeDragged = true;
    private bool _isDragged;
    private Vector3 _lastPosition;
    private int _lastSiblingIndex;
    
    private void Start()
    {
        // Game manager
        _gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        
        // Tile components
        _image = GetComponent<Image>();
        _eventTrigger = GetComponent<EventTrigger>();
        _shadow = GetComponent<Shadow>();

        // Disable drag for empty tiles
        if (_image.sprite == null)
        {
            _canBeDragged = false;
        }
        
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
        ResetScaleTile();

        // Needed to put every tile in the right place
        if (_isDragged)
        {
            ResetTilePos();
        }
    }

    /********************************************************************
     * Drag event
     ********************************************************************/
    
    public void OnStartDrag(BaseEventData eventData)
    {
        if (!_canBeDragged) return;
        
        _isDragged = true;
        _shadow.enabled = true;
        
        // Save the position before any movement
        _lastPosition = transform.position;
        _lastSiblingIndex = transform.GetSiblingIndex();
    }
    
    public void OnDrag(BaseEventData eventData)
    {
        if (!_canBeDragged) return;
        
        var e = (PointerEventData) eventData;

        // Move the tile onto the mouse and set it as last child (prevent from being draw behind other tiles)
        transform.position = e.position;
        transform.SetSiblingIndex(transform.parent.childCount);
    }
    
    
    public void OnEndDrag(BaseEventData eventData)
    {
        if (!_canBeDragged) return;
        
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
        if (!_canBeDragged) return;
        
        ScaleTile(new Vector3(1.05f, 1.05f));
        _shadow.enabled = true;
    }
    
    public void OnPointerExit(BaseEventData eventData)
    {
        if (!_canBeDragged) return;
        
        ResetScaleTile();
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

    public void ScaleTile(Vector3 scale)
    {
        transform.localScale = scale;
    }
    
    public void ResetScaleTile()
    {
        ScaleTile(new Vector3(1f, 1f));
    }
}
