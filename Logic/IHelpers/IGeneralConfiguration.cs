using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.IHelpers
{
    public interface IGeneralConfiguration
    {
        string AdminEmail { get; set; }
        string DeveloperEmail { get; set; }
        string HebrewCalApiUrl { get; set; }
        string Version { get; set; }
        string JsonOutput { get; set; }
        string Major { get; set; }
        string Minor { get; set; }
        string MordenHoliday { get; set; }
        string RoshChodesh { get; set; }
        string Year { get; set; }
        string Month { get; set; }
        string SpecialShabbath { get; set; }
        string MinorFast { get; set; }
        string CandleLightingTimes { get; set; }
        string Geo { get; set; }
        string Geonameid { get; set; }
        string Havdalah { get; set; }
        string ParashatHaShavuah { get; set; }
        string CronReminderExpressionTime { get; set; }
        string BuckSMSBaseURL { get; set; }
        string BuckSMSEmail { get; set; }
        string BuckSMSPassword { get; set; }
        string BuckSMSforcednd { get; set; }
        string PayStakApiKey { get; set; }
    }
}
