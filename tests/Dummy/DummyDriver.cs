﻿using System;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;

using Zongsoft.Data.Common;
using Zongsoft.Data.Common.Expressions;

namespace Zongsoft.Data.Dummy
{
	public class DummyDriver : IDataDriver
	{
		#region 构造函数
		public DummyDriver()
		{
			this.Features = new FeatureCollection();
		}
		#endregion

		#region 公共属性
		public string Name
		{
			get
			{
				return "dummy";
			}
		}

		public FeatureCollection Features
		{
			get;
		}

		public IStatementBuilder Builder
		{
			get
			{
				return DummyStatementBuilder.Default;
			}
		}

		public IStatementScriptor Scriptor
		{
			get
			{
				return DummyStatementScriptor.Default;
			}
		}
		#endregion

		#region 公共方法
		public DbCommand CreateCommand()
		{
			return new OleDbCommand();
		}

		public DbCommand CreateCommand(IStatement statement)
		{
			if(statement == null)
				throw new ArgumentNullException(nameof(statement));

			var script = this.Scriptor.Script(statement);
			var command = new OleDbCommand(script.Text);

			foreach(var parameter in script.Parameters)
			{
				parameter.Attach(command);
			}

			return command;
		}

		public DbCommand CreateCommand(string text, CommandType commandType = CommandType.Text)
		{
			return new OleDbCommand(text)
			{
				CommandType = commandType,
			};
		}

		public DbConnection CreateConnection()
		{
			return new OleDbConnection();
		}

		public DbConnection CreateConnection(string connectionString)
		{
			return new OleDbConnection(connectionString);
		}
		#endregion
	}
}
