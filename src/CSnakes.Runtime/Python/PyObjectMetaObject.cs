using System.Dynamic;
using System.Linq.Expressions;
using System.Text;

namespace CSnakes.Runtime.Python;

public class PyObjectMetaObject : DynamicMetaObject
{
    private PyObject pyObject;

    public PyObjectMetaObject(Expression expression, PyObject value)
        : base(expression, BindingRestrictions.Empty, value)
    {
        pyObject = value;
    }

    // Override to intercept method calls
    public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
    {
        // Extract the method name and arguments
        string methodName = ToSnakeCase(binder.Name);
        var argExpressions = args.Select(arg =>
        {
            // Check if the argument is already a PyObject; if not, convert it
            var convertToPyObject = Expression.Condition(
                Expression.TypeIs(arg.Expression, typeof(PyObject)),
                arg.Expression, // If it's already a PyObject, use it as-is
                Expression.Call(
                    typeof(PyObject).GetMethod(nameof(PyObject.From)), 
                    Expression.Convert(arg.Expression, typeof(object))) // Convert to PyObject
            );

            return Expression.Convert(convertToPyObject, typeof(object));
        }).ToArray();

        Expression instance = Expression.Convert(Expression, typeof(PyObject));
        Expression callMethod = Expression.Call(
            instance,
            typeof(PyObject).GetMethod(nameof(PyObject.Call)),
            Expression.NewArrayInit(typeof(PyObject), argExpressions)
        );

        // Return the new dynamic meta-object with the call expression
        return new DynamicMetaObject(callMethod, BindingRestrictions.GetTypeRestriction(Expression, LimitType));
    }

    public static string ToSnakeCase(string method)
    {
        if (string.IsNullOrEmpty(method))
            return method;

        var sb = new StringBuilder();
        bool wasPrevLower = false;

        foreach (char c in method)
        {
            if (char.IsUpper(c))
            {
                // Add underscore before uppercase letter, except at the start or after another underscore
                if (sb.Length > 0 && wasPrevLower)
                {
                    sb.Append('_');
                }
                sb.Append(char.ToLower(c));
                wasPrevLower = false;
            }
            else
            {
                sb.Append(c);
                wasPrevLower = true;
            }
        }

        return sb.ToString();
    }
}
