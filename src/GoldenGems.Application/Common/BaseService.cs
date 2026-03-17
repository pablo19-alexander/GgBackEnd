using Microsoft.Extensions.Logging;

namespace GoldenGems.Application.Common;

/// <summary>
/// Clase base abstracta que proporciona funcionalidad común a todos los servicios.
/// </summary>
public abstract class BaseService : IBaseService
{
    protected readonly ILogger _logger;

    protected BaseService(ILogger logger)
    {
        _logger = logger;
    }
}
