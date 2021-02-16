using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TilesManager : MonoBehaviour
{
    [Header("Tiles")]
    public GameObject tilesContainer;
    public GameObject tilePrefab = null;
    public int tilesPerRow = 3;
    public int tilesMarginSize = 10;
    public bool destroyRandomTile = true;
    public Texture2D[] textures = new Texture2D[0];

    
    private GameManager _gameManager;
    
    private float _containerW;
    private float _containerH;
    private float _tileW;
    private float _tileH;
    private Texture2D _currentTexture;
    private List<GameObject> _tiles;
    
    private void Start()
    {
        // Game manager
        _gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        
        GetNewPuzzle();
    }
    
    /********************************************************************
     * Initialization
     ********************************************************************/

    public void GetNewPuzzle()
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

        ShuffleTiles();
        
        if (destroyRandomTile)
        {
            RemoveRandomTile(true);
        }
        else
        {
            RemoveCenterTile(true);
        }
    }

    private void GetRandomTexture()
    {
        var spriteId = Random.Range(0, textures.Length);
        _currentTexture = textures[spriteId];
    }

    private void InitTileArray()
    {
        _tiles = new List<GameObject>();
        
        // Destroy tiles of the world
        foreach (Transform tile in tilesContainer.transform)
        {
            Destroy(tile.gameObject);
        }
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

    private void RemoveCenterTile(bool fakeDeletion = false)
    {
        // Doesn't work
        //var tileIndex = (Mathf.CeilToInt(tilesPerRow * (tilesPerRow / 2))) - 1;
        //var tile = _tiles[tileIndex];
        //
        //RemoveTile(tile, fakeDeletion);

        var center = Mathf.CeilToInt(tilesPerRow / 2);
        
        foreach (var tile in _tiles)
        {
            var checker = tile.GetComponent<TilePlacementChecker>();
            var pos = checker.GetCurrentTilePos();

            if (pos.x == center && pos.y == center)
            {
                RemoveTile(tile, fakeDeletion);
                break;
            }
        }
    }
    
    private void RemoveRandomTile(bool fakeDeletion = false)
    {
        Debug.Log("RemoveRandomTile");
        RemoveTile(GetRandomTile(), fakeDeletion);
    }
    
    private void RemoveTile(GameObject tile, bool fakeDeletion = false)
    {
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
            
            // Skip if the tile isn't empty
            if (!IsMovingOnEmptyTile(t)) continue;
            
            // Skip if the tile isn't next to the moving one
            if (!IsNextToTheTile(tile, t)) continue;

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

        Invoke(nameof(CheckPuzzleState), 1f);

        return hasMoved;
    }
    
    private bool IsMovingOnEmptyTile(GameObject tile)
    {
        var image = tile.GetComponent<Image>();
        return (image.sprite == null);
    }
    
    private bool IsNextToTheTile(GameObject firstTile, GameObject secondTile)
    {
        // Get Placement checker and pos
        var firstTileChecker = firstTile.GetComponent<TilePlacementChecker>();
        var secondTileChecker = secondTile.GetComponent<TilePlacementChecker>();

        var firstPos = firstTileChecker.GetCurrentTilePos();
        var secondPos = secondTileChecker.GetCurrentTilePos();
        
        // Check where the second tile is (relative to the first)
        var isOnLeftRight = (firstPos.x - 1 == secondPos.x) || (firstPos.x + 1 == secondPos.x);
        var isOnTopBottom = (firstPos.y - 1 == secondPos.y) || (firstPos.y + 1 == secondPos.y);
        
        return (isOnLeftRight && !isOnTopBottom) || (isOnTopBottom && !isOnLeftRight);
    }

    private bool IsInsideOfTile(float aX, float aY, float bX, float bY, float bW, float bH)
    {
        return ((aX > bX) && (aX < bX + bW)) && ((aY < bY) && (aY > bY - bH));
    }
    
    /********************************************************************
     * Puzzle completion
     ********************************************************************/

    public bool CheckPuzzleState()
    {
        if (!isPuzzleFinished()) return false;
        
        _gameManager.SetGameWon();
        return true;
    }
    
    public bool isPuzzleFinished()
    {
        for (var i = 0; i < _tiles.Count; i++)
        {
            var tile = _tiles[i];
            var placementChecker = tile.GetComponent<TilePlacementChecker>();

            if (!placementChecker.IsAtTheRightPos())
            {
                return false;
            }
        }
        return true;
    }
}
