using UnityEngine;
using UnityEngine.Tilemaps;

public class MapSystem : MonoBehaviour
{
    public Tilemap gameplayTilemap;  // ��Ϸ�е�ʵ�� Tilemap
    public Tilemap mapTilemap;       // ������ʾ��ͼ�� Tilemap
    public Tile mapTile;             // �����ڵ�ͼ�ϱ����̽������� Tile
    public Transform player;         // ��ҵ� Transform
    public Camera mapCamera;         // ������Ⱦ��ͼ�����
    public KeyCode toggleMapKey = KeyCode.M;  // �л���ͼ��ʾ�İ���

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

        // ֻ������ƶ����µ� Tile ʱ����
        if (playerTilePosition != lastPlayerTilePosition)
        {
            lastPlayerTilePosition = playerTilePosition;

            // �������Χ��һ����Χ�ڱ��Ϊ��̽��
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

    // ������������Ӹ����ͼ��صĹ��ܣ�����������λ�õ�
}