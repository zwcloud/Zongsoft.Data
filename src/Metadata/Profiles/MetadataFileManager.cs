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
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Zongsoft.Data.Metadata.Profiles
{
	public class MetadataFileManager : IMetadataManager
	{
		#region 私有常量
		private const int LOADED = 1;
		private const int UNLOAD = 0;
		#endregion

		#region 成员字段
		private readonly string _name;
		private readonly object _syncRoot;
		private readonly MetadataCollection _metadatas;
		private readonly CommandCollection _commands;
		private readonly EntityCollection _entities;

		private int _loading;
		private IMetadataLoader _loader;
		#endregion

		#region 构造函数
		public MetadataFileManager(string name)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));

			_name = name.Trim();
			_syncRoot = new object();
			_loader = new MetadataFileLoader();
			_metadatas = new MetadataCollection(this);
			_commands = new CommandCollection(_metadatas);
			_entities = new EntityCollection(_metadatas);
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取元数据管理器所属的应用名。
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
		}

		/// <summary>
		/// 获取或设置当前应用的元数据文件加载器。
		/// </summary>
		public IMetadataLoader Loader
		{
			get
			{
				return _loader;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				if(object.ReferenceEquals(_loader, value))
					return;

				_loader = value;

				//如果当前元数据文件已经加载完成，则启动重新加载
				if(_loading != UNLOAD)
					this.Reload();
			}
		}

		/// <summary>
		/// 获取元数据提供程序集（即数据映射文件集合）。
		/// </summary>
		public ICollection<IMetadata> Metadatas
		{
			get
			{
				//如果未加载过元数据提供程序，则尝试加载它
				if(_loading == UNLOAD)
					this.EnsureLoad();

				return _metadatas;
			}
		}

		/// <summary>
		/// 获取全局的实体元数据集。
		/// </summary>
		public Collections.IReadOnlyNamedCollection<IEntity> Entities
		{
			get
			{
				//如果未加载过元数据提供程序，则尝试加载它
				if(_loading == UNLOAD)
					this.EnsureLoad();

				return _entities;
			}
		}

		/// <summary>
		/// 获取全局的命令元数据集。
		/// </summary>
		public Collections.IReadOnlyNamedCollection<ICommand> Commands
		{
			get
			{
				//如果未加载过元数据提供程序，则尝试加载它
				if(_loading == UNLOAD)
					this.EnsureLoad();

				return _commands;
			}
		}
		#endregion

		#region 加载方法
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
		public void Reload()
		{
			var loader = _loader;

			//如果加载器为空则退出
			if(loader == null)
				return;

			//加载当前应用的所有元数据提供程序
			var items = loader.Load(_name);

			_metadatas.Clear();
			_metadatas.AddRange(items);
		}
		#endregion

		#region 私有方法
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		private void EnsureLoad()
		{
			if(_loading == UNLOAD)
			{
				var original = System.Threading.Interlocked.Exchange(ref _loading, LOADED);

				if(original == UNLOAD)
					this.Reload();
			}
		}
		#endregion

		#region 嵌套子类
		private class MetadataCollection : ICollection<IMetadata>
		{
			#region 成员字段
			private readonly IMetadataManager _manager;
			private readonly List<IMetadata> _items;
			#endregion

			#region 构造函数
			public MetadataCollection(IMetadataManager manager)
			{
				_manager = manager;
				_items = new List<IMetadata>();
			}
			#endregion

			#region 公共属性
			public int Count => _items.Count;

			public bool IsReadOnly => false;
			#endregion

			#region 公共方法
			public void Add(IMetadata item)
			{
				if(item == null)
					throw new ArgumentNullException(nameof(item));

				if(string.IsNullOrEmpty(item.Name) || string.Equals(_manager.Name, item.Name, StringComparison.OrdinalIgnoreCase))
				{
					item.Manager = _manager;
					_items.Add(item);
				}
				else
					throw new ArgumentException($"The '{item.Name}' metadata provider is invalid.");
			}

			public void AddRange(IEnumerable<IMetadata> items)
			{
				if(items == null)
					return;

				foreach(var item in items)
				{
					if(!string.IsNullOrEmpty(item.Name) &&
					   !string.Equals(_manager.Name, item.Name, StringComparison.OrdinalIgnoreCase))
						throw new ArgumentException($"The '{item.Name}' metadata provider is invalid.");

					item.Manager = _manager;
				}

				_items.AddRange(items);
			}

			public void Clear()
			{
				_items.Clear();
			}

			public bool Contains(IMetadata item)
			{
				if(item == null)
					return false;

				return _items.Contains(item);
			}

			public void CopyTo(IMetadata[] array, int arrayIndex)
			{
				_items.CopyTo(array, arrayIndex);
			}

			public bool Remove(IMetadata item)
			{
				if(item == null)
					return false;

				return _items.Remove(item);
			}
			#endregion

			#region 枚举遍历
			public IEnumerator<IMetadata> GetEnumerator()
			{
				return _items.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return _items.GetEnumerator();
			}
			#endregion
		}

		private class EntityCollection : Collections.IReadOnlyNamedCollection<IEntity>
		{
			#region 成员字段
			private readonly ICollection<IMetadata> _metadatas;
			#endregion

			#region 构造函数
			public EntityCollection(ICollection<IMetadata> metadatas)
			{
				_metadatas = metadatas ?? throw new ArgumentNullException(nameof(metadatas));
			}
			#endregion

			#region 公共成员
			public int Count
			{
				get
				{
					return _metadatas.Sum(p => p.Entities.Count);
				}
			}

			public bool Contains(string name)
			{
				return _metadatas.Any(p => p.Entities.Contains(name));
			}

			public IEntity Get(string name)
			{
				IEntity entity;

				foreach(var metadata in _metadatas)
				{
					if(metadata.Entities.TryGet(name, out entity))
						return entity;
				}

				throw new KeyNotFoundException($"The specified '{name}' entity does not exist.");
			}

			public bool TryGet(string name, out IEntity value)
			{
				value = null;

				foreach(var metadata in _metadatas)
				{
					if(metadata.Entities.TryGet(name, out value))
						return true;
				}

				return false;
			}

			public IEnumerator<IEntity> GetEnumerator()
			{
				foreach(var metadata in _metadatas)
				{
					foreach(var entity in metadata.Entities)
						yield return entity;
				}
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}
			#endregion
		}

		private class CommandCollection : Collections.IReadOnlyNamedCollection<ICommand>
		{
			#region 成员字段
			private readonly ICollection<IMetadata> _metadatas;
			#endregion

			#region 构造函数
			public CommandCollection(ICollection<IMetadata> metadatas)
			{
				_metadatas = metadatas ?? throw new ArgumentNullException(nameof(metadatas));
			}
			#endregion

			#region 公共成员
			public int Count
			{
				get
				{
					return _metadatas.Sum(p => p.Commands.Count);
				}
			}

			public bool Contains(string name)
			{
				return _metadatas.Any(p => p.Commands.Contains(name));
			}

			public ICommand Get(string name)
			{
				ICommand command;

				foreach(var metadata in _metadatas)
				{
					if(metadata.Commands.TryGet(name, out command))
						return command;
				}

				throw new KeyNotFoundException($"The specified '{name}' command does not exist.");
			}

			public bool TryGet(string name, out ICommand value)
			{
				value = null;

				foreach(var metadata in _metadatas)
				{
					if(metadata.Commands.TryGet(name, out value))
						return true;
				}

				return false;
			}

			public IEnumerator<ICommand> GetEnumerator()
			{
				foreach(var metadata in _metadatas)
				{
					foreach(var command in metadata.Commands)
						yield return command;
				}
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}
			#endregion
		}
		#endregion
	}
}
