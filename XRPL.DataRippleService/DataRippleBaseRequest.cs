using XRPL.DataRippleService.Enums;

namespace XRPL.DataRippleService
{
    public class DataRippleBaseRequest
    {
        /// <summary>
        /// Filter results to this time and later.
        /// Standardized UTC format yyyy-MM-ddTHH:mm:ssZ
        /// </summary>
        public string Start => StartTime is { } time ? $"{time:yyyy-MM-ddTHH:mm:ssZ}" : null;
        /// <summary>
        /// Filter results to this time and later.
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// Filter results to this time and earlier.
        /// Standardized UTC format yyyy-MM-ddTHH:mm:ssZ
        /// </summary>
        public string End => EndTime is { } time ? $"{time:yyyy-MM-ddTHH:mm:ssZ}" : null;
        /// <summary>
        /// Filter results to this time and earlier.
        /// </summary>
        public DateTime? EndTime { get; set; }
        /// <summary>
        /// If true, return results in reverse chronological order.
        /// The default is false.
        /// POSSIBLE ERROR when True
        /// </summary>
        public bool? Descending { get; set; }

        /// <summary>
        /// Maximum results per page. The default is 200. Cannot be more than 1000.
        /// </summary>
        public uint? Limit { get; set; } = 1000;
        /// <summary>
        /// Pagination key from previously returned response.
        /// </summary>
        public string Marker { get; set; }

        /// <summary>
        /// Format of returned results: csv or json. The default is json.
        /// </summary>
        public DataRippleResponseFormat? Format { get; set; } = DataRippleResponseFormat.json;

    }
}
