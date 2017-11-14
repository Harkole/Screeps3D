﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RequestBody : Dictionary<string, string> {

    public string GetQueryString() {
        var count = 0;
        var str = "?";
        foreach (var kvp in this) {
            str += string.Format("{0}={1}", kvp.Key, kvp.Value);
            count++;
            if (count < this.Count) {
                str += "&";
            }
        }
        return str;
    }
}