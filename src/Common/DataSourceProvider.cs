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

using Zongsoft.Options;
using Zongsoft.Options.Configuration;

namespace Zongsoft.Data.Common
{
	public class DataSourceProvider : IDataSourceProvider
	{
		#region 单例字段
		public static readonly DataSourceProvider Default = new DataSourceProvider();
		#endregion

		#region 私有构造
		private DataSourceProvider()
		{
		}
		#endregion

		#region 公共方法
		public IEnumerable<IDataSource> GetSources(string name)
		{
			var connectionStrings = OptionManager.Default.GetOptionValue("/Data/ConnectionStrings") as ConnectionStringElementCollection;

			if(connectionStrings != null)
			{
				foreach(ConnectionStringElement connectionString in connectionStrings)
				{
					if(string.Equals(connectionString.Name, name, StringComparison.OrdinalIgnoreCase) ||
					   connectionString.Name.StartsWith(name + ":", StringComparison.OrdinalIgnoreCase))
						yield return new DataSource(connectionString);
				}
			}
		}
		#endregion
	}
}