using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class Helper {

    public static string GetToolkitDirectory() {
        string path = Application.dataPath;
        for (int i=0; i<2; i++) {
            path = path.Substring(0, path.LastIndexOf('/'));
        }
        return path + "/";
    }

    public static IEnumerable<string> GetFilesRecursive(string path) {
        Queue<string> queue = new Queue<string>();
        queue.Enqueue(path);
        while (queue.Count > 0) {
            path = queue.Dequeue();
            try {
                foreach (string subDir in System.IO.Directory.GetDirectories(path)) {
                    queue.Enqueue(subDir);
                }
            } catch (Exception ex) {
                //Console.Error.WriteLine(ex);
            }
            string[] files = null;
            try {
                files = System.IO.Directory.GetFiles(path);
            } catch (Exception ex) {
                //Console.Error.WriteLine(ex);
            }
            if (files != null) {
                for (int i = 0; i < files.Length; i++) {
                    yield return files[i];
                }
            }
        }
    }
}