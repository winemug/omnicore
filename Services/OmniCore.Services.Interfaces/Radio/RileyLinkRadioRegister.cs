namespace OmniCore.Services.Interfaces.Radio;

public enum RileyLinkRadioRegister
{
    SYNC1 = 0,
    SYNC0 = 1,
    PKTLEN = 2,
    PKTCTRL1 = 3,
    PKTCTRL0 = 4,
    ADDR = 5,
    CHANNR = 6,
    FSCTRL1 = 7,
    FSCTRL0 = 8,
    FREQ2 = 9,
    FREQ1 = 10,
    FREQ0 = 11,
    MDMCFG4 = 12,
    MDMCFG3 = 13,
    MDMCFG2 = 14,
    MDMCFG1 = 15,
    MDMCFG0 = 16,
    DEVIATN = 17,
    MCSM2 = 18,
    MCSM1 = 19,
    MCSM0 = 20,
    FOCCFG = 21,
    BSCFG = 22,
    AGCCTRL2 = 23,
    AGCCTRL1 = 24,
    AGCCTRL0 = 25,
    FREND1 = 26,
    FREND0 = 27,
    FSCAL3 = 28,
    FSCAL2 = 29,
    FSCAL1 = 30,
    FSCAL0 = 31,

    // TEST2 = None,
    TEST1 = 36,
    TEST0 = 37,

    // PA_TABLE7 = None,
    // PA_TABLE6 = None,
    // PA_TABLE5 = None,
    // PA_TABLE4 = None,
    // PA_TABLE3 = None,
    // PA_TABLE2 = None,
    // PA_TABLE1 = None,
    PA_TABLE0 = 46
}