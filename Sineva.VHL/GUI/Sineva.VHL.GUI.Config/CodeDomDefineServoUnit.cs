using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Reflection;
using System.IO;
using Sineva.VHL.Device;
using Sineva.VHL.Device.ServoControl;

namespace Sineva.VHL.GUI.Config
{
    class CodeDomDefineServoUnit
    {
        CodeCompileUnit m_TargetUnit;
        CodeTypeDeclaration m_TargetClass;
        private const string outputFileName = "DefineDevServo";

        private string m_Path = "";

        public CodeDomDefineServoUnit(string path, string className)
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

        public void AddConstructor()
        {
            // Declare the constructor
            CodeConstructor constructor = new CodeConstructor();
            constructor.Attributes =
                MemberAttributes.Private | MemberAttributes.Final;

            m_TargetClass.Members.Add(constructor);
        }

        public void AddSingleTonFields(string class_name)
        {
            CodeMemberField field = new CodeMemberField();
            field.Attributes = MemberAttributes.Public | MemberAttributes.Static;
            field.Type = new CodeTypeReference(class_name);
            field.Name = "Instance";
            field.InitExpression = new CodeObjectCreateExpression(class_name, new CodeExpression[] { });
            m_TargetClass.Members.Add(field);
        }

        public void AddServoUnitFields(string name, int blockid)
        {
            CodeMemberField field = new CodeMemberField();
            field.Attributes = MemberAttributes.Public | MemberAttributes.Static;
            field.Type = new CodeTypeReference(typeof(_DevServoUnit));
            field.Name = name;
            string block = string.Format("DevServoUnits[{0}]", blockid);
            field.InitExpression = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression("ServoControlManager.Instance"), block);
            m_TargetClass.Members.Add(field);
        }

        public void AddAxisFields(string name, int axisid, int blockid)
        {
            CodeMemberField field = new CodeMemberField();
            field.Attributes = MemberAttributes.Public | MemberAttributes.Static;
            //            field.Type = new CodeTypeReference(typeof(IAxisCommand));
            field.Type = new CodeTypeReference(typeof(_DevAxis));
            field.Name = name;
            //            string block = string.Format("ServoUnits[{0}].Axes[{1}] as IAxisCommand", blockid, axisid);
            string block = string.Format("DevServoUnits[{0}].DevAxes[{1}]", blockid, axisid);
            field.InitExpression = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression("ServoControlManager.Instance"), block);
            m_TargetClass.Members.Add(field);
        }
        public void AddFields(string name, int no)
        {
            CodeMemberField field = new CodeMemberField();
            field.Attributes = MemberAttributes.Public | MemberAttributes.Static;
            field.Type = new CodeTypeReference(typeof(ushort));
            field.Name = name;
            field.InitExpression = new CodePrimitiveExpression(no);
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
