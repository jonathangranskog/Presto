using UnityEngine;

public static class ExtraUtils {

	public static string ClampName(string name, int n)
    {
        if (name.Length < n)
        {
            return name;
        } else
        {
            string cutoff = name.Substring(0, n - 3);
            cutoff += "...";
            return cutoff;
        }
    }

    public static string ClampFrontName(string name, int n)
    {
        if (name.Length < n)
        {
            return name;
        } else
        {
            string cutoff = name.Substring(name.Length - n + 3, n - 3);
            string result = "..." + cutoff;
            return result;
        }
    }

}
