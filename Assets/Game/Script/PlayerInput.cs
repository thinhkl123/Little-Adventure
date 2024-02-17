using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public float horizontalInput;
    public float verticalInput;
    public bool mouseButtonDown;
    public bool spaceButtonDown;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!mouseButtonDown && Time.timeScale != 0)
        {
            mouseButtonDown = Input.GetMouseButtonDown(0);
        }

        if (!spaceButtonDown && Time.timeScale != 0)
        {
            spaceButtonDown = Input.GetKeyDown(KeyCode.Space);
        }
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void OnDisable()
    {
        ClearCache();
    }

    public void ClearCache()
    {
        mouseButtonDown = false;
        spaceButtonDown = false;
        horizontalInput = 0;
        verticalInput = 0;
    }
}
