using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TilesManager : MonoBehaviour
{
    [Header("Tiles")]
    [SerializeField] private GameObject tilesContainer = null;
    [SerializeField] private GameObject tilePrefab = null;
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
        
        RemoveRandomTile(true);
        ShuffleTiles();
    }
    
    /********************************************************************
     * Initialization
     ********************************************************************/

    private void GetRandomTexture()
    {
        var spriteId = Random.Range(0, textures.Length);
        _currentTexture = textures[spriteId];
    }

    private void InitTileArray()
    {
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
    
    /********************************************************************
     * Tile creation
     ********************************************************************/

    private Image CreateTileGameObject(int x, int y)
    {
        // Get the tile index
        var tileIndex = x * tilesPerRow + y;
        
        // Create the game object
        _tiles.Add(Instantiate(tilePrefab, tilesContainer.transform));
        _tiles[tileIndex].name = "tile" + x + ":" + y;

        // Add the placement of the tile
        var placementChecker = _tiles[tileIndex].GetComponent<TilePlacementChecker>();
        placementChecker.SetCurrentTilePos(x, y);
        placementChecker.SetCorrectTilePos(x, y);
        
        // Set tile size and position
        var tileRect = _tiles[tileIndex].GetComponent<RectTransform>();
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
        return _tiles[tileIndex].GetComponent<Image>();
    }

    private Sprite CreateSpriteFrom(int x, int y)
    {
        var rect = new Rect(_tileW * x, _tileH * y, _tileW, _tileH);
        return Sprite.Create(_currentTexture, rect, Vector2.one * 0.5f);
    }

    private GameObject GetRandomTile()
    {
        return _tiles[GetRandomTileIndex()];
    }

    private int GetRandomTileIndex()
    {
        return Random.Range(0, _tiles.Count);
    }

    private void ShuffleTiles()
    {
        for (var i = 0; i < _tiles.Count; i++)
        {
            // Get tiles
            var tile = _tiles[i];
            var secondTileIndex = GetRandomTileIndex();
            var secondTile = _tiles[secondTileIndex];
        
            if (tile == secondTile) continue;
            
            // Invert position
            var oldPos = tile.transform.position;
            var newPos = secondTile.transform.position;
        
            tile.transform.position = newPos;
            secondTile.transform.position = oldPos;
        
            // Swap index
            SwapTileIndex(i, tile, secondTileIndex, secondTile);
        }
    }
    
    private void SwapTileIndex(GameObject firstTile, GameObject secondTile)
    {
        var firstTileChecker = firstTile.GetComponent<TilePlacementChecker>();
        var secondTileChecker = secondTile.GetComponent<TilePlacementChecker>();
        var firstTilePos = firstTileChecker.GetCurrentTilePos();
            
        firstTileChecker.SetCurrentTilePos(secondTileChecker.GetCurrentTilePos());
        secondTileChecker.SetCurrentTilePos(firstTilePos);
    }

    private void SwapTileIndex(int firstIndex, GameObject firstTile, int secondIndex, GameObject secondTile)
    {
        SwapTileIndex(firstTile, secondTile);
            
        _tiles[secondIndex] = firstTile;
        _tiles[firstIndex] = secondTile;
    }
    
    

    /********************************************************************
     * Tile deletion
     ********************************************************************/

    private void RemoveRandomTile(bool fakeDeletion = false)
    {
        var tile = GetRandomTile();

        // Change the sprite to an invisible one. The tile still exist
        if (fakeDeletion)
        {
            tile.GetComponent<Image>().sprite = null;
            tile.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0f);
        }
        // Delete the tile entirely
        else
        {
            Destroy(tile);
            _tiles.Remove(tile);
        }
    }
    
    /********************************************************************
     * Tile movement
     ********************************************************************/

    public bool MoveTileTo(GameObject tile, Vector3 newPos, Vector3 oldPos, int oldSiblingIndex)
    {
        var hasMoved = false;
        
        // Loop through each tile and search for position match
        foreach (var t in _tiles)
        {
            // Skip if it's the moving tile
            if (t.name.Equals(tile.name)) continue;
            
            // Get second tile position, size and origin
            var tRect = t.GetComponent<RectTransform>().rect;
            var tW = tRect.width;
            var tH = tRect.height;
            var tPos = t.transform.localPosition;
            var tX = tPos.x;
            var tY = tPos.y;
            var tOriginX = tX - (tW / 2);
            var tOriginY = tY + (tH / 2);

            // Check if the moving tile is inside the second one
            if (IsInsideOfTile(newPos.x, newPos.y, tOriginX, tOriginY, tW, tH))
            {
                // Swap position
                tile.transform.localPosition = new Vector3(tX, tY);
                t.transform.position = oldPos;

                // Swap index
                SwapTileIndex(tile, t);
                
                hasMoved = true;
                break;
            }
        }
        
        Debug.Log("Finished ?: " + isPuzzleFinished());

        return hasMoved;
    }

    private bool IsInsideOfTile(float aX, float aY, float bX, float bY, float bW, float bH)
    {
        return ((aX > bX) && (aX < bX + bW)) && ((aY < bY) && (aY > bY - bH));
    }
    
    /********************************************************************
     * Puzzle completion
     ********************************************************************/

    public bool isPuzzleFinished()
    {
        for (var i = 0; i < _tiles.Count; i++)
        {
            var tile = _tiles[i];
            var placementChecker = tile.GetComponent<TilePlacementChecker>();
            var tilePos = placementChecker.GetCurrentTilePos();
            var tileEndingPos = placementChecker.GetCorrectTilePos();
            
            Debug.Log(tile.name + ": ("+tilePos.x+" != "+tileEndingPos.x+" || "+tilePos.y+" != "+tileEndingPos.y+") = "+
                      ((bool) (tilePos.x != tileEndingPos.x || tilePos.y != tileEndingPos.y)));

            if ((tilePos.x != tileEndingPos.x) || (tilePos.y != tileEndingPos.y))
            {
                return false;
            }
        }
        return true;
    }
}
