﻿using System;
using System.Linq;
using System.Collections.Generic;

using Zongsoft.Data.Common;
using Zongsoft.Data.Common.Expressions;

using Xunit;

namespace Zongsoft.Data.Tests
{
	public class DeleteStatementBuilderTest
	{
		#region 常量定义
		private const string APPLICATION_NAME = "Community";
		#endregion

		#region 成员变量
		private readonly IDataProvider _provider;
		#endregion

		#region 构造函数
		public DeleteStatementBuilderTest()
		{
			_provider = Utility.GetProvider(APPLICATION_NAME);
		}
		#endregion

		#region 测试方法
		[Fact]
		public void Test()
		{
			var context = new DataDeleteContext(new DataAccess(APPLICATION_NAME),
				"Asset", //name
				Condition.Equal("PrincipalId", 100), // Condition.Equal("Principal.User.Name", "Popeye") //condition
				null //"Principal{Department}" //schema
				);

			var source = _provider.Multiplexer.GetSource(context);
			Assert.NotNull(source);

			var statements = source.Driver.Builder.Build(context, source).ToArray();
			Assert.NotNull(statements);
			Assert.NotEmpty(statements);

			var command = source.Driver.CreateCommand(statements[0]);
			Assert.NotNull(command);
			Assert.NotNull(command.CommandText);
			Assert.True(command.CommandText.Length > 0);

			System.Diagnostics.Debug.WriteLine(command.CommandText);
		}
		#endregion
	}
}
