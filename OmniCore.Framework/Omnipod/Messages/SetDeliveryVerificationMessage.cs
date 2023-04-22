﻿using OmniCore.Common.Pod;
using OmniCore.Services;
using OmniCore.Services.Interfaces.Entities;
using OmniCore.Services.Interfaces.Pod;

namespace OmniCore.Framework.Omnipod.Messages;

public class SetDeliveryVerificationMessage : IMessageData
{
    public static Predicate<IMessageParts> CanParse => (parts) => parts.MainPart.Type == PodMessagePartType.RequestSetDeliveryFlags;

    public byte VerificationFlag0 { get; set; }
    public byte VerificationFlag1 { get; set; }

    public IMessageData FromParts(IMessageParts parts)
    {
        VerificationFlag0 = parts.MainPart.Data[0];
        VerificationFlag1 = parts.MainPart.Data[1];
        return this;
    }

    public IMessageParts ToParts()
    {
        return new MessageParts(
            new MessagePart
            {
                Type = PodMessagePartType.RequestSetDeliveryFlags,
                RequiresNonce = true,
                Data = new Bytes(new byte[]{ VerificationFlag0, VerificationFlag1  }),
            });
    }
}