using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChildColliderController : MonoBehaviour
{

    private PlayerController playerController;

    void Start() {
        playerController = transform.parent.gameObject.GetComponent<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        playerController.OnChildTriggerEnter2D(other);
    }

    private void OnTriggerExit2D(Collider2D other) {
        playerController.OnChildTriggerExit2D(other);
    }
}
