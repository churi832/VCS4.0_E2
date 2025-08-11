/****************************************************************
 * Copyright	: www.sineva.com.cn
 * Version		: V3.0
 * Programmer	: Software Team
 * Issue Date	: 23.01.13 HJYOU
 * Description	: 
 * 
 ****************************************************************/
using System.Collections.Generic;

namespace Sineva.VHL.Library
{
    public delegate void OccurEventMessageHandler(string msg);

    public delegate void DelVoid_Void();
    public delegate void DelVoid_Object(object obj);
    public delegate void DelVoid_ObjectObject(object obj1, object obj2);
    public delegate void DelVoid_String(string a);
    public delegate void DelVoid_StringString(string a, string b);
    public delegate void DelVoid_StringBool(string a, bool b);
    public delegate void DelVoid_Int(int a);
    public delegate void DelVoid_IntInt(int a, int b);
    public delegate void DelVoid_IntBool(int a, bool b);
    public delegate void DelVoid_Bool(bool a);
    public delegate void DelVoid_BoolBool(bool a, bool b);
    public delegate bool DelBool_Void();

    public delegate void DelVoid_OperateMode(OperateMode opMode);
    public delegate void DelVoid_EqpRunMode(EqpRunMode runMode);
    public delegate void DelVoid_OperatorCall(OperatorCallKind kind, string message);
    public delegate void DelVoid_OperatorCallCommand(string message, string[] commands, bool show);

    public delegate void DelSensorPv_IntInt(int Id, int val);
    public delegate void DelSensorPv_IntDouble(int Id, double val);
    public delegate void DelSensorPv_IntString(int Id, string val);
    public delegate void DelSensorPv_IntList(int Id, List<double> val);

    public delegate void DelVoid_RecipeCommandConfirm(int ackCode, string recipeId);
    public delegate void DelVoid_PermissionRequest(int key, string[] values);

    public delegate void DelVoid_DataVersionChanged(GeneralInformationItemName name, string version);
    public delegate void DelVoid_AutoTeachingVisionResult(double dx, double dy, double dt);

    public delegate void DelVoid_UpdatePathData(object obj, bool update);
}
