using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private GameObject m_tilePrefab;
    [SerializeField] private int m_gridSize = 8;
    private GameObject map;
    private List<GameObject> m_allTiles;

    void SetupGrid()
    {
        m_allTiles = new List<GameObject>();

        map = new GameObject("MapTiles");

        for (int x = 0; x < m_gridSize; x++)
        {
            for (int y = 0; y < m_gridSize; y++)
            {
                GameObject tile = Instantiate(m_tilePrefab, new Vector3(x, y, 0f), Quaternion.identity);
                tile.name = "Tile (" + x.ToString() + ", " + y.ToString() + ")";
                tile.transform.SetParent(map.transform);

                m_allTiles.Add(tile);

                if (y % 2 == 0 && x % 2 == 0)
                {
                    tile.GetComponent<SpriteRenderer>().enabled = false;
                }

                if (y % 2 == 1 && x % 2 == 1)
                {
                    tile.GetComponent<SpriteRenderer>().enabled = false;
                }
            }
        }
    }

    int m_count = 0;

    private void OnEnable()
    {
        BeatManager.SubscribeToBeatEvent(OnBeatHandler);
        SetupGrid();
    }

    private void OnDisable()
    {
        BeatManager.UnsubscribeFromBeatEvent(OnBeatHandler);
        Destroy(map);
    }

    private void OnBeatHandler()
    {
        m_count += 1;

        if (m_count > 1)
        {
            foreach (GameObject obj in m_allTiles)
            {
                SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
                sr.enabled = !sr.enabled;
                //obj.SetActive(!obj.activeInHierarchy);
                LeanTween.scale(obj, Vector3.one, BeatManager.I.GetIntervalLength(BeatType.Beat)).setEaseInOutBack().setOnComplete(() =>
                {
                    LeanTween.scale(obj, Vector3.one * .8f, BeatManager.I.GetIntervalLength(BeatType.Beat)).setEaseInOutBack();
                });
            }

            m_count = 0;
        }
    }
}
