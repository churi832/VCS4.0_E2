using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Reflection;
using System.IO;

namespace Sineva.VHL.GUI.Config
{
	public class CodeDomDefineAlarm
	{
		CodeCompileUnit m_TargetUnit;
		CodeTypeDeclaration m_TargetClass;
		private const string outputFileName = "DefineAlarm.cs";

		private string m_Path = "";

		public CodeDomDefineAlarm(string path, string className)
		{
			m_Path = path;
			m_TargetUnit = new CodeCompileUnit();
			CodeNamespace nameSpace = new CodeNamespace("AppDefine");
			m_TargetClass = new CodeTypeDeclaration(className);
			m_TargetClass.IsClass = true;
			m_TargetClass.TypeAttributes = System.Reflection.TypeAttributes.Public;

			nameSpace.Types.Add(m_TargetClass);
			m_TargetUnit.Namespaces.Add(nameSpace);
		}

		public void AddSingleTonFields()
		{
			CodeMemberField field = new CodeMemberField();
			field.Attributes = MemberAttributes.Public | MemberAttributes.Static;
			field.Type = new CodeTypeReference("DefineAlarm");
			//			field.InitExpression = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression("System.Int32"), val);
			field.Name = "Instance";
			field.InitExpression = new CodeObjectCreateExpression("DefineAlarm", new CodeExpression[] { });
			//field.InitExpression = new CodePrimitiveExpression(val);

			m_TargetClass.Members.Add(field);
		}

		public void AddInt32Fields(string fieldName, int val)
		{
			CodeMemberField field = new CodeMemberField();
			field.Attributes = MemberAttributes.Public | MemberAttributes.Static;
            char[] names = fieldName.ToArray();
            for (int i = 0; i < names.Length; i++)
            {
                if (names[i] < '0') //특수 문자일 경우
                {
                    fieldName = fieldName.Replace(names[i], '_');
                }
            }
            //fieldName = fieldName.Replace(' ', '_');
			field.Name = fieldName;
			field.Type = new CodeTypeReference(typeof(System.Int32));
			//			field.InitExpression = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression("System.Int32"), val);
			field.InitExpression = new CodePrimitiveExpression(val);

			m_TargetClass.Members.Add(field);
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
