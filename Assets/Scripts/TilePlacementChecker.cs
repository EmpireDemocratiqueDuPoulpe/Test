using UnityEngine;

public class TilePos
{
    public int x;
    public int y;

    public TilePos() { x = y = 0; }
}

public class TilePlacementChecker : MonoBehaviour
{
    /**
     * This script is only used to check if the piece is at the right place.
     * _currentTilePos = Current x / y position in the grid
     * _correctTilePos = x / y position where the tile need to be
     */
    private TilePos _currentTilePos = new TilePos();
    private TilePos _correctTilePos = new TilePos();

    /********************************************************************
     * Current position
     ********************************************************************/
    
    public TilePos GetCurrentTilePos() { return _currentTilePos; }
    
    public void SetCurrentTilePos(TilePos pos) { _currentTilePos = pos; }
    public void SetCurrentTilePos(int x, int y)
    {
        _currentTilePos.x = x;
        _currentTilePos.y = y;
    }
    
    /********************************************************************
     * Correct position
     ********************************************************************/
    
    public TilePos GetCorrectTilePos() { return _correctTilePos; }
    
    public void SetCorrectTilePos(TilePos pos) { _correctTilePos = pos; }
    public void SetCorrectTilePos(int x, int y)
    {
        _correctTilePos.x = x;
        _correctTilePos.y = y;
    }
    
    /********************************************************************
     * Position checker
     ********************************************************************/
    
    public bool IsAtTheRightPos()
    {
        return (_currentTilePos.x == _correctTilePos.x) && (_currentTilePos.y == _correctTilePos.y);
    }
}
