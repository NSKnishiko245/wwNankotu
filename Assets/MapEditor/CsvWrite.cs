using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CsvWrite : MonoBehaviour
{
    private string stageDataPath;

    void Start()
    {
        stageDataPath = Application.dataPath + "/Resources/StageData/";
    }

    public void WriteMapFromCsv(int[,] map, string fileName)
    {
        StreamWriter file = new StreamWriter(stageDataPath + fileName + ".csv", false, System.Text.Encoding.UTF8);
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
    public void WriteBarMapFromCsv(List<int> map, string fileName)
    {
        StreamWriter file = new StreamWriter(stageDataPath + fileName + ".csv", false, System.Text.Encoding.UTF8);
        string rowData = "";
        for (int x = 0; x < map.Count; x++)
        {
            rowData += map[x].ToString();
            rowData += ',';
        }
        file.WriteLine(rowData);
        file.Close();
    }

    public bool ReadMapFromCsv(int[,] map, string fileName)
    {
        string csvFileName = stageDataPath + fileName + ".csv";

        if (!File.Exists(csvFileName)) return false;

        FileInfo fileInfo = new FileInfo(csvFileName);
        StreamReader streamReader = new StreamReader(fileInfo.OpenRead(), System.Text.Encoding.UTF8);

        for (int y = 0; y < map.GetLength(0); y++)
        {
            // 1çsì«Ç›éÊÇË
            string rowData = streamReader.ReadLine();
            // ','Ç≤Ç∆Ç…ãÊêÿÇ¡ÇƒîzóÒÇ÷äiî[
            string[] rowArray = (rowData.Split(','));
            for (int x = 0; x < map.GetLength(1); x++)
            {
                map[y, x] = int.Parse(rowArray[x]);
            }
        }
        return true;
    }
    public bool ReadBarMapFromCsv(List<int> map, string fileName)
    {
        string csvFileName = stageDataPath + fileName + ".csv";

        if (!File.Exists(csvFileName)) return false;

        FileInfo fileInfo = new FileInfo(csvFileName);
        StreamReader streamReader = new StreamReader(fileInfo.OpenRead(), System.Text.Encoding.UTF8);
        // 1çsì«Ç›éÊÇË
        string rowData = streamReader.ReadLine();
        // ','Ç≤Ç∆Ç…ãÊêÿÇ¡ÇƒîzóÒÇ÷äiî[
        string[] rowArray = (rowData.Split(','));
        for (int x = 0; x < map.Count; x++)
        {
            map[x] = int.Parse(rowArray[x]);
        }
        return true;
    }
}
