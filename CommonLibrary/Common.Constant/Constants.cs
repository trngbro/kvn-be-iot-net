namespace Common.Constant;

public static class Constants
{
    public static string Schema = "dbo";
    public static int TimeExtendForgotPassword = 120;
    public static int DefaultTimeExpired = 300;
    public static int DefaultDayExpired = 1;
    public static TimeSpan timeUncache = TimeSpan.FromMinutes(10);
    public static string FormatDatetimeString = "dd-MM-yyyy HH:mm:ss";
}

public class CommonMessage
{
    public static string Message_Fail = "Something went wrong. Please try again or contact IT to support.";
    public static string Message_DataNotFound = "{0} is not found";
    public static string Message_Denied = "Action {0} denied";
    public static string Message_Required = "{0} is required";
    public static string Message_NotExistsData = "Not exist data";
    public static string Message_InvalidRequest = "{0} is invalid";
    public static string Message_Exists = "{0} has exists";

    public static string Message_Expired = "{0} is expired";
    public static string Message_NotFound = "Data not found";
    public static string Message_Update = "{0} was updated.";
    public static string Message_NotHavePermission_ContactOwner = "You doesn't have permission. Please contact owner or Admin!!";
    public static string Message_CannotUpdateData = "Can't update this data";
    public static string Message_GreaterThanZero = "{0} must be greater than zero";
    public static string Message_Overdue = "The deadline for action is overdue!";
}

public class KeyCache
{
    public const string SysActivity = "SysActivity";
    public const string SysRole = "SysRole";
    public const string SysUserRole = "SysUserRole";
    public const string User = "SysAccount";
    public const string CrFlightInfo = "CrFlightInfo";
    public const string CrAircraftDefect = "CrAircraftDefect";
    public const string FinalReportCategory = "FinalReportCategory";
    public const string EmployeeSimpleInfo = "EmployeeSimpleInfo";
    public const string SttNational = "SttNational";
    public const string PieEmployee = "PieEmployee";
    public const string Support = "Support";
    public const string Department = "Department";
    public const string FormCategory = "FormCategory";
    public const string News = "News";
    public const string PssCkInSum = "PssCheckInSummary";
    public const string BlackListSms = "BlackListSms";
    public const string ListUser = "TmsUsers";
}

public class CacheTime
{
    public static TimeSpan CommmonUncache = TimeSpan.FromMinutes(10);
    public static TimeSpan BlackList = new(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")).Year, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")).Month, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")).Day, 23, 59, 59);
    public static TimeSpan QrCode = TimeSpan.FromMinutes(5);
    public static TimeSpan OTP = TimeSpan.FromMinutes(5);
}

public class MyFormatConst
{
    public const string CM_Format_Money_Style = "CM_Format_Money_Style";
    public const string CM_Format_Money_Unit_VND_ShortText = "CM_Format_Money_Unit_VND_ShortText";
    public const string CM_Format_LongDate24_None_Second = "CM_Format_LongDate24_None_Second";
    public const string CM_Format_Money_Unit_VND = "CM_Format_Money_Unit_VND";
    public const string CM_Format_Money_Unit_USD = "CM_Format_Money_Unit_USD";
    public const string CM_Format_ShortDate = "CM_Format_ShortDate";
    public const string CM_Format_Money_Style_With_Precision_Two = "CM_Format_Money_Style_With_Precision_Two";
}

public class MyColor
{
    public const string APP_BLUE = "#1976D2";
    public const string APP_GREEN = "#388E3C";
    public const string APP_ORANGE = "#F57C00";
    public const string APP_LIGHT_GRAY = "#CDCDCD";
    public const string APP_BLACK = "#000000";
    public const string APP_RED = "#F44336";
    public const string COLOR_GRAY = "#BDBDBD";
    public const string COLOR_BLACK = "#000000";
    public const string COLOR_WHITE = "#FFFFFF";
    public const string COLOR_GREEN = "#43A047";
    public const string COLOR_RED = "#F44336";
    public const string COLOR_BLUE = "#1976D2";
    public const string COLOR_YELLOW = "#FBC02D";
    public const string COLOR_ORANGE = "#F57C00";
    public const string COLOR_LIGHTGRAY = "#D8D8D8";
}

public class IconStateColor
{
    public const string Missing = "#C0C0C0";//Default-Silver
    public const string Warning = "#FFFF00";//Yellow
    public const string Good = "#00FFFF";//Blue
    public const string Alert = "#FF0713";//"#D10000";//"#FF5733";//Red
    public const string Light = "#E5EE7E";//Yellow + Green
    public const string White = "#FFFFFF";//White 

}

public class IconTextColor
{
    public const string Black = "#000000";//Black
    public const string Missing = "#0031A3";//Blue
    public const string Warning = "#000000";//Black
    public const string Good = "#000000";//"#FFFFFF";//White  
    public const string Alert = "#000000";//"#FFFFFF";//White 
    public const string Red_Crimson = "#DC143C";
    public const string Red_Tomato = "#FF6347";
    public const string Red_DarkOrange = "#FF8C00";
    public const string Green_Dark = "#006400";
    public const string Blue = "#0000FF";
    public const string GOLD_VIP = "#DAA520";
    public const string GOLD_VIPA = "#ff7500";
}

public class IconValueCrewDuty
{
    public const string Missing = "!";
    public const string Warning = "!";
    public const string Good = "✅";//✔"☑";
    public const string Alert = "!";

    public const string None = "";
}

public class MyModule
{
    public const string SYSTEM = "SYS";
}

public class BHTMBHandOverStatusMessage
{
    public const string Pending = "Chờ xác nhận";
    public const string Confirmed = "Thành công";
    public const string Transfer = "Đã bàn giao (chờ xác nhận)";
}

public class CrewTaskTextColor
{
    public const string Default = "#6699CC";//"#02BBBE";//Blue
    public const string CanWrite = "#DBA510";//Yellow
    public const string ReadOnly = "#A0A0A0";//Grey
}

public class MessageSource
{
    public const long NEWSID_COVID19 = 149632; // id tst
    public const long NEWSID_COVID19_NVTV = 152226; // id tin ngung viec TV
    const string SOURCE_ALL = "ALL";
    public const string SOURCE_COVI = "COVI";
    public const string SOURCE_CAKE = "CAKE";
    public const string SOURCE_SET = "SET";
    public const string PUBLIC_NEWS = "NEWS";
}

public class RequestStatus
{
    public const string submitted = "Submitted";
    public const string approved = "Approved";
    public const string preApproved = "PreApproved";
    public const string rejected = "Rejected";
    public const string inProgress = "c";
    public const string acceptance = "Acceptance";
    public const string completed = "Completed";
}

public class ApproverType
{
    public const int Approver = 1;
    public const int ITLead = 2;
    public const int ITHead = 3;
    public const int Assignee = 4;
    public const int Rejected = 5;
}

public class MailTemplateTypeCode
{
    public const string AccountMS_Approved = "DARA";
    public const string AccountMS_Finnished = "DARF";
    public const string AccountMS_Rejected = "DARR";
    public const string Amabeus_Approved = "ITCRA";
    public const string Amabeus_Finnished = "ITCRF";
    public const string Amabeus_Rejected = "ITCRR";
}