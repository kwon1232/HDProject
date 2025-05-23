using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using UnityEngine;

public static class GoogleSheetLoader
{
    public static List<List<string>> LoadCSVFromUrl(string csvUrl)
    {
        var result = new List<List<string>>();
        try
        {
            using (var client = new WebClient())
            {
                string data = client.DownloadString(csvUrl);
                using (var reader = new StringReader(data))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        result.Add(new List<string>(line.Split(',')));
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("CSV 로딩 실패: " + e.Message);
        }
        return result;
    }

    public static List<string> GetSheetNamesFromHtml(string spreadsheetId)
    {
        List<string> result = new List<string>();
        string url = $"https://docs.google.com/spreadsheets/d/{spreadsheetId}/edit";
        try
        {
            using (WebClient client = new WebClient())
            {
                string html = client.DownloadString(url);
                var matches = Regex.Matches(html, "\\\"sheetId\\\":\\d+,\\\"title\\\":\\\"([^\\\"]+)\\\"");
                foreach (Match m in matches)
                {
                    result.Add(m.Groups[1].Value);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("시트 탭 목록 추출 실패: " + e.Message);
        }
        return result;
    }
}