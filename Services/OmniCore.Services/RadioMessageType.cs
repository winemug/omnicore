namespace OmniCore.Services
{
    public enum RadioMessageType
    {
        RequestSetupPod = 0x03,
        RequestAssignAddress = 0x07,
        RequestSetDeliveryFlags = 0x08,
        RequestStatus = 0x0e,
        RequestAcknowledgeAlerts = 0x11,
        RequestBasalSchedule = 0x13,
        RequestTempBasalSchedule = 0x16,
        RequestBolusSchedule = 0x17,
        RequestConfigureAlerts = 0x19,
        RequestInsulinSchedule = 0x1a,
        RequestDeactivatePod = 0x1c,
        RequestCancelDelivery = 0x1f,
        RequestBeepConfig = 0x1e,

        ResponseVersionInfo = 0x01,
        ResponseDetailInfo = 0x02,
        ResponseResyncRequest = 0x06,
        ResponseStatus = 0x1d,
    }
}