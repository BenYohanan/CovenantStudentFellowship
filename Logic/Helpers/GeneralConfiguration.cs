using Logic.IHelpers;

namespace Logic.Helpers
{
    public class GeneralConfiguration : IGeneralConfiguration
    {
        public string AdminEmail { get; set; }
        public string DeveloperEmail { get; set; }
        public string HebrewCalApiUrl { get; set; }
        public string Version { get; set; }
        public string JsonOutput { get; set; }
        public string Major { get; set; }
        public string Minor { get; set; }
        public string MordenHoliday { get; set; }
        public string RoshChodesh { get; set; }
        public string Year { get; set; }
        public string Month { get; set; }
        public string SpecialShabbath { get; set; }
        public string MinorFast { get; set; }
        public string CandleLightingTimes { get; set; }
        public string Geo { get; set; }
        public string Geonameid { get; set; }
        public string Havdalah { get; set; }
        public string ParashatHaShavuah { get; set; }
        public string CronReminderExpressionTime { get; set; }
        public string BuckSMSBaseURL { get; set; }
        public string BuckSMSEmail { get; set; }
        public string BuckSMSPassword { get; set; }
        public string BuckSMSforcednd { get; set; }
        public string PayStakApiKey { get; set; }
    }
}
