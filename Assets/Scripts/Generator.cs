using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

[System.Serializable]
public struct TerrainLayer
{
    public string name;
    public Material material;
    public float maxHeight; // Altura hasta la que llega este material (0 a 1)
}

public class Generator : MonoBehaviour
{
    [SerializeField] private GameObject _cubePrefab;
    [SerializeField] private int _width, _large, _seed;
    [SerializeField] private float _detail;
    [SerializeField] private GameObject _player;


    [Header("Configuración de Capas")]
    [SerializeField] private List<TerrainLayer> _layers;

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
                float noise = Mathf.PerlinNoise((x + _seed) / _detail, (z + _seed) / _detail);
                int currentHeight = (int)(noise * _detail);

                for (int y = 0; y < currentHeight; y++)
                {
                    GameObject cube = Instantiate(_cubePrefab, new Vector3(x, y, z), Quaternion.identity);

                    // Calculamos el porcentaje de altura actual (0 a 1)
                    float heightPercent = (float)y / _detail;

                    // Asignamos el material según la capa
                    Material selectedMat = GetMaterialForHeight(heightPercent);
                    cube.GetComponent<Renderer>().material = selectedMat;

                    // IMPORTANTE: Ponle el Tag o Layer para que el Player lo reconozca
                    cube.layer = LayerMask.NameToLayer("Blocks");
                }
            }
        }
        PosicionarJugadorEnSuperficie();
    }

    private void PosicionarJugadorEnSuperficie()
    {
        if (_player == null) return;

        // Centro del mapa
        float spawnX = _width / 2f;
        float spawnZ = _large / 2f;

        // Calculamos la altura exacta de ese punto
        float noise = Mathf.PerlinNoise((spawnX + _seed) / _detail, (spawnZ + _seed) / _detail);
        float spawnY = (noise * _detail) + 2f; // +2 para que no se entierre

        CharacterController cc = _player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false; // IMPORTANTE: Apagar para mover

        _player.transform.position = new Vector3(spawnX, spawnY, spawnZ);

        if (cc != null) cc.enabled = true; // Volver a encender
    }

    private Material GetMaterialForHeight(float height)
    {
        foreach (var layer in _layers)
        {
            if (height <= layer.maxHeight)
                return layer.material;
        }
        // Si supera todas las capas, devuelve la última
        return _layers[_layers.Count - 1].material;
    }
}
