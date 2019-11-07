﻿/*
 *   _____                                ______
 *  /_   /  ____  ____  ____  _________  / __/ /_
 *    / /  / __ \/ __ \/ __ \/ ___/ __ \/ /_/ __/
 *   / /__/ /_/ / / / / /_/ /\_ \/ /_/ / __/ /_
 *  /____/\____/_/ /_/\__  /____/\____/_/  \__/
 *                   /____/
 *
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@qq.com>
 *
 * Copyright (C) 2015-2019 Zongsoft Corporation <http://www.zongsoft.com>
 *
 * This file is part of Zongsoft.Data.
 *
 * Zongsoft.Data is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * Zongsoft.Data is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * Lesser General Public License for more details.
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with Zongsoft.Data; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA
 */

using System;
using System.Collections;
using System.Collections.Generic;

using Zongsoft.Data.Metadata;
using Zongsoft.Data.Common.Expressions;

namespace Zongsoft.Data.Common
{
	public abstract class DataMutateExecutor<TStatement> : IDataExecutor<TStatement> where TStatement : IMutateStatement
	{
		#region 执行方法
		public void Execute(IDataAccessContext context, TStatement statement)
		{
			if(context is IDataMutateContext ctx)
				this.OnExecute(ctx, statement);
		}

		protected virtual void OnExecute(IDataMutateContext context, TStatement statement)
		{
			//根据生成的脚本创建对应的数据命令
			var command = context.Session.Build(statement);

			//获取当前操作是否为多数据
			var isMultiple = statement.Schema == null ?
				context.IsMultiple :
				statement.Schema.Token.IsMultiple;

			if(isMultiple && context.Data != null)
			{
				foreach(var item in (IEnumerable)context.Data)
				{
					//更新当前操作数据
					context.Data = item;

					this.Mutate(context, statement, command);
				}
			}
			else
			{
				this.Mutate(context, statement, command);
			}
		}
		#endregion

		#region 虚拟方法
		protected virtual bool OnMutated(IDataMutateContext context, TStatement statement, System.Data.IDataReader reader)
		{
			if(context is DataIncrementContextBase increment)
			{
				if(reader.Read())
					increment.Result = reader.IsDBNull(0) ? 0 : (long)Convert.ChangeType(reader.GetValue(0), TypeCode.Int64);
				else
					return false;
			}

			return true;
		}

		protected virtual bool OnMutated(IDataMutateContext context, TStatement statement, int count)
		{
			return count > 0;
		}

		protected virtual void OnMutating(IDataMutateContext context, TStatement statement)
		{
		}
		#endregion

		#region 私有方法
		private bool Mutate(IDataMutateContext context, TStatement statement, System.Data.Common.DbCommand command)
		{
			bool keeping;

			//调用写入操作开始方法
			this.OnMutating(context, statement);

			//绑定命令参数
			statement.Bind(context, command, context.Data);

			if(statement.Returning != null && statement.Returning.Table == null)
			{
				//调用写入操作完成方法
				keeping = this.OnMutated(context, statement, command.ExecuteReader());
			}
			else
			{
				//执行数据命令操作
				var count = command.ExecuteNonQuery();

				//累加总受影响的记录数
				context.Count += count;

				//调用写入操作完成方法
				keeping = this.OnMutated(context, statement, count);
			}

			//如果需要继续并且有从属语句则尝试绑定从属写操作数据
			if(keeping && statement.HasSlaves)
				this.Bind(context, statement.Slaves);

			return keeping;
		}

		private void Bind(IDataMutateContext context, IEnumerable<IStatementBase> statements)
		{
			if(context.Data == null)
				return;

			foreach(var statement in statements)
			{
				if(statement is IMutateStatement mutation)
				{
					//设置子新增语句中的关联参数值
					this.SetLinkedParameters(mutation, context.Data);

					//重新计算当前的操作数据
					context.Data = mutation.Schema.Token.GetValue(context.Data);
				}
			}
		}

		private void SetLinkedParameters(IMutateStatement statement, object data)
		{
			if(statement.Schema == null || statement.Schema.Token.Property.IsSimplex)
				return;

			var complex = (IDataEntityComplexProperty)statement.Schema.Token.Property;

			foreach(var link in complex.Links)
			{
				var parameter = statement.Parameters[link.Foreign.Name];
				parameter.Value = this.GetValue(data, link.Principal.Name);
			}
		}

		private object GetValue(object target, string name)
		{
			if(target is IDictionary<string, object> generic)
				return generic.TryGetValue(name, out var value) ? value : null;

			if(target is IDictionary classic)
				return classic.Contains(name) ? classic[name] : null;

			return Reflection.Reflector.GetValue(target, name);
		}
		#endregion
	}
}
