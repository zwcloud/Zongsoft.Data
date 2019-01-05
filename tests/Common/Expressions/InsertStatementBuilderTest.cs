﻿using System;
using System.Linq;
using System.Collections.Generic;

using Zongsoft.Data.Common;
using Zongsoft.Data.Common.Expressions;
using Zongsoft.Data.Tests.Models;

using Xunit;

namespace Zongsoft.Data.Tests
{
	public class InsertStatementBuilderTest
	{
		#region 常量定义
		private const string APPLICATION_NAME = "Community";
		#endregion

		#region 成员变量
		private readonly IDataProvider _provider;
		#endregion

		#region 构造函数
		public InsertStatementBuilderTest()
		{
			_provider = Utility.GetProvider(APPLICATION_NAME);
		}
		#endregion

		#region 测试方法
		[Fact]
		public void Test()
		{
			var context = new DataInsertContext(new DataAccess(APPLICATION_NAME),
				"UserProfile", //name
				false, //isMultiple
				GetUserProfile(), //data
				"CorporationId, Gender, User{*}", //schema
				null //state
				);

			var statements = context.Build();
			Assert.NotNull(statements);
			Assert.NotEmpty(statements);

			var command = context.Build(statements.First());
			Assert.NotNull(command);
			Assert.NotNull(command.CommandText);
			Assert.True(command.CommandText.Length > 0);
			Assert.True(command.Parameters.Count > 0);

			System.Diagnostics.Debug.WriteLine(command.CommandText);
		}
		#endregion

		#region 私有方法
		private UserProfile GetUserProfile(uint userId = 1, string name = null, string fullName = null)
		{
			return new UserProfile
			{
				SiteId = 1,
				UserId = userId,
				Gender = Gender.Male,
				CreatedTime = DateTime.Now,
				PhotoPath = "zfs.local:/data/temp/test.jpg",
				MostRecentPostId = 200,
				MostRecentPostTime = new DateTime(2010, 1, 2),
				MostRecentThreadId = 100,
				MostRecentThreadSubject = "This is a subject of the thread.",
				MostRecentThreadTime = new DateTime(2010, 1, 1),
				User = new Security.Membership.User
				{
					UserId = userId,
					Name = name ?? "Popeye",
					FullName = fullName ?? "Popeye Zhong",
					Avatar = ":smile:",
					Email = "zongsoft@qq.com",
					Namespace = "Zongsoft",
					Status = Security.Membership.UserStatus.Active,
				}
			};
		}

		private IEnumerable<UserProfile> GetUserProfiles()
		{
			yield return GetUserProfile(10, "Popey", "Popeye Zhong");
			yield return GetUserProfile(11, "Sophia", "Sophia Zhong");
		}
		#endregion
	}
}