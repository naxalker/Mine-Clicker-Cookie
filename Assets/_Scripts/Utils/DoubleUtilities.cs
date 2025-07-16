using System;
using System.Globalization;
using UnityEngine;

public enum IdleAbbreviation
{
    K,    // тысяча
    M,    // миллион
    B, // миллиард 
    T,    // триллион
    Qa,  // квадриллион
    Qi,  // квинтиллион
    Sx,  // секстиллион
    Sp,  // септиллион
    Oc,  // октиллион
    No,  // нониллион
    Dc,  // дециллион
    Ud,  // ундециллион
    Dd,  // дуодециллион
    Td   // тредециллион
}

public static class DoubleUtilities
{
    public static string ToIdleNotation(double value)
    {
        if (value < 1000)
            return value.ToString("0.##");

        double tmpValue = value;
        int abbreviationIndex = -1;

        while (tmpValue >= 1000)
        {
            tmpValue /= 1000;
            abbreviationIndex++;
        }

        if (abbreviationIndex >= Enum.GetValues(typeof(IdleAbbreviation)).Length)
            return ToScientificNotation(value);

        string idleAbbreviation
            = Enum.GetValues(typeof(IdleAbbreviation)).GetValue(abbreviationIndex).ToString();

        return tmpValue.ToString("0.##") + idleAbbreviation;
    }

    public static string ToScientificNotation(double value)
    {
        int exponent = 0;
        double tmpValue = value;

        if (tmpValue < 10)
            return value.ToString("0.##");

        while (tmpValue > 10)
        {
            tmpValue /= 10;
            exponent++;
        }

        return tmpValue.ToString("0.##") + "e" + exponent;
    }

    public static string ToCustomScientificNotation(double value)
    {
        if (value < Mathf.Pow(10, 3))
            return ToSeparatedThousands(value);
        else
            return ToIdleNotation(value);
    }

    public static string ToSeparatedThousands(double value)
    {
        NumberFormatInfo nfi = new NumberFormatInfo();

        nfi.NumberGroupSeparator = ".";
        nfi.NumberDecimalSeparator = ",";

        return value.ToString("#,0.##", nfi);
    }

    public static double RoundToLeadingDigit(double num)
    {
        if (num == 0) return 0;

        int orderOfMagnitude = (int)Math.Floor(Math.Log10(Math.Abs(num)));
        double leadingDigit = num / Math.Pow(10, orderOfMagnitude);

        return Math.Sign(num) * Math.Floor(leadingDigit) * Math.Pow(10, orderOfMagnitude);
    }
}