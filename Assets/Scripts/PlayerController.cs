using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Ajustes de Movimiento")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _mouseSensitivity = 2f;
    [SerializeField] private float _gravity = -9.81f;
    [SerializeField] private float _jumpHeight = 1.5f;

    [Header("Interacción con Bloques")]
    [SerializeField] private Camera _cam;
    [SerializeField] private GameObject _cubePrefab;
    [SerializeField] private float _reachDistance = 5f;
    [SerializeField] private LayerMask _blockLayer;

    private CharacterController _controller;
    private Vector3 _velocity;
    private float _xRotation = 0f;

    void Start()
    {
        _controller = GetComponent<CharacterController>();

        // Bloqueamos el cursor para que no se salga de la ventana
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        ManejarMovimiento();
        ManejarCamara();
        ManejarInteraccion();
    }

    private void ManejarMovimiento()
    {
        // Movimiento WASD
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        _controller.Move(move * _moveSpeed * Time.deltaTime);

        // Gravedad y Suelo
        if (_controller.isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }

        // Salto
        if (Input.GetButtonDown("Jump") && _controller.isGrounded)
        {
            _velocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
        }

        _velocity.y += _gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);
    }

    private void ManejarCamara()
    {
        float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity;

        // Rotación Vertical (Cámara)
        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);
        _cam.transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);

        // Rotación Horizontal (Jugador)
        transform.Rotate(Vector3.up * mouseX);
    }

    private void ManejarInteraccion()
    {
        Ray ray = _cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, _reachDistance, _blockLayer))
        {
            // IZQUIERDO: COLOCAR
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 nuevaPos = hit.transform.position + hit.normal;
                GameObject nuevoCubo = Instantiate(_cubePrefab, nuevaPos, Quaternion.identity);

                // Actualizamos el bloque que acabamos de poner y el que está debajo
                nuevoCubo.GetComponent<CubeController>().ActualizarVisibilidad();
                hit.transform.GetComponent<CubeController>().ActualizarVisibilidad();
            }

            // DERECHO: ROMPER
            if (Input.GetMouseButtonDown(1))
            {
                Vector3 posDebajo = hit.transform.position + Vector3.down;
                GameObject aEliminar = hit.transform.gameObject;

                // Antes de borrar, buscamos si hay un bloque debajo
                Collider[] vecinos = Physics.OverlapSphere(posDebajo, 0.1f, _blockLayer);
                foreach (var col in vecinos)
            {
                    CubeController cc = col.GetComponent<CubeController>();
                    if (cc != null)
                    {
                        // Forzamos al de abajo a aparecer ANTES de borrar el de arriba
                        cc.SetEstado(true);
                    }
                }

                Destroy(aEliminar);
            }
        }
    }
}
