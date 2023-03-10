using System;

namespace OmniCore.Services.Interfaces.Radio;

public static class RileyLinkGatt
{
    public static Guid ServiceGenericAccess =
        new(0x00001800, 0x0000, 0x1000, 0x80, 0x00, 0x00, 0x80, 0x5f, 0x9b, 0x34, 0xfb); // Generic Access

    public static Guid ServiceGenericAccessCharDeviceName =
        new(0x00002a00, 0x0000, 0x1000, 0x80, 0x00, 0x00, 0x80, 0x5f, 0x9b, 0x34, 0xfb); // Device Name

    public static Guid ServiceGenericAccessCharAppearance =
        new(0x00002a01, 0x0000, 0x1000, 0x80, 0x00, 0x00, 0x80, 0x5f, 0x9b, 0x34, 0xfb); // Appearance

    public static Guid ServiceGenericAccessCharPeripheralPreferences =
        new(0x00002a04, 0x0000, 0x1000, 0x80, 0x00, 0x00, 0x80, 0x5f, 0x9b, 0x34,
            0xfb); // Peripheral Preferred Connection Parameters

    public static Guid ServiceGenericAccessCharUnkown1 =
        new(0x00002aa6, 0x0000, 0x1000, 0x80, 0x00, 0x00, 0x80, 0x5f, 0x9b, 0x34, 0xfb); // Unknown characteristic

    public static Guid
        ServiceBattery =
            new(0x0000180f, 0x0000, 0x1000, 0x80, 0x00, 0x00, 0x80, 0x5f, 0x9b, 0x34, 0xfb); // Battery Service

    public static Guid ServiceBatteryCharBatteryLevel =
        new(0x00002a19, 0x0000, 0x1000, 0x80, 0x00, 0x00, 0x80, 0x5f, 0x9b, 0x34, 0xfb); // Battery Level

    public static Guid
        ServiceMain =
            new(0x0235733b, 0x99c5, 0x4197, 0xb8, 0x56, 0x69, 0x21, 0x9c, 0x2a, 0x38, 0x45); // Unknown Service

    public static Guid ServiceMainCharData =
        new(0xc842e849, 0x5028, 0x42e2, 0x86, 0x7c, 0x01, 0x6a, 0xda, 0xda, 0x91, 0x55); // Unknown characteristic

    public static Guid ServiceMainCharResponseCount =
        new(0x6e6c7910, 0xb89e, 0x43a5, 0xa0, 0xfe, 0x50, 0xc5, 0xe2, 0xb8, 0x1f, 0x4a); // Unknown characteristic

    public static Guid ServiceMainCharTimerTick =
        new(0x6e6c7910, 0xb89e, 0x43a5, 0x78, 0xaf, 0x50, 0xc5, 0xe2, 0xb8, 0x6f, 0x7e); // Unknown characteristic

    public static Guid ServiceMainCharCustomName =
        new(0xd93b2af0, 0x1e28, 0x11e4, 0x8c, 0x21, 0x08, 0x00, 0x20, 0x0c, 0x9a, 0x66); // Unknown characteristic

    public static Guid ServiceMainCharVersion =
        new(0x30d99dc9, 0x7c91, 0x4295, 0xa0, 0x51, 0x0a, 0x10, 0x4d, 0x23, 0x8c, 0xf2); // Unknown characteristic

    public static Guid ServiceMainCharLedMode =
        new(0xc6d84241, 0xf1a7, 0x4f9c, 0xa2, 0x5f, 0xfc, 0xe1, 0x67, 0x32, 0xf1, 0x4e); // Unknown characteristic

    public static Guid OrangeServiceUnknown1 =
        new(0x00001801, 0x0000, 0x1000, 0x80, 0x00, 0x00, 0x80, 0x5f, 0x9b, 0x34, 0xfb); // Generic Attribute

    public static Guid OrangeServiceUnknown2 =
        new(0x0000fe59, 0x0000, 0x1000, 0x80, 0x00, 0x00, 0x80, 0x5f, 0x9b, 0x34, 0xfb); // Unknown Service

    public static Guid OrangeServiceUnknown2CharUnknown1 =
        new(0x8ec90003, 0xf315, 0x4f60, 0x9f, 0xb8, 0x83, 0x88, 0x30, 0xda, 0xea, 0x50); // Unknown characteristic

    public static Guid OrangeServiceUnknown3 =
        new(0x6e400001, 0xb5a3, 0xf393, 0xe0, 0xa9, 0xe5, 0x0e, 0x24, 0xdc, 0xca, 0x9e); // Unknown Service

    public static Guid OrangeServiceUnknown3CharUnknown1 =
        new(0x6e400002, 0xb5a3, 0xf393, 0xe0, 0xa9, 0xe5, 0x0e, 0x24, 0xdc, 0xca, 0x9e); // Unknown characteristic

    public static Guid OrangeServiceUnknown3CharUnknown2 =
        new(0x6e400003, 0xb5a3, 0xf393, 0xe0, 0xa9, 0xe5, 0x0e, 0x24, 0xdc, 0xca, 0x9e); // Unknown characteristic
}