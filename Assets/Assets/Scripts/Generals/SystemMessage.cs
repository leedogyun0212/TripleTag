using UnityEngine;

public static class SystemMessage
{
    public static string FileNameNotFound(string FileName)
        => $"<Error Code 0x00000cc6> FileName \"{FileName}\" Has Not Founded";

    public static string ObjectNameNotFound(string FileName)
        => $"<Error Code 0x00000cc7> ObjectName \"{FileName}\" Has Not Founded";
}
