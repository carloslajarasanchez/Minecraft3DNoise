using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    [SerializeField] private GameObject _cubePrefab;
    [SerializeField] private int _width, _height, _large, _seed;
    [SerializeField] private float _detail;
    // Start is called before the first frame update
    void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int z = 0; z < _large; z++)
            {
                _height = (int)(Mathf.PerlinNoise((x / 2 + _seed) / _detail, (z/2+_seed)/ _detail) * _detail);
                for(int y = 0; y < _height; y++)
                {
                    Instantiate(_cubePrefab, new Vector3(x, y, z), Quaternion.identity);
                }
            }
        }
    }
}
