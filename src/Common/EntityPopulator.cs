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
using System.Data;

namespace Zongsoft.Data.Common
{
	public class EntityPopulator : IDataPopulator
	{
		#region 成员字段
		private readonly Type _type;
		private readonly EntityMember[] _members;
		private readonly Func<Type, IDataRecord, object> _creator;
		#endregion

		#region 构造函数
		internal protected EntityPopulator(Type type, EntityMember[] members)
		{
			_type = type ?? throw new ArgumentNullException(nameof(type));
			_members = members ?? throw new ArgumentNullException(nameof(members));
			_creator = this.GetCreator();
		}
		#endregion

		#region 公共方法
		public object Populate(IDataRecord record)
		{
			if(record.FieldCount != _members.Length)
				throw new DataException("The record of populate has failed.");

			//创建一个对应的实体对象
			var entity = _creator(_type, record);

			//装配实体属性集
			for(var i = 0; i < record.FieldCount; i++)
			{
				if(_members[i] != null)
					_members[i].Populate(entity, record, i);
			}

			//返回装配完成的实体对象
			return entity;
		}
		#endregion

		#region 虚拟方法
		protected virtual Func<Type, IDataRecord, object> GetCreator()
		{
			return (type, record) => System.Activator.CreateInstance(type);
		}
		#endregion
	}
}
