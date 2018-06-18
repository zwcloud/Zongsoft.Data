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
 * Copyright (C) 2015-2018 Zongsoft Corporation <http://www.zongsoft.com>
 *
 * This file is part of Zongsoft.Data.MySql.
 *
 * Zongsoft.Data.MySql is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * Zongsoft.Data.MySql is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * Lesser General Public License for more details.
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with Zongsoft.Data.MySql; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA
 */

using System;
using System.Text;
using System.Collections.Generic;

using Zongsoft.Data.Common;
using Zongsoft.Data.Common.Expressions;

namespace Zongsoft.Data.MySql
{
	public class MySqlStatementScriptor : StatementScriptorBase
	{
		#region 单例字段
		public static readonly MySqlStatementScriptor Default = new MySqlStatementScriptor();
		#endregion

		#region 构造函数
		protected MySqlStatementScriptor()
		{
		}
		#endregion

		#region 重写方法
		protected override IExpressionVisitor GetVisitor(IExpression expression, StringBuilder text)
		{
			return new MySqlExpressionVisitor(text);
		}

		protected override DeleteStatementVisitor GetDeleteVisitor(DeleteStatement statement, StringBuilder text)
		{
			return new MySqlDeleteStatementVisitor(text);
		}

		protected override InsertStatementVisitor GetInsertVisitor(InsertStatement statement, StringBuilder text)
		{
			return new MySqlInsertStatementVisitor(text);
		}

		protected override UpsertStatementVisitor GetUpsertVisitor(UpsertStatement statement, StringBuilder text)
		{
			return new MySqlUpsertStatementVisitor(text);
		}

		protected override UpdateStatementVisitor GetUpdateVisitor(UpdateStatement statement, StringBuilder text)
		{
			return new MySqlUpdateStatementVisitor(text);
		}

		protected override SelectStatementVisitor GetSelectVisitor(SelectStatement statement, StringBuilder text)
		{
			return new MySqlSelectStatementVisitor(text);
		}
		#endregion
	}
}
