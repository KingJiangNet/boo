#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//
// As a special exception, if you link this library with other files to
// produce an executable, this library does not by itself cause the
// resulting executable to be covered by the GNU General Public License.
// This exception does not however invalidate any other reasons why the
// executable file might be covered by the GNU General Public License.
//
// Contact Information
//
// mailto:rbo@acm.org
#endregion

using System;
using Boo.Ast;
using Boo.Ast.Compilation;

namespace Boo.Ast.Compilation.Pipeline
{
	public class AstNormalizationStep : AbstractTransformerCompilerStep
	{
		public override void Run()
		{
			foreach (Module module in CompileUnit.Modules)
			{
				Switch(module.Globals.Statements);
				Switch(module.Members);
			}
		}
		
		public override void LeaveClassDefinition(ClassDefinition node, ref ClassDefinition resultingNode)
		{
			if (!node.IsVisibilitySet)
			{
				node.Modifiers |= TypeMemberModifiers.Public;
			}
		}
		
		public override void LeaveField(Field node, ref Field resultingNode)
		{
			if (!node.IsVisibilitySet)
			{
				node.Modifiers |= TypeMemberModifiers.Protected;
			}
		}
		
		public override void LeaveProperty(Property node, ref Property resultingNode)
		{
			if (!node.IsVisibilitySet)
			{
				node.Modifiers |= TypeMemberModifiers.Public;
			}
		}
		
		public override void LeaveMethod(Method node, ref Method resultingNode)
		{
			if (!node.IsVisibilitySet)
			{
				node.Modifiers |= TypeMemberModifiers.Public;
			}
		}
		
		public override void LeaveConstructor(Constructor node, ref Constructor resultingNode)
		{
			if (!node.IsVisibilitySet)
			{
				node.Modifiers |= TypeMemberModifiers.Public;
			}
		}		
		
		public override void LeaveExpressionStatement(ExpressionStatement node, ref Statement resultingNode)
		{
			LeaveStatement(node, ref resultingNode);
		}
		
		public override void LeaveRaiseStatement(RaiseStatement node, ref Statement resultingNode)
		{
			LeaveStatement(node, ref resultingNode);
		}
		
		public override void LeaveReturnStatement(ReturnStatement node, ref Statement resultingNode)
		{
			LeaveStatement(node, ref resultingNode);
		}
		
		public void LeaveStatement(Statement node, ref Statement resultingNode)
		{
			if (null != node.Modifier)
			{
				switch (node.Modifier.Type)
				{
					case StatementModifierType.If:
					{	
						IfStatement stmt = new IfStatement();
						stmt.LexicalInfo = node.Modifier.LexicalInfo;
						stmt.Expression = node.Modifier.Condition;
						stmt.TrueBlock = new Block();						
						stmt.TrueBlock.Statements.Add(node);						
						node.Modifier = null;
						
						resultingNode = stmt;
						
						break;
					}
						
					default:
					{							
						Errors.NotImplemented(node, "only if supported");
						break;
					}
				}
			}
		}

	}
}
