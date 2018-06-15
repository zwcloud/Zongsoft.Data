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
using System.Collections.Generic;

namespace Zongsoft.Data.Common.Expressions
{
	public class MethodExpression : Expression
	{
		#region 构造函数
		protected MethodExpression(string name, MethodType type, IEnumerable<IExpression> arguments)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));

			this.Name = name.Trim();
			this.Type = type;
			this.Arguments = new List<IExpression>(arguments);
		}
		#endregion

		#region 公共属性
		public string Name
		{
			get;
			set;
		}

		public string Alias
		{
			get;
			set;
		}

		public MethodType Type
		{
			get;
		}

		public IList<IExpression> Arguments
		{
			get;
		}
		#endregion

		#region 静态方法
		public static MethodExpression Function(string name, params IExpression[] arguments)
		{
			return new MethodExpression(name, MethodType.Function, arguments);
		}

		public static MethodExpression Function(string name, IEnumerable<IExpression> arguments)
		{
			return new MethodExpression(name, MethodType.Function, arguments);
		}

		public static MethodExpression Procedure(string name, params IExpression[] arguments)
		{
			return new MethodExpression(name, MethodType.Procedure, arguments);
		}

		public static MethodExpression Procedure(string name, IEnumerable<IExpression> arguments)
		{
			return new MethodExpression(name, MethodType.Procedure, arguments);
		}
		#endregion
	}
}