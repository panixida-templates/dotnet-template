using System.Linq.Expressions;

namespace Dal.Implementations.Core;

internal class OrderedQueryableVisitor : ExpressionVisitor
{
    public bool IsOrdered { get; private set; }

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        if (node.Method.DeclaringType == typeof(Queryable) &&
            (node.Method.Name == nameof(Queryable.OrderBy) || node.Method.Name == nameof(Queryable.OrderByDescending)))
        {
            IsOrdered = true;
        }

        return base.VisitMethodCall(node);
    }
}
