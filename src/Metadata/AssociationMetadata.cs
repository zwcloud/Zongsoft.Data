﻿/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
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

namespace Zongsoft.Data.Metadata
{
	/// <summary>
	/// 表示数据实体关系的元数据类。
	/// </summary>
	public class AssociationMetadata : IEntityAssociation
	{
		#region 成员字段
		private IEntity _principal;
		private IEntity _foreign;
		private IEntityAssociationMember[] _members;
		#endregion

		#region 构造函数
		public AssociationMetadata(IEntity principal, string[] principalKeys, IEntity foreign, string[] foreignKeys)
		{
			_principal = principal ?? throw new ArgumentNullException(nameof(principal));
			_foreign = foreign ?? throw new ArgumentNullException(nameof(foreign));

			if(principalKeys == null || principalKeys.Length == 0)
				throw new ArgumentNullException(nameof(principalKeys));
			if(foreignKeys == null || foreignKeys.Length == 0)
				throw new ArgumentNullException(nameof(foreignKeys));
			if(principalKeys.Length != foreignKeys.Length)
				throw new ArgumentException();

			_members = new AssociationMemberMetadata[principalKeys.Length];

			for(int i = 0; i < principalKeys.Length; i++)
			{
				if(!principal.Properties.TryGet(principalKeys[i], out var principalKey))
					throw new ArgumentException();

				if(!principalKey.IsSimplex)
					throw new ArgumentException();

				if(!foreign.Properties.TryGet(foreignKeys[i], out var foreignKey))
					throw new ArgumentException();

				if(!foreignKey.IsSimplex)
					throw new ArgumentException();

				_members[i] = new AssociationMemberMetadata((EntitySimplexPropertyMetadata)principalKey, (EntitySimplexPropertyMetadata)foreignKey);
			}
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取关系中的宿主数据实体。
		/// </summary>
		public IEntity Principal
		{
			get
			{
				return _principal;
			}
		}

		/// <summary>
		/// 获取关系中的外部数据实体。
		/// </summary>
		public IEntity Foreign
		{
			get
			{
				return _foreign;
			}
		}

		/// <summary>
		/// 获取关系中的成员对应数组。
		/// </summary>
		public IEntityAssociationMember[] Members
		{
			get
			{
				return _members;
			}
		}
		#endregion

		#region 重写方法
		public override string ToString()
		{
			var text = new System.Text.StringBuilder();

			foreach(var member in _members)
			{
				if(text.Length > 0)
					text.Append(" AND ");

				text.Append(member.Principal.Name + "=" + member.Foreign.Name);
			}

			return $"{_principal} <-> {_foreign} ({text.ToString()})";
		}
		#endregion

		#region 嵌套子类
		/// <summary>
		/// 表示数据实体关系的成员元数据类。
		/// </summary>
		public class AssociationMemberMetadata : IEntityAssociationMember
		{
			#region 成员字段
			private IEntitySimplexProperty _principal;
			private IEntitySimplexProperty _foreign;
			#endregion

			#region 构造函数
			public AssociationMemberMetadata(IEntitySimplexProperty principal, IEntitySimplexProperty foreign)
			{
				_principal = principal ?? throw new ArgumentNullException(nameof(principal));
				_foreign = foreign ?? throw new ArgumentNullException(nameof(foreign));
			}
			#endregion

			#region 公共属性
			/// <summary>
			/// 获取关系中宿主数据实体中的成员。
			/// </summary>
			public IEntitySimplexProperty Principal
			{
				get
				{
					return _principal;
				}
			}

			/// <summary>
			/// 获取关系中外部数据实体中的成员。
			/// </summary>
			public IEntitySimplexProperty Foreign
			{
				get
				{
					return _foreign;
				}
			}
			#endregion
		}
		#endregion
	}
}