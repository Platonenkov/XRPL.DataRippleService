namespace XRPL.DataRippleService.Enums;

/// <summary> Transaction change type  </summary>
public enum BalanceChangeResultEnum
{
    none,
    /// <summary> Sold token </summary>
    Sold,
    /// <summary> Bought token </summary>
    Bought,
    /// <summary> Sent token </summary>
    Sent,
    /// <summary> Receive token </summary>
    Receive,
    /// <summary> Burn token </summary>
    Burn,
}