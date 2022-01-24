using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class StringUtils
{
    public static string FormatString(string myString)
    {
        myString = Regex.Replace(myString.Trim(), @"\s+", " ");
        return myString;
    }

    public static string FormatMoney(double value)
    {
        return value.ToString("N0", System.Globalization.CultureInfo.GetCultureInfo("de"));
    }

    public static string RemoveCommas(string value)
    {
        return string.IsNullOrEmpty(value) ? "" : value.Replace(".", string.Empty);
    }

    public static string FormatMoneyK(double value)
    {
        if (value >= 1000000000)
        {
            value /= 1000000000;
            return Math.Round(value, 2) + "B";
        }

        if (value >= 1000000)
        {
            value /= 1000000;
            return Math.Round(value, 2) + "M";
        }

        if (!(value >= 1000)) return value.ToString();
        value /= 1000;
        return Math.Round(value, 2) + "K";
    }


    public static string GetName(string str, int maxSize)
    {
        if (str == null) return "";
        if (str.Length <= maxSize)
        {
            return str;
        }

        var temp = str.Substring(0, 9);
        temp += "...";
        return temp;
    }
}