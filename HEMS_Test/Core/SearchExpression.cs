using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HEMS_Test.Core
{
  public class SearchExpression
  {
    public string Name { get; set; }
    public SearchFunction Function { get; set; }
    public string Value { get; set; }

    public bool IsLeaf
    {
      get
      {
        return Children == null || Children.Count == 0;
      }
    }
    public List<SearchExpression> Children { get; set; }

    public LogicalOperation ChildrenOperation { get; set; }
    //public bool IsExtended { get; set; }

    public SearchExpression()
    {
    }
    public SearchExpression(JToken obj)
    {
      if (obj["Children"] != null && obj["Children"].Count() > 0)
      {
        ChildrenOperation = (LogicalOperation)((int?)obj["ChildrenOperation"]);
        Children = new List<SearchExpression>();
        foreach (var item in obj["Children"])
        {
          Children.Add(new SearchExpression(item));
        }
      }
      else
      {
        Name = (string)obj["Name"];
        Value = (string)obj["Value"];
        Function = (SearchFunction)((int?)obj["Function"]);
      }
    }


    public SearchExpression(List<SearchExpression> _subConitions, LogicalOperation op = LogicalOperation.Or)
    {

      Children = _subConitions;
      ChildrenOperation = op;
    }
    public SearchExpression(string name, SearchFunction cond, string val)
    {
      Name = name;
      Value = val;
      Function = cond;
    }

    public static Expression<Func<T, bool>> GetExpression<T>(SearchExpression search) where T : class
    {
      ParameterExpression param = Expression.Parameter(typeof(T), "x");
      try
      {
        Expression result = Expression.Constant(true);
        if (search != null)
        {

          Expression exp = search.BuildLambdaExpression(param);
          if (exp != null)
          {
            result = Expression.And(result, exp);
          }


        }


        return Expression.Lambda<Func<T, bool>>(result, param);
      }
      catch (Exception eeee)
      {

        return null;
      }


    }

    public Expression BuildLambdaExpression(ParameterExpression param)
    {
      if (IsLeaf)
      {


        return BuildLmbdaExpressionBasic(this, param);
      }
      else
      {
        Expression result = Expression.Constant(true);
        if (this.ChildrenOperation == LogicalOperation.Or)
        {
          result = Expression.Constant(false);
        }
        foreach (var cond in this.Children)
        {
          if (this.ChildrenOperation == LogicalOperation.Or)
          {
            result = Expression.Or(result, cond.BuildLambdaExpression(param) ?? Expression.Constant(false));
          }
          else
          {
            result = Expression.And(result, cond.BuildLambdaExpression(param) ?? Expression.Constant(true));
          }
        }
        return result;
      }


    }

    private Expression BuildLmbdaExpressionBasic(SearchExpression cond, ParameterExpression param)
    {
      Expression res = null;
      Expression member = param;
      foreach (var p in cond.Name.Split('.'))
      {
        member = Expression.Property(member, p);
      }
      Type underType;
      if (Nullable.GetUnderlyingType(member.Type) == null)
      {
        underType = member.Type;
      }
      else
      {
        underType = Nullable.GetUnderlyingType(member.Type);
      }
      if (underType.IsEnum)
      {
        underType = typeof(int);
      }
      Expression constant = null;
      List<Expression> constants = new List<Expression>();
      MethodInfo method = null;


      switch (cond.Function)
      {
        case SearchFunction.Equal:
          #region Equal
          {
            if (string.IsNullOrEmpty(cond.Value)) { return Expression.Constant(false); }
            else
            {

              object value =
             underType == typeof(string) ? cond.Value :
                  (
                      underType == typeof(DateTime) ?
                      Convert.ChangeType(underType.GetMethod("ParseExact", new Type[] { typeof(string), typeof(string), typeof(IFormatProvider) })
                              .Invoke(null, new object[] { cond.Value, "dd/MM/yyyy", CultureInfo.InvariantCulture.DateTimeFormat }), underType) :
                      Convert.ChangeType(underType.GetMethod("Parse", new Type[] { typeof(string) })
                              .Invoke(null, new object[] { cond.Value }), underType)
                  );

              constant = Expression.Constant(value);
            }
            res = Expression.Equal(member, Expression.Convert(constant, member.Type));
          }
          break;
        #endregion
        case SearchFunction.NotEqual:
          #region NotEqual
          {
            if (string.IsNullOrEmpty(cond.Value)) { return Expression.Constant(false); }
            else
            {
              object value =
            underType == typeof(string) ? cond.Value :
                  (
                      underType == typeof(DateTime) ?
                      Convert.ChangeType(underType.GetMethod("ParseExact", new Type[] { typeof(string), typeof(string), typeof(IFormatProvider) })
                              .Invoke(null, new object[] { cond.Value, "dd/MM/yyyy", CultureInfo.InvariantCulture.DateTimeFormat }), underType) :
                      Convert.ChangeType(underType.GetMethod("Parse", new Type[] { typeof(string) })
                              .Invoke(null, new object[] { cond.Value }), underType)
                  );

              constant = Expression.Constant(value);
            }
            res = Expression.NotEqual(member, Expression.Convert(constant, member.Type));
          }
          break;
        #endregion
        case SearchFunction.Like:
          #region Like
          if (string.IsNullOrEmpty(cond.Value)) { return null; }
          else
          {
            object value =
           underType == typeof(string) ? cond.Value :
                    (
                        Convert.ChangeType(underType.GetMethod("Parse", new Type[] { typeof(string) })
                                .Invoke(null, new object[] { cond.Value }), underType)
                    );

            constant = Expression.Constant(value);
          }
          method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
          res = Expression.Call(member, method, constant);
          break;
        #endregion
        case SearchFunction.NotLike:
          #region NotLike
          if (string.IsNullOrEmpty(cond.Value)) { return Expression.Constant(false); }
          else
          {
            object value =
           underType == typeof(string) ? cond.Value :
                    (
                        Convert.ChangeType(underType.GetMethod("Parse", new Type[] { typeof(string) })
                                .Invoke(null, new object[] { cond.Value }), underType)
                    );

            constant = Expression.Constant(value);
          }
          method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
          res = Expression.Not(Expression.Call(member, method, constant));
          break;
        #endregion
        case SearchFunction.Null:
          #region Null
          {
            res = Expression.Or(Expression.Equal(member, Expression.Constant("")), Expression.Equal(member, Expression.Constant(null)));
          }
          break;
        #endregion
        case SearchFunction.NotNull:
          #region NotNull
          {
            res = Expression.And(Expression.NotEqual(member, Expression.Constant("")), Expression.NotEqual(member, Expression.Constant(null)));
          }
          break;
        #endregion
        case SearchFunction.In:
          #region In
          {
            if (string.IsNullOrEmpty(cond.Value))
            { return Expression.Constant(false); }
            else
            {
              List<string> objectStrs = cond.Value.Split(',').ToList();


              objectStrs.ForEach(s =>
              {
                object value = string.IsNullOrEmpty(s) ?
                         null :
                         (underType == typeof(string) ? s :
                              (
                                  underType == typeof(DateTime) ?
                                  Convert.ChangeType(underType.GetMethod("ParseExact", new Type[] { typeof(string), typeof(string), typeof(IFormatProvider) })
                                          .Invoke(null, new object[] { s, "dd/MM/yyyy", CultureInfo.InvariantCulture.DateTimeFormat }), underType) :
                                  Convert.ChangeType(underType.GetMethod("Parse", new Type[] { typeof(string) })
                                          .Invoke(null, new object[] { s }), underType)
                              ));

                constants.Add(Expression.Constant(value));
              });
            }
            Expression exp = Expression.Constant(false);
            constants.ForEach(xc =>
            {
              exp = Expression.Or(exp, Expression.Equal(member, Expression.Convert(xc, member.Type)));
            });
            res = exp;
          }
          break;
        #endregion
        case SearchFunction.NotIn:
          #region NotIn
          {
            if (string.IsNullOrEmpty(cond.Value))
            { return Expression.Constant(false); }
            else
            {
              List<string> objectStrs = cond.Value.Split(',').ToList();


              objectStrs.ForEach(s =>
              {
                object value = string.IsNullOrEmpty(s) ?
                         null :
                         (underType == typeof(string) ? s :
                              (
                                  underType == typeof(DateTime) ?
                                  Convert.ChangeType(underType.GetMethod("ParseExact", new Type[] { typeof(string), typeof(string), typeof(IFormatProvider) })
                                          .Invoke(null, new object[] { s, "dd/MM/yyyy", CultureInfo.InvariantCulture.DateTimeFormat }), underType) :
                                  Convert.ChangeType(underType.GetMethod("Parse", new Type[] { typeof(string) })
                                          .Invoke(null, new object[] { s }), underType)
                              ));

                constants.Add(Expression.Constant(value));
              });
            }
            Expression exp = Expression.Constant(true);
            constants.ForEach(xc =>
            {
              exp = Expression.And(exp, Expression.NotEqual(member, Expression.Convert(xc, member.Type)));
            });
            res = exp;
          }
          break;
        #endregion
        case SearchFunction.Between:
          #region Between
          {
            if (string.IsNullOrEmpty(cond.Value))
            { return Expression.Constant(false); }
            else
            {
              List<string> objectStrs = cond.Value.Split(',').ToList();


              objectStrs.ForEach(s =>
              {
                object value = string.IsNullOrEmpty(s) ?
                         null :
                         (underType == typeof(string) ? s :
                              (
                                  underType == typeof(DateTime) ?
                                  Convert.ChangeType(underType.GetMethod("ParseExact", new Type[] { typeof(string), typeof(string), typeof(IFormatProvider) })
                                          .Invoke(null, new object[] { s, "dd/MM/yyyy", CultureInfo.InvariantCulture.DateTimeFormat }), underType) :
                                  Convert.ChangeType(underType.GetMethod("Parse", new Type[] { typeof(string) })
                                          .Invoke(null, new object[] { s }), underType)
                              ));

                constants.Add(Expression.Constant(value));
              });
            }

            res = Expression.And(Expression.GreaterThanOrEqual(member, Expression.Convert(constants[0], member.Type)), Expression.LessThanOrEqual(member, Expression.Convert(constants[1], member.Type)));

          }
          break;
        #endregion
        case SearchFunction.NotBetween:
          #region NotBetween
          {
            if (string.IsNullOrEmpty(cond.Value))
            { return Expression.Constant(false); }
            else
            {
              List<string> objectStrs = cond.Value.Split(',').ToList();


              objectStrs.ForEach(s =>
              {
                object value = string.IsNullOrEmpty(s) ?
                         null :
                         (underType == typeof(string) ? s :
                              (
                                  underType == typeof(DateTime) ?
                                  Convert.ChangeType(underType.GetMethod("ParseExact", new Type[] { typeof(string), typeof(string), typeof(IFormatProvider) })
                                          .Invoke(null, new object[] { s, "dd/MM/yyyy", CultureInfo.InvariantCulture.DateTimeFormat }), underType) :
                                  Convert.ChangeType(underType.GetMethod("Parse", new Type[] { typeof(string) })
                                          .Invoke(null, new object[] { s }), underType)
                              ));

                constants.Add(Expression.Constant(value));
              });
            }

            res = Expression.Or(Expression.LessThan(member, Expression.Convert(constants[0], member.Type)), Expression.GreaterThan(member, Expression.Convert(constants[1], member.Type)));

          }
          break;
        #endregion
        case SearchFunction.GreaterThan:
          #region GreaterThan
          {
            if (string.IsNullOrEmpty(cond.Value)) { return Expression.Constant(false); }
            else
            {
              object value =
             underType == typeof(string) ? cond.Value :
                  (
                      underType == typeof(DateTime) ?
                      Convert.ChangeType(underType.GetMethod("ParseExact", new Type[] { typeof(string), typeof(string), typeof(IFormatProvider) })
                              .Invoke(null, new object[] { cond.Value, "dd/MM/yyyy", CultureInfo.InvariantCulture.DateTimeFormat }), underType) :
                      Convert.ChangeType(underType.GetMethod("Parse", new Type[] { typeof(string) })
                              .Invoke(null, new object[] { cond.Value }), underType)
                  );

              constant = Expression.Constant(value);
            }
            res = Expression.GreaterThan(member, Expression.Convert(constant, member.Type));
          }
          break;
        #endregion
        case SearchFunction.GreaterThanOrEqual:
          #region GreaterThanOrEqual
          {
            if (string.IsNullOrEmpty(cond.Value)) { return Expression.Constant(false); }
            else
            {
              object value =
             underType == typeof(string) ? cond.Value :
                  (
                      underType == typeof(DateTime) ?
                      Convert.ChangeType(underType.GetMethod("ParseExact", new Type[] { typeof(string), typeof(string), typeof(IFormatProvider) })
                              .Invoke(null, new object[] { cond.Value, "dd/MM/yyyy", CultureInfo.InvariantCulture.DateTimeFormat }), underType) :
                      Convert.ChangeType(underType.GetMethod("Parse", new Type[] { typeof(string) })
                              .Invoke(null, new object[] { cond.Value }), underType)
                  );

              constant = Expression.Constant(value);
            }
            res = Expression.GreaterThanOrEqual(member, Expression.Convert(constant, member.Type));
          }
          break;
        #endregion
        case SearchFunction.LessThan:
          #region LessThan
          {
            if (string.IsNullOrEmpty(cond.Value)) { return Expression.Constant(false); }
            else
            {
              object value =
             underType == typeof(string) ? cond.Value :
                  (
                      underType == typeof(DateTime) ?
                      Convert.ChangeType(underType.GetMethod("ParseExact", new Type[] { typeof(string), typeof(string), typeof(IFormatProvider) })
                              .Invoke(null, new object[] { cond.Value, "dd/MM/yyyy", CultureInfo.InvariantCulture.DateTimeFormat }), underType) :
                      Convert.ChangeType(underType.GetMethod("Parse", new Type[] { typeof(string) })
                              .Invoke(null, new object[] { cond.Value }), underType)
                  );

              constant = Expression.Constant(value);
            }
            res = Expression.LessThan(member, Expression.Convert(constant, member.Type));
          }
          break;
        #endregion
        case SearchFunction.LessThanOrEqual:
          #region LessThanOrEqual
          {
            if (string.IsNullOrEmpty(cond.Value)) { return Expression.Constant(false); }
            else
            {
              object value =
             underType == typeof(string) ? cond.Value :
                  (
                      underType == typeof(DateTime) ?
                      Convert.ChangeType(underType.GetMethod("ParseExact", new Type[] { typeof(string), typeof(string), typeof(IFormatProvider) })
                              .Invoke(null, new object[] { cond.Value, "dd/MM/yyyy", CultureInfo.InvariantCulture.DateTimeFormat }), underType) :
                      Convert.ChangeType(underType.GetMethod("Parse", new Type[] { typeof(string) })
                              .Invoke(null, new object[] { cond.Value }), underType)
                  );

              constant = Expression.Constant(value);
            }
            res = Expression.LessThanOrEqual(member, Expression.Convert(constant, member.Type));
          }
          break;
          #endregion
      }
      return res;
    }


  }

  public enum SearchFunction
  {
    Equal = 0,
    NotEqual = 1,

    Like = 10,
    NotLike = 11,

    Null = 20,
    NotNull = 21,

    In = 30,
    NotIn = 31,

    Between = 40,
    NotBetween = 41,

    GreaterThan = 50,
    GreaterThanOrEqual = 51,

    LessThan = 60,
    LessThanOrEqual = 61,

  }

  public enum LogicalOperation
  {
    And = 0,
    Or = 1,
  }
}
