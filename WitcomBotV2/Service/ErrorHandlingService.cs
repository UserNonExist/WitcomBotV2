using Discord;

namespace WitcomBotV2.Service;

public class ErrorHandlingService
{
    private static readonly Dictionary<ErrorCodes, string> ErrorDescriptions = new()
    {
        {
            ErrorCodes.PermissionDenied, "คุณไม่มีสิทธิ์ในการใช้คำสั่งนี้\n{0}"
        },
        {
            ErrorCodes.UnableToParseDuration,
            "{0} นั้นไม่ถูกต้อง.\nคุณต้องใส่เวลาเป็นจำนวนเต็มพร้อมด้วยตัวกำกับ เช่น 's', 'm', 'h', 'd', 'w', 'M' or 'y'"
        },
        {
            ErrorCodes.SpecifiedUserNotFound, "ผู้ใช้ ({0}) ไม่อยู่ในเซิร์ฟเวอร์นี้"
        },
        {
            ErrorCodes.InternalCommandError, "กูพังอะไรไปสักอย่าง ปิง User_NotExist\n{0}"
        },
        {
            ErrorCodes.InvalidChannelId, "หาห้องไอดี {0} ไม่เจอ"
        },
        {
            ErrorCodes.UnableToParseDate, "วันที่ {0} นั้นไม่ถูกต้อง"
        },
        {
            ErrorCodes.InvalidNumberOfArguments, "คำสั่งนี้ต้องใส่ {0} ค่า"
        },
        {
            ErrorCodes.DatabaseNotFound, "กูพังอะไรไปสักอย่าง ปิง User_NotExist"
        },
        {
            ErrorCodes.FailedToParseTitle,
            "No valid title was found. Titles for embeds must be encased in \"double-quotes\""
        },
        {
            ErrorCodes.FailedToParseColor, "{0} ไม่ใช่ HTML HEX color code"
        },
        {
            ErrorCodes.NoRecordFound, "ไม่มีข้อมูลของ {0} ในระบบ"
        },
        {
            ErrorCodes.InvalidMessageId, "หาข้อความไอดี {0} ไม่เจอ"
        },
        {
            ErrorCodes.Unspecified, "แกน่าจะใช้อะไรสักอย่างผิดไป แต่ก็ปิง User_NotExist มาเหอะ\n{0}"
        },
        {
            ErrorCodes.TriggerLengthExceedsLimit, "Ping triggers มีตัวอักษรได้สูงสุด {0} ตัว"
        },
        {
            ErrorCodes.AlreadyExists, "An entry with that identifier already exists."
        },
        {
            ErrorCodes.CouldNotGetIp, "ไม่สามารถหา IP ของเซิร์ฟเวอร์ได้"
        },
        {
            ErrorCodes.UnableToParseId, "Unable to parse ID from {0}"
        },
        {
            ErrorCodes.NoMusicClass, "ไม่ได้เชื่อมต่อกับเซิร์ฟเวอร์เพลง"
        }
    };

    private static string GetErrorMessage(ErrorCodes e, string extra = "") => $"Code {(int)e}: {e.ToString().SplitCamelCase()} {extra}".TrimEnd(' ');

    private static string GetErrorDescription(ErrorCodes e) => ErrorDescriptions.ContainsKey(e)
        ? ErrorDescriptions[e]
        : ErrorDescriptions[ErrorCodes.Unspecified];

    public static async Task<Embed> GetErrorEmbed(ErrorCodes errorCode, string extra = "")
    {
        Log.Info(nameof(GetErrorEmbed), $"Sending error code: {errorCode}");
        return await EmbedBuilderService.CreateBasicEmbed(GetErrorMessage(errorCode),
            !string.IsNullOrEmpty(extra)
                ? string.Format(GetErrorDescription(errorCode), $"\"{extra}\"")
                : GetErrorDescription(errorCode).Replace("{0}", string.Empty), Color.Red);
    }
}