using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrikerControlScript : MonoBehaviour
{
    public GameObject rBCircleLeft;
    public GameObject rBCircleRight;
    public int playerId;

    /*
        state 0: ready to be launched.
        state 1: launched.
        state 2: it has stopped
    */

    private const int STATE_AIMING = 0;
    private const int STATE_MOVING = 1;
    private const int STATE_RESET = 2;
    private int state;

    private Vector3 mousePosition;
    

    // Start is called before the first frame update
    void Start()
    {
        state = STATE_RESET;
        mousePosition = Vector3.negativeInfinity;
    }

    // Update is called once per frame
    void Update() {

    }


    void FixedUpdate()
    {            
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (state == STATE_AIMING) {
            if (transform.localScale != Vector3.one) {
                transform.localScale = Vector3.one;  // reset scale if it has changed.
            }
            GetComponent<CircleCollider2D>().isTrigger = true;  // make striker a trigger before release
            ConstrainStriker();
            TraceMouse();
        } else if (state == STATE_MOVING) {
            // check if the board is stagnant
            // first check the striker
            if (rb.velocity.magnitude <= Global.effZeroVelocity) {
                // now check if all the men are stagnant
                Transform mcaXForm = Global.manCommonAncestor.transform;
                bool allAtRest = true;
                for (int i = 0; i < mcaXForm.childCount; i++) {
                    GameObject m = mcaXForm.GetChild(i).gameObject;
                    Rigidbody2D b = m.GetComponent<Rigidbody2D>();
                    if (b.velocity.magnitude > Global.effZeroVelocity) {
                        allAtRest = false; break;
                    }
                }
                if (allAtRest) { 
                    rb.angularVelocity = 0; 
                    Global.currentPlayer = (Global.currentPlayer + 1) % Global.maxPlayers;
                    state = STATE_RESET; 
                }
            }
        } else {
            if (transform.localScale == Vector3.zero || (!Global.pocketStack.Contains(this.gameObject) && transform.localScale == Vector3.one)) {
                if (Global.currentPlayer != playerId) {
                    transform.position = Global.outOfBoardPosition;
                } else {
                    state = STATE_AIMING;
                }    
            }
            
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

    void TraceMouse() {
        Vector3 mp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mp.z = 0;
        if (state == STATE_AIMING 
            && (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))) {
            if (mousePosition == Vector3.negativeInfinity) {
                mousePosition = mp;
            } else {
                // compute line the mouse has made between each frame
                float slope = (mp.y - mousePosition.y) / (mp.x - mousePosition.x);
                float intersect = (mousePosition.y - slope * mousePosition.x);
                // compute quadratic formula to check if there's an intersection and find the point of intersection
                float   a = 1 + Mathf.Pow(slope, 2), 
                        b = -2 * transform.position.x 
                            + 2 * slope * intersect
                            - 2 * transform.position.y * slope, 
                        c = Mathf.Pow(intersect, 2)
                            - 2 * transform.position.y * intersect
                            + Mathf.Pow(transform.position.x, 2)
                            + Mathf.Pow(transform.position.y, 2)
                            - Mathf.Pow(Global.carromStrikerDiameter / 2.0f, 2);
                float disc = Mathf.Pow(b, 2) - 4*a*c;  // the discriminant
                Vector3 next = mp;
                if (disc >= 0) {  // the mouse line intersects with the striker
                    // find point of intersection by computing quadratic formula
                    float   px1 = (-b - Mathf.Sqrt(disc)) / (2.0f * a),
                            px2 = (-b + Mathf.Sqrt(disc)) / (2.0f * a);  
                    // the point that is closer to the original mouse position is the point of contact
                    float px;
                    if (px1 == px2 || Mathf.Abs(mousePosition.x - px1) < Mathf.Abs(mousePosition.x - px2)) {
                        px = px1;
                    } else {
                        px = px2;
                    }

                    float minX = mousePosition.x < mp.x ? mousePosition.x : mp.x;
                    float maxX = mousePosition.x > mp.x ? mousePosition.x : mp.x;

                    if (px >= minX && px <= maxX) {
                        float py = slope * px + intersect;
                        Vector2 position = new Vector2(px, py);
                        Vector2 velocity = new Vector2(mp.x - mousePosition.x, mp.y - mousePosition.y) / Time.fixedDeltaTime;
                        Vector2 appForce = velocity * Global.unitForce;

                        Rigidbody2D rb = GetComponent<Rigidbody2D>();
                        rb.AddForceAtPosition(appForce, position, ForceMode2D.Impulse);
                        GetComponent<CircleCollider2D>().isTrigger = false;  // make a rigidbody collider upon release
                        state = STATE_MOVING;
                        next = Vector3.negativeInfinity;
                    }
                } 
                mousePosition = next;
            }
        } else {
            mousePosition = Vector3.negativeInfinity;
        }
    }
/*
    void OnMouseEnter() {
        if (state == STATE_AIMING && (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))) {
            Vector2 appForce = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * Global.unitForce;
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.AddForce(appForce, ForceMode2D.Impulse);
            GetComponent<CircleCollider2D>().isTrigger = false;  // make a rigidbody collider upon release
            state = STATE_MOVING;
        }
    }
*/
    void OnTriggerStay2D(Collider2D other) {
        if (state == STATE_AIMING) {
            Transform rbclXForm = rBCircleLeft.transform;
            Transform rbcrXForm = rBCircleRight.transform;
            Transform othXForm = other.gameObject.transform;
            if (rbclXForm.position.y == rbcrXForm.position.y) {
                float dx = transform.position.x - othXForm.position.x;

                if (dx < 0) {  // then user wanted to likely go right
                    transform.position += new Vector3(1.5f*Global.carromManDiameter - dx, 0, 0);
                } else {  // then he wanted to go left
                    transform.position += new Vector3(-1.5f*Global.carromManDiameter - dx, 0, 0);
                }
            } else {
                float dy = transform.position.y - othXForm.position.y;

                if (dy < 0) {
                    transform.position += new Vector3(0, 1.5f*Global.carromManDiameter - dy, 0);
                } else {  // then he wanted to go left
                    transform.position += new Vector3(0, -1.5f*Global.carromManDiameter - dy, 0);
                }
            }
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
        if (sx <= minX + Global.baseCircleDiameter) { adj.x = minX - sx; }
        else if (sx >= maxX - Global.baseCircleDiameter) { adj.x = maxX - sx; }
        if (sy <= minY + Global.baseCircleDiameter) { adj.y = minY - sy; }
        else if (sy >= maxY - Global.baseCircleDiameter) { adj.y = maxY - sy; }
        transform.position += adj;  // change the striker transform directly
    }
}
