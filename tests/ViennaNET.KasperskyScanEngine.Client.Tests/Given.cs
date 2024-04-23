namespace ViennaNET.KasperskyScanEngine.Client.Tests;

public static class Given
{
    public static class KsnInfo
    {
        public static readonly string ConnectedJson = @"{
                ""ksnStatus"": ""Connected"",
                ""whiteApplications"": 6882322203,
                ""malwareApplications"": 2125763035,
                ""blockedThreats"": 14542499,
                ""region"": ""global"",
                ""responseTimestamp"": ""2024 02 10 12:15:02""
            
        }";

        public static readonly string TurnedOffJson = @"{
                ""ksnStatus"": ""KSN turned off""
        }";
    }

    public static class UpdateStatus
    {
        public static readonly string NotStartedJson = @"{
                ""status"": ""not started"",
                ""last_update_result"": ""success"",
                ""last_update_time"": ""21:03:53 30.01.2019""
        }";

        public static readonly string InProgressJson = @"{
                ""status"": ""in progress"",
                ""progress"": ""100%"",
                ""action_needed"": ""Product restart needed"",
                ""action_apply_period"": 2
        }";

        public static readonly string StartedJson = @"{
                ""status"": ""update started""
        }";
    }

    public static class ErrorResponse
    {
        public static readonly string BadRequest = @"{
                ""error"": ""Тестовое сообщение об ошибке""
        }";

        public static readonly string EmptyError = @"{
        }";

        public static readonly string StatisticsCleared = @"{
                ""error"": ""CLEARED""
        }";
    }

    public static class StatisticsResponse
    {
        public static readonly string OkJson = @"{
                ""statistics"": {
                    ""total_requests"": 3,
                    ""infected_requests"": 3,
                    ""protected_requests"": 3,
                    ""error_requests"": 0,
                    ""engine_errors"": 0,
                    ""processed_data"": 204,
                    ""infected_data"": 204,
                    ""processed_urls"": 1,
                    ""infected_urls"": 1
                }
        }";
    }

    public static class LicenseInfo
    {
        public static readonly string OkOfflineModeJson = @"{
                ""licenseName"": ""test.key"",
                ""licenseExpirationDate"": ""05.12.2020""
        }";

        public static readonly string OkOnlineModeJson = @"{
                ""activationCode"": ""TEST-*****-*****-12345"",
                ""licenseExpirationDate"": ""05.12.2020"",
                ""ticketExpired"": ""The license ticket has expired.""
        }";
    }

    public static class VersionResponse
    {
        public static readonly string OkJson = @"{
                ""KAVSDKVersion"": ""8.8.2.58""
        }";
    }

    public static class BasesDateResponse
    {
        public static readonly string OkJson = @"{
                ""databaseVersion"": ""30.01.2019 18:38 GMT""
        }";
    }

    public static class CheckUrlResponse
    {
        public static readonly string OkCleanJson = @"{
                ""url"": ""http:\/\/bug.qainfo.ru\/TesT\/Wmuf_w"",
                ""scanResult"": ""CLEAN""
        }";
        public static readonly string OkDetectJson = @"{
                ""url"": ""http:\/\/bug.qainfo.ru\/TesT\/Wmuf_w"",
                ""scanResult"": ""DETECT"",
                ""detectionName"": ""PHISHING_URL""
        }";
    }

    public static class ScanMemoryResponse
    {
        public static readonly string OkWithRootObjDetectJson = @"{
                ""object"": ""memory"",
                ""scanResult"": ""DETECT"",
                ""name"": ""test.txt"",
                ""detectionName"": ""EICAR-Test-File""
        }";

        public static readonly string OkWithRootObjCleanJson = @"{
                ""object"": ""memory"",
                ""scanResult"": ""CLEAN"",
                ""name"": ""test.txt""
        }";

        public static readonly string OkWithSubScanResultsJson = @"{
                ""object"": ""memory"",
                ""scanResult"": ""DETECT"",
                ""name"": ""test.zip"",
                ""containsOfficeMacro"": ""false"",
                ""detectionName"": ""EICAR-Test-File"",
                ""subObjectsScanResults"": [
                    {
                        ""object"": ""test.docx"",
                        ""scanResult"": ""DETECT"",
                        ""containsOfficeMacro"": ""true"",
                         ""detectionName"": ""EICAR-Test-File""
                    }
                ]
        }";
    }

    public static class ScanFileResponse
    {
        public static readonly string OkWithRootObjDetectJson = @"{
                ""object"": ""test.txt"",
                ""scanResult"": ""DETECT"",
                ""detectionName"": ""EICAR-Test-File""
        }";

         public static readonly string OkWithRootObjCleanJson = @"{
                ""object"": ""test.txt"",
                ""scanResult"": ""CLEAN""
        }";
    }
}
