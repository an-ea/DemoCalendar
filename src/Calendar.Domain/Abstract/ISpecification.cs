using System.Linq.Expressions;

namespace Calendar.Domain.Abstract;

/// <summary>
/// Defines a specification that is used to filter <typeparamref name="T"/> elements.
/// </summary>
/// <typeparam name="T">Type of element being filtered.</typeparam>
public interface ISpecification<T>
{
    /// <summary>
    /// Checks if an element satisfies the predicate.
    /// </summary>
    Expression<Func<T, bool>> IsSatisfiedBy { get; }
}