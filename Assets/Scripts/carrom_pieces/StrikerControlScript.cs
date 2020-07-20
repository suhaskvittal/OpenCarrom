using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrikerControlScript : MonoBehaviour
{
    public GameObject rBCircleLeft;
    public GameObject rBCircleRight;

    /*
        state 0: ready to be launched.
        state 1: launched.
        state 2: has stopped on the board.
        state 3: has stopped in a pocket.
    */
    private int state;
    

    // Start is called before the first frame update
    void Start()
    {
        state = 0;
    }

    // Update is called once per frame
    void Update() {

    }


    void FixedUpdate()
    {
        if (state == 0) {
            ConstrainStriker();
        } else if (state == 1) {
            
        }
    }

    void OnMouseDrag() {
        if (state == 0) {
            Vector3 mp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mp.z = 0;
            transform.position = mp;
            ConstrainStriker();
        }
    }

    void OnMouseEnter() {
        if (state == 0 && Input.GetMouseButton(0) && (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))) {
            Vector2 appForce = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * Constants.unitForce;
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.AddForce(appForce, ForceMode2D.Impulse);
            state = 1;
        }
    }

    private void ConstrainStriker() {
        Transform strkXForm = GetComponent<Transform>();  // make sure the striker is in a valid region
        Transform rbclXForm = rBCircleLeft.GetComponent<Transform>();
        Transform rbcrXForm = rBCircleRight.GetComponent<Transform>();

        float sx = strkXForm.position.x, sy = strkXForm.position.y;
        float minX = rbclXForm.position.x, maxX = rbcrXForm.position.x;
        float minY = rbclXForm.position.y, maxY = rbcrXForm.position.y;  

        Vector3 adj = Vector3.zero;
        if (sx <= minX + Constants.baseCircleDiameter) { adj.x = minX - sx; }
        else if (sx >= maxX - Constants.baseCircleDiameter) { adj.x = maxX - sx; }
        if (sy <= minY + Constants.baseCircleDiameter) { adj.y = minY - sy; }
        else if (sy >= maxY - Constants.baseCircleDiameter) { adj.y = maxY - sy; }
        transform.position += adj;  // change the striker transform directly
    }
}
