﻿namespace XRPL.DataRippleService;

public class DataRippleBaseResponse
{
    public string result { get; set; }
    public uint count { get; set; }
    public string marker { get; set; }
}