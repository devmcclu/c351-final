using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour
{
    //time it will take to move
    public float moveTime = 0.1f;
    //Layer for collision checks
    public LayerMask blockingLayer;

    //Components the object has
    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;
    //Float to make calculations for moving easier
    private float inverseMoveTime;


    // Start is called before the first frame update
    protected virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        //Get the inverse of moveTime to make calculations easier
        inverseMoveTime = 1f / moveTime;
    }

    //Return true if able to move, false if can't move
    protected bool Move(int xDir, int yDir, out RaycastHit2D hit)
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);

        boxCollider.enabled = false;
        hit = Physics2D.Linecast(start, end, blockingLayer);
        boxCollider.enabled = true;

        //Check if anything was hit
        if(hit.transform == null)
        {
            //Start moving if nothing was hit
            StartCoroutine(SmoothMovement(end));
            return true;
        }
        //Something was hit
        return false;
    }

    //Co-routine for moving units from one space to the next
    protected IEnumerator SmoothMovement(Vector3 end)
    {
        //Calculate remaining distance to end point
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        //While distance is greater than a small amount
        while (sqrRemainingDistance > float.Epsilon)
        {
            //Find place that is closer to end point
            Vector3 newPostion = Vector3.MoveTowards (rb2D.position, end, inverseMoveTime * Time.deltaTime);
            //Move ot new position
            rb2D.MovePosition(newPostion);
            //Find the remaining distance
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            yield return null;
        }
    }

    protected virtual void AttemptMove <T> (int xDir, int yDir)
        where T : Component
    {
        //Whatever the linecast hits
        RaycastHit2D hit;
        //Set to whatever the outcome of Move was
        bool canMove = Move(xDir, yDir, out hit);
        
        //Check if nothing was hit
        if(hit.transform == null)
        {
            //Don't do anything else
            return;
        }    

        //Get a reference to the component of type T attached to the object hit
        T hitComponent = hit.transform.GetComponent<T>();
        if (!canMove && hitComponent != null)
        {
            //call OnCantMove if the MovingObject hit something
            OnCantMove(hitComponent);
        }
    }

    protected abstract void OnCantMove <T> (T component)
        where T : Component;
}
