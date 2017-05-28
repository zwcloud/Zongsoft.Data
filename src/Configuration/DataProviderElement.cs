﻿/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2015-2017 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Data.Configuration
{
	public class DataProviderElement : OptionConfigurationElement
	{
		#region 常量定义
		private const string XML_NAME_ATTRIBUTE = "name";
		private const string XML_DRIVERNAME_ATTRIBUTE = "driverName";
		private const string XML_ACCESSMODE_ATTRIBUTE = "accessMode";
		private const string XML_CONNECTIONSTRING_ATTRIBUTE = "connectionString";
		#endregion

		#region 公共属性
		[OptionConfigurationProperty(XML_NAME_ATTRIBUTE, Behavior = OptionConfigurationPropertyBehavior.IsKey)]
		public string Name
		{
			get
			{
				return (string)this[XML_NAME_ATTRIBUTE];
			}
			set
			{
				this[XML_NAME_ATTRIBUTE] = value;
			}
		}

		[OptionConfigurationProperty(XML_DRIVERNAME_ATTRIBUTE, Behavior = OptionConfigurationPropertyBehavior.IsRequired)]
		public string DriverName
		{
			get
			{
				return (string)this[XML_DRIVERNAME_ATTRIBUTE];
			}
			set
			{
				this[XML_DRIVERNAME_ATTRIBUTE] = value;
			}
		}

		[OptionConfigurationProperty(XML_ACCESSMODE_ATTRIBUTE, DefaultValue = DataAccessMode.ReadWrite)]
		public DataAccessMode AccessMode
		{
			get
			{
				return (DataAccessMode)this[XML_ACCESSMODE_ATTRIBUTE];
			}
			set
			{
				this[XML_ACCESSMODE_ATTRIBUTE] = value;
			}
		}

		[OptionConfigurationProperty(XML_CONNECTIONSTRING_ATTRIBUTE, Behavior = OptionConfigurationPropertyBehavior.IsRequired)]
		public string ConnectionString
		{
			get
			{
				return (string)this[XML_CONNECTIONSTRING_ATTRIBUTE];
			}
			set
			{
				this[XML_CONNECTIONSTRING_ATTRIBUTE] = value;
			}
		}

		[OptionConfigurationProperty("", ElementName = "container")]
		public DataProviderContainerElementCollection Containers
		{
			get
			{
				return (DataProviderContainerElementCollection)this[""];
			}
		}
		#endregion
	}
}
