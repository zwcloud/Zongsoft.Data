﻿using System;
using System.Collections;

namespace Zongsoft.Data.Common.Expressions
{
	public static class ConditionExtension
	{
		#region 公共方法
		public static BinaryExpression ToExpression(this Condition condition,
		                                            Func<string, FieldIdentifier> fieldThunk,
		                                            Func<Condition, FieldIdentifier, IExpression> valueThunk = null)
		{
			if(condition == null)
				throw new ArgumentNullException(nameof(condition));

			if(fieldThunk == null)
				throw new ArgumentNullException(nameof(fieldThunk));

			var field = fieldThunk(condition.Name);
			var value = EnsureConditionValue(condition, field, valueThunk);

			if(value == null)
				return null;

			switch(condition.Operator)
			{
				case ConditionOperator.Between:
					if(value is RangeExpression range)
						return Expression.Between(field, range);
					else
						return null;
				case ConditionOperator.Like:
					return Expression.Like(field, value);
				case ConditionOperator.In:
					return Expression.In(field, value);
				case ConditionOperator.NotIn:
					return Expression.NotIn(field, value);
				case ConditionOperator.Equal:
					return Expression.Equal(field, value);
				case ConditionOperator.NotEqual:
					return Expression.NotEqual(field, value);
				case ConditionOperator.GreaterThan:
					return Expression.GreaterThan(field, value);
				case ConditionOperator.GreaterThanEqual:
					return Expression.GreaterThanOrEqual(field, value);
				case ConditionOperator.LessThan:
					return Expression.LessThan(field, value);
				case ConditionOperator.LessThanEqual:
					return Expression.LessThanOrEqual(field, value);
				default:
					throw new NotSupportedException($"Invalid '{condition.Operator}' condition operator.");
			}
		}

		public static ConditionExpression ToExpression(this IConditional conditions,
		                                               Func<string, FieldIdentifier> map,
		                                               Func<Condition, FieldIdentifier, IExpression> valueThunk = null)
		{
			if(conditions == null)
				throw new ArgumentNullException(nameof(conditions));

			ConditionExpression expressions = new ConditionExpression(conditions.Combination);

			foreach(var condition in conditions)
			{
				switch(condition)
				{
					case Condition c:
						var item = ToExpression(c, map, valueThunk);

						if(item != null)
							expressions.Add(item);

						break;
					case IConditional cc:
						var items = ToExpression(cc, map, valueThunk);

						if(items != null && items.Count > 0)
							expressions.Add(items);

						break;
				}
			}

			return expressions.Count > 0 ? expressions : null;
		}
		#endregion

		private static IExpression EnsureConditionValue(Condition condition, FieldIdentifier field, Func<Condition, FieldIdentifier, IExpression> valueThunk = null)
		{
			switch(condition.Operator)
			{
				case ConditionOperator.Between:
					if(condition.Value == null)
						return null;

					if(valueThunk != null)
						return valueThunk(condition, field);

					if(condition.Value is Array array && array.Length > 1)
						return new RangeExpression(Expression.Constant(array.GetValue(0)), Expression.Constant(array.GetValue(1)));

					return null;
				case ConditionOperator.In:
				case ConditionOperator.NotIn:
					if(condition.Value != null && condition.Value is IEnumerable iterator)
					{
						var collection = new ExpressionCollection();

						foreach(var item in iterator)
						{
							if(item != null)
								collection.Add(Expression.Constant(item));
						}

						if(collection.Count > 0)
							return collection;
					}

					return null;
				default:
					if(valueThunk == null)
						return Expression.Constant(condition.Value);
					else
						return valueThunk(condition, field);
			}
		}
	}
}
