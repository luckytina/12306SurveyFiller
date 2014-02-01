﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SurveyFiller
{
    class Program
    {
        static void Main(string[] args)
        {
            SurveyControl sc = new SurveyControl();
            Console.WriteLine("请输入待填问卷列表所位于的Excel文件的文件名（目前仅支持Excel 97-2003）,直接敲回车默认为1.xls");
            String FilePath = Console.ReadLine().Trim();
            if (FilePath == "") { FilePath = "1.xls"; }
            Console.WriteLine("请输入12306账户配置文件名后回车（直接敲回车默认为AccountConfig.txt)");
            String AccountConfigPath = Console.ReadLine();
            if (AccountConfigPath == "") { AccountConfigPath = "AccountConfig.txt"; }
            Console.WriteLine("请选择评价类型：1.全部满意，2.一般评价（输入1或2后回车选择）,直接敲回车默认为全部满意");
            int Option = 0;
            String inputOption = Console.ReadLine().Trim();
            if (inputOption == "2") { Option = 1; }

            List<SurveyBaseInfo> WorkList = sc.LoadWorkList(FilePath,AccountConfigPath);
            List<SurveyBaseInfo> SuccessList = new List<SurveyBaseInfo>();
            List<SurveyBaseInfo> FailedList = new List<SurveyBaseInfo>();
            List<String> ResultList = new List<string>();
            WebControl wc = new WebControl();

            Console.WriteLine("开始提交问卷...");
            int i = 1;
            foreach (SurveyBaseInfo sbi in WorkList)
            {
                Console.WriteLine("正在提交第" + i + "张问卷");
                if (sbi.TravelRecord.TravelDate == "###")
                {
                    sbi.SurveyNumber = sbi.TravelRecord.TravelTrainNumber;
                    FailedList.Add(sbi);
                }
                else
                {
                    String response = wc.FillSurvey(sbi, Option);
                    if (response[0] == '0')
                    {
                        Console.WriteLine("提交第" + i + "张问卷失败，失败原因：" + response.Substring(2));
                        ResultList.Add(response.Substring(2));
                        sbi.SurveyNumber = response.Substring(2);
                        FailedList.Add(sbi);
                    }
                    else
                    {
                        Console.WriteLine("提交第" + i + "张问卷成功");
                        ResultList.Add(response.Substring(2));
                        sbi.SurveyNumber = response.Substring(2);
                        SuccessList.Add(sbi);
                    }
                }
                i++;
            }
            Console.WriteLine("正在输出结果到文件...");
            String OutputFileName=sc.Output(SuccessList, FailedList, ResultList);
            Console.WriteLine("程序运行完成，请到程序目录下查看名为\""+OutputFileName+"\"的输出文件，按任意键退出程序");
            Console.ReadLine();
        }


    }
}