using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CsvWrite : MonoBehaviour
{
    public void WriteMapFromCsv(int[,] map, string fileName)
    {
        StreamWriter file = new StreamWriter(Application.dataPath + "/Resources/StageData/" + fileName + ".csv", false, System.Text.Encoding.UTF8);
        for (int y = 0; y < map.GetLength(0); y++)
        {
            string rowData = "";
            for (int x = 0; x < map.GetLength(1); x++)
            {
                rowData += map[y, x].ToString();
                rowData += ',';
            }
            file.WriteLine(rowData);
        }
        file.Close();
    }

    public bool ReadMapFromCsv(int[,] map, string fileName)
    {
        string csvFileName = Application.dataPath + "/Resources/StageData/" + fileName + ".csv";

        if (!File.Exists(csvFileName)) return false;

        FileInfo fileInfo = new FileInfo(csvFileName);
        StreamReader streamReader = new StreamReader(fileInfo.OpenRead(), System.Text.Encoding.UTF8);
        for (int y = 0; y < map.GetLength(0); y++)
        {
            // 1s“Ç‚ÝŽæ‚è
            string rowData = streamReader.ReadLine();
            // ','‚²‚Æ‚É‹æØ‚Á‚Ä”z—ñ‚ÖŠi”[
            string[] rowArray = (rowData.Split(','));
            for (int x = 0; x < map.GetLength(1); x++)
            {
                map[y, x] = int.Parse(rowArray[x]);
            }
        }
        return true;
    }
}
