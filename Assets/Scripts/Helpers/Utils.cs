using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils {
    public static bool FindPlayer(ref GameObject player) {
        const string playerTag = "Player";
        var p = GameObject.FindGameObjectWithTag(playerTag);
        if (p) player = p;
        return p;
    }

    public static bool FindPlayer(ref Transform player) {
        GameObject p = null;
        if (FindPlayer(ref p)) {
            player = p.transform;
        }
        return p;
    }
}
