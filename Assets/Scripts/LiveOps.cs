using System;
using System.Globalization;
using UnityEngine;

public static class LiveOps
{
    public const string FindId = "find";
    public const string MarkId = "mark";

    const string DateKey    = "liveops_last_claim";
    const string DateFormat = "yyyy-MM-dd";

    public static bool IsNewDay(string savedDate, DateTime today)
    {
        
        if (!DateTime.TryParseExact(savedDate, DateFormat, CultureInfo.InvariantCulture,
                                    DateTimeStyles.None, out DateTime last))
            return true;

        return today > last;
    }

    public static bool CanClaim()
        => IsNewDay(PlayerPrefs.GetString(DateKey, ""), DateTime.Today);

    public static bool TryClaimDaily()
    {
        if (!CanClaim()) return false;

        PlayerPrefs.SetString(DateKey, DateTime.Today.ToString(DateFormat, CultureInfo.InvariantCulture));
        PlayerPrefs.Save();
        return true;
    }

    public static int GetCharge(string id, int max) => PlayerPrefs.GetInt("charge_" + id, max);

    public static void SetCharge(string id, int value)
    {
        PlayerPrefs.SetInt("charge_" + id, value);
        PlayerPrefs.Save();
    }

#if UNITY_EDITOR
    public static void ResetDaily() => PlayerPrefs.DeleteKey(DateKey);
#endif
}
