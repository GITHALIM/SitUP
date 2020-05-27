/**
 * standard 배열에 임의값 넣어놨음 수정바람
 */
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;


public class DataManaging : MonoBehaviour
{
    string input;

    float[] standard = new float[] { 0, 0, 5, 20, 33, 30 };
    public float[] newData = new float[6];
    public int currentGrade;
    public int[] gradeToday = new int[7];  // 월, 일, 5단계
    public float monthlyTotal;
    public float dailyTotal;
    public int monthlyCount;
    public int dailyCount;

    public static DataManaging DataManaging_Instance;

    private void Awake()
    {
        DataManaging_Instance = this;
    }

    public void ProcessData(string recievedData)
    {
        if (recievedData == input)//이전 값과 같다면 입력을 받지 않음. 일단 주기적 입력 전까지는 이렇게 적용
            return;

        input = recievedData;

        //  입력된 데이터를 저장하는 과정
        FileEditing("Data", input);

        //  string 배열을 float 배열로 바꾸기
        var data_values = input.Split(',');
        for (int i = 0; i < data_values.Length; i++)
        {
            newData[i] = float.Parse(data_values[i]);
        }
        
        float total = Calculating();

        // 등급 나누는 과정
        int grade = Grading(total);
        currentGrade = grade;

        //  일이 바뀌었는지 확인
        bool monthTest = (newData[0] == gradeToday[0]);
        bool dayTest = (newData[1] == gradeToday[1]);

        if (!dayTest)
        {
            //  월이 바뀌었는지 확인
            if (!monthTest)
            {
                /**
                 * 달이 바뀌었다.
                 * 월별 데이터 일 별 데이터 모두 파일에 쓰고 배열은 초기화해준다.
                 * 새로운 데이터 시작
                 * */
                string dailyData = string.Join(",", gradeToday[0], gradeToday[1], dailyTotal / dailyCount);
                FileEditing("DailyData", dailyData);
                dailyTotal = 0;
                dailyCount = 0;

                string monthlyData = string.Join(",", gradeToday[0], monthlyTotal / monthlyCount);
                FileEditing("MonthlyData", monthlyData);
                monthlyTotal = 0;
                monthlyCount = 0;

                InitDailyGrade();

                gradeToday[grade]++;
                dailyTotal += total;
                dailyCount++;
                monthlyTotal += total;
                monthlyCount++;
            }
            else
            {
                /**
                 * 같은 달에 날짜만 바뀌었다
                 * 일별 데이터를 파일에 쓰고 초기화한다.
                 * 다시 들어온 데이터를 저장하기
                 * 월 별 데이터는 건들이기만 한다.
                 * */
                string dailyData = string.Join(",", newData[0],newData[1],dailyTotal/dailyCount);
                FileEditing("DailyData", dailyData);
                dailyTotal = 0;
                dailyCount = 0;

                InitDailyGrade();

                gradeToday[grade]++;
                dailyTotal += total;
                dailyCount++;
                monthlyTotal += total;
                monthlyCount++;
            }
        }
        else
        {
            /** 같은 날이라는 뜻!
             *  오늘 데이터, 이번 달 데이터 건들이기 
             **/
            gradeToday[grade]++;
            dailyTotal += total;
            dailyCount++;
            monthlyTotal += total;
            monthlyCount++;
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

    float Calculating()
    {
        float total = 0;
        for (int i = 2; i < newData.Length; i++)
        {
            float sum = (newData[i] - standard[i]);
            if (sum < 0) sum = sum * (-1);
            sum = (sum / newData[i]) * 100;
            total += sum;
        }
        total = total / (newData.Length - 2);

        return total;
    }

    int Grading(float total)
    {
        //  등급 나누기

        int grade;  // 등급 배열의 index로 활용
        if (total < 10) grade = 2;
        else if (total < 40) grade = 3;
        else if (total < 60) grade = 4;
        else if (total < 80) grade = 5;
        else
        {
            grade = 6;
        }

        return grade;
    }

    void IncreasingGrade(int grade)
    {
        gradeToday[grade]++;
    }

    void InitDailyGrade()
    {
        InitArray(gradeToday);

        gradeToday[0] = (int)newData[0];
        gradeToday[1] = (int)newData[1];
    }
}
