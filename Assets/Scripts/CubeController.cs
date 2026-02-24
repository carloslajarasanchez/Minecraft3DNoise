using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    private MeshRenderer _mesh;
    private Collider _col;

    void Awake()
    {
        _mesh = GetComponent<MeshRenderer>();
        _col = GetComponent<Collider>();
    }

    // Este método decide si el bloque debe verse o no
    public void ActualizarVisibilidad()
    {
        // Lanzamos un rayo muy corto hacia ARRIBA
        // Si hay un bloque justo encima, este bloque se oculta
        if (Physics.Raycast(transform.position, Vector3.up, out RaycastHit hit, 1.1f))
        {
            // Solo se oculta si lo que tiene arriba es OTRO bloque
            if (hit.collider.CompareTag("Block") || hit.collider.gameObject.layer == LayerMask.NameToLayer("Blocks"))
            {
                SetEstado(false);
                return;
            }
        }

        // Si no hay nada arriba, debe ser visible
        SetEstado(true);
    }

    public void SetEstado(bool visible)
    {
        if (_mesh == null) _mesh = GetComponent<MeshRenderer>();
        if (_col == null) _col = GetComponent<Collider>();

        _mesh.enabled = visible;
        _col.enabled = true; // El collider SIEMPRE debe estar activo para poder poner bloques encima
    }

    void Start()
    {
        // Un pequeño retraso para asegurar que todos los bloques se han creado
        Invoke("ActualizarVisibilidad", 0.1f);
    }
}
