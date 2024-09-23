using UnityEngine;
using UnityEngine.Tilemaps;

public class MapSystem : MonoBehaviour
{
    public Tilemap gameplayTilemap;  // 游戏中的实际 Tilemap
    public Tilemap mapTilemap;       // 用于显示地图的 Tilemap
    public Tile mapTile;             // 用于在地图上标记已探索区域的 Tile
    public Transform player;         // 玩家的 Transform
    public Camera mapCamera;         // 用于渲染地图的相机
    public KeyCode toggleMapKey = KeyCode.M;  // 切换地图显示的按键

    private bool isMapVisible = false;
    private Vector3Int lastPlayerTilePosition;

    private void Start()
    {
        if (mapCamera != null)
        {
            mapCamera.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        UpdateExploredArea();
        HandleMapToggle();
    }

    private void UpdateExploredArea()
    {
        if (gameplayTilemap == null || mapTilemap == null || player == null) return;

        Vector3Int playerTilePosition = gameplayTilemap.WorldToCell(player.position);

        // 只在玩家移动到新的 Tile 时更新
        if (playerTilePosition != lastPlayerTilePosition)
        {
            lastPlayerTilePosition = playerTilePosition;

            // 在玩家周围的一定范围内标记为已探索
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    Vector3Int tileToCheck = playerTilePosition + new Vector3Int(x, y, 0);
                    if (gameplayTilemap.HasTile(tileToCheck))
                    {
                        mapTilemap.SetTile(tileToCheck, mapTile);
                    }
                }
            }
        }
    }

    private void HandleMapToggle()
    {
        if (Input.GetKeyDown(toggleMapKey))
        {
            isMapVisible = !isMapVisible;
            if (mapCamera != null)
            {
                mapCamera.gameObject.SetActive(isMapVisible);
            }
        }
    }

    // 可以在这里添加更多地图相关的功能，比如标记特殊位置等
}