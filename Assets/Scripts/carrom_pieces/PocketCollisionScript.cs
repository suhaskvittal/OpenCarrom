using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PocketCollisionScript : MonoBehaviour
{
    private float maxSpeedThreshold = 40.0f;  // maximum speed for a piece to drop in a pocket

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerStay2D(Collider2D other) {
        GameObject g = other.gameObject;

        Rigidbody2D rb = g.GetComponent<Rigidbody2D>();
        if (rb.velocity.magnitude <= maxSpeedThreshold && WillCollapse(g)) {
            Global.pocketStack.Push(g); 
            rb.velocity = Vector2.zero;
            g.transform.position = transform.position;
        }
    }

    private bool WillCollapse(GameObject g) {
        CircleCollider2D cc = GetComponent<CircleCollider2D>();

        float dx = transform.position.x - g.transform.position.x;
        float dy = transform.position.y - g.transform.position.y;

        return Mathf.Abs(dx) < cc.radius && Mathf.Abs(dy) < cc.radius; 
    }
}
