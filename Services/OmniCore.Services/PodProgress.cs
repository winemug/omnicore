namespace OmniCore.Services;

public enum PodProgress
{
    Init0 = 0,
    Init1 = 1,
    Init2 = 2,
    Paired = 3,
    Priming = 4,
    Primed = 5,
    BasalSet = 6,
    Inserting = 7,
    Running = 8,
    RunningLow = 9,
    Faulted = 13,
    ActivationExpired = 14,
    Inactive = 15
}