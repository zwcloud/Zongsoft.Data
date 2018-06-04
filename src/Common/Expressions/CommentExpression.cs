﻿using System;

namespace Zongsoft.Data.Common.Expressions
{
	public class CommentExpression : Expression
	{
		#region 构造函数
		public CommentExpression(string text)
		{
			this.Text = text;
		}
		#endregion

		#region 公共属性
		public string Text
		{
			get;
			set;
		}
		#endregion

		#region 重写方法
		public override string ToString()
		{
			if(string.IsNullOrEmpty(this.Text))
				return string.Empty;

			return "/* " + this.Text + " */";
		}
		#endregion
	}
}
