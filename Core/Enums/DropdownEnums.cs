using System.ComponentModel;

namespace Core.Enums
{
    public enum DropdownEnums
    {
        [Description("For returning Genders")]
        Gender = 1,

        [Description("For returning Religions")]
        Religion = 2,
        [Description("For returning Home Page Image")]
        HomePageImage = 4,

        [Description("For returning BlogCategory")]
        BlogCategory = 5,

        [Description("For returning Currency")]
        Currency = 6,

        [Description("For returning Donation Category")]
        DonationCategory = 7,
    }

    public enum BlogStatus
    {
        [Description("To return Approved Blog")]
        Approved = 1,
        [Description("To return Pending Blog")]
        Pending = 2,
        [Description("To return Decline Blog")]
        Decline = 3,
    }

    public enum DocumentList
    {
        [Description("To return Approved Blog")]
        Manual = 1,
        [Description("To return Pending Blog")]
        Constitution = 2,
    }

    public enum VerificationStatus
    {
        [Description("Complete Registration")]
        Completed = 1,
        [Description("Incomplete Registration")]
        InComplete = 2,
    }

    public enum PaymentStatus
    {
        [Description("For Approved Payment")]
        ApprovedAndVerified = 1,
        [Description("For Declined Payment")]
        Declined = 2,
        [Description("For Pending Payment")]
        Pending = 3,
        [Description("For Payments that was paid Successfully")]
        Paid,
    }

    public enum DonationInterval
    {
        [Description("For Weekly Period Package")]
        Weekly = 1,
        [Description("For Monthly Period Package")]
        Monthly,
        [Description("For Yearly Period Package")]
        Yearly,
    }
}
