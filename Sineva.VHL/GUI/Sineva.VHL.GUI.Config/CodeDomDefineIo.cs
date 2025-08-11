using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Reflection;
using System.IO;
using Sineva.VHL.Library;
using Sineva.VHL.Library.IO;

namespace Sineva.VHL.GUI.Config
{
	class CodeDomDefineIo
	{
		CodeCompileUnit m_TargetUnit;
		CodeTypeDeclaration m_TargetClass;
		private const string outputFileName = "DefineIo.cs";

		private string m_Path = "";

		public CodeDomDefineIo(string path, string className)
		{
			m_Path = path;
			m_TargetUnit = new CodeCompileUnit();
			CodeNamespace nameSpace = new CodeNamespace("AppDefine");
			nameSpace.Imports.Add(new CodeNamespaceImport("Sineva.VHL.Library"));
			nameSpace.Imports.Add(new CodeNamespaceImport("Sineva.VHL.Library.IO"));

			//			m_TargetClass = new CodeTypeDeclaration("DefineMelIo");
			m_TargetClass = new CodeTypeDeclaration(className);
			m_TargetClass.IsClass = true;
			m_TargetClass.TypeAttributes = System.Reflection.TypeAttributes.Public;

			nameSpace.Types.Add(m_TargetClass);
			m_TargetUnit.Namespaces.Add(nameSpace);
		}


		public void AddInt32Fields(string fieldName, int val)
		{
			CodeMemberField field = new CodeMemberField();
			field.Attributes = MemberAttributes.Private;
			field.Name = fieldName;
			field.Type = new CodeTypeReference(typeof(System.Int32));
			//			field.InitExpression = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression("System.Int32"), val);
			field.InitExpression = new CodePrimitiveExpression(val);

			m_TargetClass.Members.Add(field);
		}

		public void AddSingleTonFields(string class_name)
		{
			CodeMemberField field = new CodeMemberField();
			field.Attributes = MemberAttributes.Public | MemberAttributes.Static;
            field.Type = new CodeTypeReference(class_name);
			//			field.InitExpression = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression("System.Int32"), val);
			field.Name = "Instance";
            field.InitExpression = new CodeObjectCreateExpression(class_name, new CodeExpression[] { });
			//field.InitExpression = new CodePrimitiveExpression(val);

			m_TargetClass.Members.Add(field);

		}

		public void AddIMelDevicesFields(string fieldName)
		{
			/*
			CodeMemberField field = new CodeMemberField();
			field.Attributes = MemberAttributes.Private;
			field.Name = fieldName;
			field.Type = new CodeTypeReference(typeof(IMelDevices));
			//			field.InitExpression = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression("System.Int32"), val);
			field.InitExpression = new CodePrimitiveExpression(null);

			m_TargetClass.Members.Add(field);
			 * */
		}

		public void AddProperties(string name, IoType type, int id, string hexAdd)
		{

			// Declare the read-only Width property.
			CodeMemberProperty property = new CodeMemberProperty();
			property.Attributes =
				MemberAttributes.Public | MemberAttributes.Final;
			property.Name = name;
			property.HasGet = true;
			property.Type = new CodeTypeReference(typeof(IChannelCommand));
			property.Comments.Add(new CodeCommentStatement(
				"\r\n/<summary>" +
				type.ToString() + "/" + hexAdd + "/" + id.ToString() +
				"\r\n/</summary>"));
			//property.GetStatements.Add(new CodeMethodReturnStatement(
			//    new CodeFieldReferenceExpression(
			//    new CodeThisReferenceExpression(), "widthValue")));


			CodeVariableReferenceExpression varExpression = new CodeVariableReferenceExpression();
			switch (type)
			{
				case IoType.DI:
					varExpression.VariableName = "IoManager.DiChannels[" + id.ToString() + "]";
					break;

				case IoType.DO:
					varExpression.VariableName = "IoManager.DoChannels[" + id.ToString() + "]";
					break;

				case IoType.AI:
					varExpression.VariableName = "IoManager.AiChannels[" + id.ToString() + "]";
					break;

				case IoType.AO:
					varExpression.VariableName = "IoManager.AoChannels[" + id.ToString() + "]";
					break;
			}
			property.GetStatements.Add(new CodeMethodReturnStatement(varExpression));
			m_TargetClass.Members.Add(property);

		}

		public void AddConstructor()
		{
			// Declare the constructor
			CodeConstructor constructor = new CodeConstructor();
			constructor.Attributes =
				MemberAttributes.Private | MemberAttributes.Final;

			// Add parameters.
			//constructor.Parameters.Add(new CodeParameterDeclarationExpression(
			//    typeof(IMelDevices), "melDevices"));

			/*
			// Add field initialization logic
			CodeFieldReferenceExpression melReference =
				new CodeFieldReferenceExpression(
				new CodeThisReferenceExpression(), "m_MelDevices");
			constructor.Statements.Add(new CodeAssignStatement(melReference,
				new CodeArgumentReferenceExpression("(Hit.Lib.Ctrl.IMelDevices)Hit.Lib.Ctrl.MelBoardsPool.Instance")));
			*/

			m_TargetClass.Members.Add(constructor);
		}

		public void Write()
		{
			CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
			CodeGeneratorOptions options = new CodeGeneratorOptions();
			options.BracingStyle = "C";

            string path = Path.GetDirectoryName(m_Path);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            using (StreamWriter sourceWriter = new StreamWriter(m_Path))
			{
				provider.GenerateCodeFromCompileUnit(
					m_TargetUnit, sourceWriter, options);
			}
		}


	}
}
