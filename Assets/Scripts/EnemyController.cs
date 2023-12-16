using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : NetworkBehaviour
{
    private GameObject[] targets;
    private PlayerController target; //accessing player info
    private Vector3 direction;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(UpdateTargets());
    }
    IEnumerator UpdateTargets() {
        while (true) {
            FindTargets();
            yield return new WaitForSeconds(2);
        }
    }
    void FindTargets()
    {
        targets = GameObject.FindGameObjectsWithTag("Player");
        SetClosestTarget();
    }
    void SetClosestTarget() {
        float smallestDistance = 30;
        for (int i = 0; i < targets.Count();i++) {
            float distance = Vector3.Distance(transform.position, targets[i].transform.position);
            if (distance < smallestDistance)
            {
                smallestDistance = distance;
                target = targets[i].GetComponent<PlayerController>();
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (target!=null) {
            direction = target.transform.position - transform.position;
            direction.y = 0;
            transform.position += direction.normalized*speed*Time.deltaTime;


            if (direction != Vector3.zero)
                transform.forward = Vector3.Slerp(transform.forward, direction.normalized, Time.deltaTime * 10);

        }
    }
}
