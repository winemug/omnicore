// using System.Text;
// using System.Text.Json;
// using OmniCore.Shared.Entities.Omnipod.Parts;
// using OmniCore.Shared.Enums;
//
// namespace OmniCore.Shared.Entities.Omnipod;
//
// public class PodSession
// {
//     private PodStatusInfo? lastStatusInfo = null;
//     public void AddAction(PodAction podAction)
//     {
//         if (podAction.Result != 0)
//             return;
//         var sent = new PodMessageBuilder().WithData(podAction.SentData).Build();
//         var received = new PodMessageBuilder().WithData(podAction.ReceivedData).Build();
//         
//         System.Console.WriteLine($"Action Idx: {podAction.Index} {podAction.RequestSentEarliest.Value}");
//         System.Console.WriteLine($"SENT {PartString(sent.Parts)}");
//         System.Console.WriteLine($"RCVD {PartString(received.Parts)}");
//
//         var psi = GetStatus(received);
//         if (psi == null)
//             return;
//
//         if (lastStatusInfo == null)
//         {
//             lastStatusInfo = psi;
//             return;
//         }
//     }
//     private string PartString(IMessagePart[] parts)
//     {
//         var sb = new StringBuilder();
//         foreach (var part in parts)
//         {
//             sb.AppendLine(part.GetType().Name);
//             sb.AppendLine(JsonSerializer.Serialize((object)part));
//         }
//         return sb.ToString();
//     }
//
//     private PodStatusInfo? GetStatus(PodMessage receivedMessage)
//     {
//         PodStatusInfo? psi = null;
//         if (receivedMessage.Parts[0] is ResponseStatus rs)
//         {
//             psi = new PodStatusInfo
//             {
//                 Progress = rs.Progress,
//                 ActiveMinutes = rs.ActiveMinutes,
//                 PulsesDelivered = rs.PulsesDelivered,
//                 PulsesPending = rs.PulsesPending,
//                 PulsesRemaining = rs.PulsesRemaining,
//                 BasalActive = rs.BasalActive,
//                 TempBasalActive = rs.TempBasalActive,
//                 ImmediateBolusActive = rs.ImmediateBolusActive,
//                 ExtendedBolusActive = rs.ExtendedBolusActive,
//                 Faulted = rs.OcclusionFault
//             };
//         }
//         else if (receivedMessage.Parts[0] is ResponseInfoExtendedStatus re)
//         {
//             psi = new PodStatusInfo
//             {
//                 Progress = re.Progress,
//                 ActiveMinutes = re.ActiveMinutes,
//                 PulsesDelivered = re.PulsesDelivered,
//                 PulsesPending = re.PulsesPending,
//                 PulsesRemaining = re.PulsesRemaining,
//                 BasalActive = re.BasalActive,
//                 TempBasalActive = re.TempBasalActive,
//                 ImmediateBolusActive = re.ImmediateBolusActive,
//                 ExtendedBolusActive = re.ExtendedBolusActive,
//                 Faulted = (re.FaultEvent != 0)
//             };
//         }
//
//         return psi;
//     }
// }
//
// public record PodStatusInfo
// {
//     public bool ExtendedBolusActive { get; init; }
//     public bool ImmediateBolusActive { get; init; }
//     public bool TempBasalActive { get; init; }
//     public bool BasalActive { get; init; }
//     public int PulsesDelivered { get; init; }
//     public int PulsesPending { get; init; }
//     public int PulsesRemaining { get; init; }
//     public int ActiveMinutes { get; init; }
//     public bool Faulted { get; set; }
//     public PodProgress Progress { get; set; }
// }
//
// public record PodInsulinAction
// {
//     
// }