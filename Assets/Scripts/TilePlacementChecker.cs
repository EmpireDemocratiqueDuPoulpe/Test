using UnityEngine;

public class TilePos
{
    public int x;
    public int y;

    public TilePos() { x = y = 0; }
}

public class TilePlacementChecker : MonoBehaviour
{
    private TilePos _currentTilePos = new TilePos();
    private TilePos _correctTilePos = new TilePos();

    // Getters
    public TilePos GetCurrentTilePos() { return _currentTilePos; }
    public TilePos GetCorrectTilePos() { return _correctTilePos; }

    // Setters
    public void SetCurrentTilePos(TilePos pos) { _currentTilePos = pos; }
    public void SetCurrentTilePos(int x, int y)
    {
        _currentTilePos.x = x;
        _currentTilePos.y = y;
    }
    
    public void SetCorrectTilePos(TilePos pos) { _correctTilePos = pos; }
    public void SetCorrectTilePos(int x, int y)
    {
        _correctTilePos.x = x;
        _correctTilePos.y = y;
    }
}
