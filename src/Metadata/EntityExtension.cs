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
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Zongsoft.Collections;

namespace Zongsoft.Data.Metadata
{
	/// <summary>
	/// 实体元数据的扩展类。
	/// </summary>
	public static class EntityExtension
	{
		#region 私有变量
		private static readonly ConcurrentDictionary<IEntityMetadata, EntityTokenCache> _cache = new ConcurrentDictionary<IEntityMetadata, EntityTokenCache>();
		#endregion

		#region 公共方法
		/// <summary>
		/// 查找指定实体元素继承的父实体元素。
		/// </summary>
		/// <param name="entity">指定的实体元素。</param>
		/// <returns>如果 <paramref name="entity"/> 参数指定的实体元素设置了继承关系，则返回它继承的父实体元素（如果指定父实体元素不存在，则抛出异常）；否则返回空(null)。</returns>
		public static IEntityMetadata GetBaseEntity(this IEntityMetadata entity)
		{
			if(entity == null || string.IsNullOrEmpty(entity.BaseName))
				return null;

			//优先从指定实体的相同元数据容器中查找
			if(entity.Metadata.Entities.TryGet(entity.BaseName, out var baseEntity))
				return baseEntity;

			//最后从系统的元数据容器集中进行全局查找
			if(entity.Metadata.Manager.Entities.TryGet(entity.BaseName, out baseEntity))
				return baseEntity;

			throw new DataException($"The '{entity.BaseName}' base of '{entity.Name}' entity does not exist.");
		}

		/// <summary>
		/// 获取指定实体元素的继承链（所有的继承元素），从最顶级的根元素开始一直到当前元素本身。
		/// </summary>
		/// <param name="entity">指定的实体元素。</param>
		/// <returns>返回的继承链（即继承关系的实体元素数组）。</returns>
		public static IEntityMetadata[] GetInherits(this IEntityMetadata entity)
		{
			if(entity == null)
				throw new ArgumentNullException(nameof(entity));

			if(string.IsNullOrEmpty(entity.BaseName))
				return new[] { entity };

			var super = entity;
			var stack = new Stack<IEntityMetadata>();

			while(super != null)
			{
				stack.Push(super);
				super = GetBaseEntity(super);
			}

			return stack.ToArray();
		}

		/// <summary>
		/// 获取指定实体元素的表名。
		/// </summary>
		/// <param name="entity">指定的实体元素。</param>
		/// <returns>如果实体元素未声明表名则返回该实体元素名。</returns>
		public static string GetTableName(this IEntityMetadata entity)
		{
			if(entity == null)
				throw new ArgumentNullException(nameof(entity));

			return string.IsNullOrEmpty(entity.Alias) ? entity.Name : entity.Alias;
		}

		/// <summary>
		/// 获取指定实体元素对应于指定数据类型的成员标记集。
		/// </summary>
		/// <param name="entity">指定的实体元素。</param>
		/// <param name="type">指定的数据类型，即实体元素对应到的输入或输出的数据类型。</param>
		/// <returns>返回实体元素对应指定数据类型的成员标记集。</returns>
		public static IReadOnlyNamedCollection<EntityPropertyToken> GetTokens(this IEntityMetadata entity, Type type)
		{
			if(entity == null)
				throw new ArgumentNullException(nameof(entity));

			if(type == null)
				throw new ArgumentNullException(nameof(type));

			return _cache.GetOrAdd(entity, e => new EntityTokenCache(e)).GetTokens(type);
		}
		#endregion

		#region 嵌套子类
		private class EntityTokenCache
		{
			#region 成员字段
			private readonly IEntityMetadata _entity;
			private readonly ConcurrentDictionary<Type, IReadOnlyNamedCollection<EntityPropertyToken>> _cache = new ConcurrentDictionary<Type, IReadOnlyNamedCollection<EntityPropertyToken>>();
			#endregion

			#region 构造函数
			internal EntityTokenCache(IEntityMetadata entity)
			{
				_entity = entity;
			}
			#endregion

			#region 公共方法
			public IReadOnlyNamedCollection<EntityPropertyToken> GetTokens(Type type)
			{
				if(type == null)
					throw new ArgumentNullException(nameof(type));

				return _cache.GetOrAdd(type, t => CreateTokens(t));
			}
			#endregion

			#region 私有方法
			private IReadOnlyNamedCollection<EntityPropertyToken> CreateTokens(Type type)
			{
				var collection = new NamedCollection<EntityPropertyToken>(m => m.Property.Name);

				if(type == null || type == typeof(object) || Zongsoft.Common.TypeExtension.IsDictionary(type))
				{
					foreach(var property in _entity.Properties)
					{
						collection.Add(new EntityPropertyToken(property, null));
					}
				}
				else
				{
					foreach(var property in _entity.Properties)
					{
						var member = (MemberInfo)type.GetField(property.Name, BindingFlags.Public | BindingFlags.Instance) ??
												 type.GetProperty(property.Name, BindingFlags.Public | BindingFlags.Instance);

						if(member != null)
							collection.Add(new EntityPropertyToken(property, member));
					}
				}

				return collection;
			}
			#endregion
		}
		#endregion
	}
}