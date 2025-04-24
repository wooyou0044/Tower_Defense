using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float zoomSpeed;
    [SerializeField] float minZoomZ;
    [SerializeField] float maxZoomZ;
    [SerializeField] float moveSpeed;
    [SerializeField] float edgeSize;
    [SerializeField] float moveDistance;
    [SerializeField] float rotateSpeed;
    [SerializeField] MapManager mapManager;

    Camera cam;

    float camSize;

    Terrain terrain;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void Start()
    {
        terrain = Terrain.activeTerrain;
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 moveDir = Vector3.zero;

        if(Input.mousePosition.x >= Screen.width - edgeSize)
        {
            moveDir += Vector3.right;
        }
        if(Input.mousePosition.x <= edgeSize)
        {
            moveDir += Vector3.left;
        }
        if(Input.mousePosition.y >= Screen.height - edgeSize)
        {
            moveDir += Vector3.up;
        }
        if(Input.mousePosition.y <= edgeSize)
        {
            moveDir += Vector3.back;
        }

        Vector3 move = (new Vector3(h, v, 0) + moveDir) * moveSpeed * Time.deltaTime;
        transform.Translate(move, Space.World);

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if(scroll != 0)
        {
            cam.orthographicSize -= scroll * zoomSpeed;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoomZ, maxZoomZ);
        }

        camSize = cam.orthographicSize;

        if(Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(Vector3.up, -rotateSpeed * Time.deltaTime, Space.World);
        }

        if(Input.GetKey(KeyCode.E))
        {
            transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);
        }

    }

    void LateUpdate()
    {
        
    }
}
