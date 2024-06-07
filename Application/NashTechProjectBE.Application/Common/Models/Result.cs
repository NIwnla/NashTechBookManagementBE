using System.Net;

namespace NashTechProjectBE.Application.Common.Models;

public class Result
{
    public HttpStatusCode StatusCode{ get; set; }
    public string Message{ get; set; } = null!;
}
