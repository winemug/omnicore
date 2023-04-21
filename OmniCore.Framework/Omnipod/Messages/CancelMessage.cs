﻿using OmniCore.Common.Pod;
using OmniCore.Services;
using OmniCore.Services.Interfaces.Entities;
using OmniCore.Services.Interfaces.Pod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OmniCore.Framework.Omnipod.Messages
{
    public class CancelMessage : IMessageData
    {
        public BeepType Beep { get; set; }
        public bool CancelExtendedBolus { get; set; }
        public bool CancelBolus { get; set; }
        public bool CancelTempBasal { get; set; }
        public bool CancelBasal { get; set; }

        public static Predicate<IMessageParts> CanParse => (parts) => parts.MainPart.Type == PodMessagePartType.RequestCancelDelivery;

        public IMessageParts ToParts()
        {
            var b0 = (int)Beep << 4;
            b0 |= CancelExtendedBolus ? 0x08 : 0x00;
            b0 |= CancelBolus ? 0x04 : 0x00;
            b0 |= CancelTempBasal ? 0x02 : 0x00;
            b0 |= CancelBasal ? 0x01 : 0x00;

            var data = new Bytes((byte)b0);
            return new MessageParts(new MessagePart
            {
                Data = data,
                Type = PodMessagePartType.RequestCancelDelivery,
                RequiresNonce = true
            });
        }

        public IMessageData FromParts(IMessageParts parts)
        {
            var data = parts.MainPart.Data;
            var b0 = data.Byte(0);
            CancelExtendedBolus = (b0 & 0x08) > 0;
            CancelBolus = (b0 & 0x04) > 0;
            CancelTempBasal = (b0 & 0x02) > 0;
            CancelBasal = (b0 & 0x01) > 0;
            return this;
        }
    }
}