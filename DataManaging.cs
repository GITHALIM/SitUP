/**
 * standard 배열에 임의값 넣어놨음 수정바람
 */
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.UI;


public class DataManaging : MonoBehaviour
{
    public InputField InputField;

    float[] standard = new float[] { 0, 0, 0, 5, 20, 33, 30 };
    public float[] newData = new float[7];
    public int[] gradeToday = new int[8];  // 연, 월, 일, 5단계
    float dailyTotal;

    public int currentGrade;

    // Start is called before the first frame update
    public void Save(string input)
    {
        //  입력된 데이터를 저장하는 과정
        FileEditing("Data", input);

        //  string 배열을 float 배열로 바꾸기
        newData = (float[])StringToFloat(input).Clone();

        // 등급 나누는 과정
        float total = Calculating(newData);
        int grade = Grading(total);

        //  일이 바뀌었는지 확인

        bool dayTest = (newData[Constants.idxYear] == gradeToday[Constants.idxYear]) && (newData[Constants.idxMonth] == gradeToday[Constants.idxMonth])&&(newData[Constants.idxDay] == gradeToday[Constants.idxDay]);

        if (!dayTest)
        {
            float n = 0;

            for(int i = Constants.idxGrade; i < gradeToday.Length; i++)
            {
                n += gradeToday[i];
            }
            InitDailyGrade(dailyTotal / n);
            dailyTotal = 0;
            dailyTotal += (100 - total);
            IncreasingGrade(grade);
        }
        else
        {
            /** 같은 날이라는 뜻!
             *  오늘 데이터 건들이기 
             **/
            IncreasingGrade(grade);
            dailyTotal += (100 - total);
        }
    }

    public void InitFile()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            try
            {
                StreamWriter m1file = new StreamWriter(Application.persistentDataPath + "/" + "Data" + ".csv", true);
                StreamWriter m2file = new StreamWriter(Application.persistentDataPath + "/" + "DailyData" + ".csv", true);
                StreamWriter m3file = new StreamWriter(Application.persistentDataPath + "/" + "MonthlyData" + ".csv", true);
                m1file.Close();
                m2file.Close();
                m3file.Close();

                Debug.Log("file successfully made");
            }
            catch (DirectoryNotFoundException e)
            {
                Debug.Log("failed to make file");
            }
        }
    }

    void FileEditing(string fileName, string str)
    {
        StreamWriter file = new StreamWriter(@"Assets/Data/" + fileName + ".csv", true);
        file.WriteLine(str);
        file.Close();
    }

    void InitArray(int[] arr)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = 0;
        }
    }

    float Calculating(float[] arr)
    {
        //  새로운 값 계산하기
        float total = 0;
        for (int i = Constants.idxGrade; i < arr.Length; i++)
        {
            float sum = (arr[i] - standard[i]);
            if (sum < 0) sum = sum * (-1);
            sum = (sum / standard[i]) * 100;
            total += sum;                     
        }

        total = total / (newData.Length - 3);
        return total;
    }

    int Grading(float total)
    {
        //  등급 나누기

        int grade;  // 등급 배열의 index로 활용
        if (total < 10) grade = 3;
        else if (total < 40) grade = 4;
        else if (total < 60) grade = 5;
        else if (total < 80) grade = 6;
        else
        {
            grade = 7;
        }

        return grade;
    }

    void InitDailyGrade(float total)
    {
        string dailyData = string.Join(",", gradeToday);
        string str = string.Join(",", total.ToString());
        FileEditing("DailyData", str);

        InitArray(gradeToday);

        gradeToday[Constants.idxYear] = (int)newData[Constants.idxYear];
        gradeToday[Constants.idxMonth] = (int)newData[Constants.idxMonth];
        gradeToday[Constants.idxDay] = (int)newData[Constants.idxDay];

    }

    void IncreasingGrade(int grade)
    {
        gradeToday[grade]++;
    }

    public float[] StringToFloat(string input)
    {
        var data_values = input.Split(',');
        for (int i = 0; i < data_values.Length; i++)
        {
            newData[i] = float.Parse(data_values[i]);
        }
        return newData;
    }

}
