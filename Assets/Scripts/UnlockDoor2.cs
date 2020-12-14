using Assets.Scripts.Enemy;
using Assets.Scripts.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockDoor2 : MonoBehaviour {

    public Health[] Enemies;

    private int Count = 0, Length = 0;

    void Start() {
        Length = Enemies.Length;
        if (Length == 0) {
            Debug.LogWarning("Door won't work without enemies!");
        }

        foreach (var e in Enemies) {
            if (e) {
                e.OnDeath += OnEnemyDeath;
            }
        }
    }

    private void OnEnemyDeath() {
        Count++;
        if (Count == Length) {
            Debug.Log("Door is opened!");
            Destroy(gameObject, .2f);
        } else {
            Debug.Log($"{Length - Count} more enemies to open the door.");
        }
    }

    // Update is called once per frame
    //void Update()
    //{
    //}
}
