using LiteDB;

namespace AleRoe.LiteDB.Extensions.DependencyInjection
{
    /// <summary>
    /// Represents a factory object for creating a <see cref="LiteDatabase"/> instance.
    /// </summary>
    public interface ILiteDatabaseFactory
    {
        /// <summary>
        /// Creates a new <see cref="LiteDatabase"/>instance.
        /// </summary>
        /// <returns>The <see cref="LiteDatabase"/> instance.</returns>
        LiteDatabase Create();
    }
}