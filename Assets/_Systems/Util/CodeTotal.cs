using System;
using System.IO;
using UnityEngine;
using UnityEditor;

public class StatisticLine
{
    [MenuItem("输出总代码行数/输出")]
    private static void PrintTotalLine()
    {
        string[] fileName = Directory.GetFiles("Assets/_Systems", "*.cs", SearchOption.AllDirectories);

        int totalLine = 0;
        foreach (var temp in fileName)
        {
            int nowLine = 0;
            StreamReader sr = new StreamReader(temp);
            while (sr.ReadLine() != null)
            {
                nowLine++;
            }
            sr.Dispose();
            //文件名+文件行数
            Debug.Log(String.Format("文件 {0}—— +行数 {1}", temp, nowLine));

            totalLine += nowLine;
        }

        Debug.Log(String.Format("总代码行数：{0}", totalLine));
    }
}