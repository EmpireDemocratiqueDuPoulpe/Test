using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [Header("Tiles")]
    [SerializeField] private GameObject tilesContainer = null;
    [SerializeField] private int tilesPerRow = 3;
    [SerializeField] private int tilesMarginSize = 10;
    [SerializeField] private Texture2D[] textures = new Texture2D[0];

    private float _containerW;
    private float _containerH;
    private float _tileW;
    private float _tileH;
    private Texture2D _currentTexture;
    private List<GameObject> _tiles;
    
    private void Start()
    {
        // Get the texture used in the game
        GetRandomTexture();
        
        // Init sizes
        GetContainerSize();
        GetTileSize();
        InitTileArray();

        // Cut the sprite into pieces
        for (var x = 0; x < tilesPerRow; x++)
        {
            for (var y = 0; y < tilesPerRow; y++)
            {
                var tileImage = CreateTileGameObject(x, y);
                tileImage.sprite = CreateSpriteFrom(x, y);
            }
        }
        
        RemoveRandomTile();
    }

    private void GetRandomTexture()
    {
        var spriteId = Random.Range(0, textures.Length);
        _currentTexture = textures[spriteId];
    }

    private void InitTileArray()
    {
        //_tiles = new GameObject[(int) Mathf.Pow(tilesPerRow, 2)];
        _tiles = new List<GameObject>();
    }

    private void GetContainerSize()
    {
        var containerRect = tilesContainer.GetComponent<RectTransform>().rect;
        _containerW = containerRect.width;
        _containerH = containerRect.height;
    }
    
    private void GetTileSize()
    {
        var spriteW = _currentTexture.width;
        var spriteH= _currentTexture.height;
        _tileW = (float) spriteW / tilesPerRow;
        _tileH = (float) spriteH / tilesPerRow;
    }

    private Image CreateTileGameObject(int x, int y)
    {
        // Get the tile index
        var tileIndex = x * tilesPerRow + y;
        
        // Create the game object
        //_tiles[tileIndex] = new GameObject("tile" + x + ":" + y);
        _tiles.Add(new GameObject("tile" + x + ":" + y));
        _tiles[tileIndex].transform.parent = tilesContainer.transform;
        
        // Set tile size and position
        var tileRect = _tiles[tileIndex].AddComponent<RectTransform>();
        var gW = _containerW / tilesPerRow;
        var gH = _containerH / tilesPerRow;
        
        tileRect.sizeDelta = new Vector2(gW - tilesMarginSize, gH - tilesMarginSize);
        tileRect.localPosition = new Vector3(
            ((_containerW / 2) - _containerW) + (gW / 2) + (gW * x), 
            ((_containerH / 2) - _containerH) + (gH / 2) + (gH * y),
            0f
        );
        
        // The previous line can be optimised to:
        // (-_containerW + gW + (2 * gW * x)) / 2
        // (-_containerH + gH + (2 * gH * y)) / 2
        
        // Add the Image component
        return _tiles[tileIndex].AddComponent<Image>();
    }

    private Sprite CreateSpriteFrom(int x, int y)
    {
        var rect = new Rect(_tileW * x, _tileH * y, _tileW, _tileH);
        return Sprite.Create(_currentTexture, rect, Vector2.one * 0.5f);
    }

    private void RemoveRandomTile()
    {
        var tileIndex = Random.Range(0, _tiles.Count);
        Destroy(_tiles[tileIndex]);
        _tiles.RemoveAt(tileIndex);
    }
}
