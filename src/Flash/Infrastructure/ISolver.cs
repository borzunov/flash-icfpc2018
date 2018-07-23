using Flash.Infrastructure.Models;

namespace Flash.Infrastructure
{
    public interface ISolver
    {
        Trace Solve(Matrix srcMatrix, Matrix tgtMatrix);
    }
}
