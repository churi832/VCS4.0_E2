using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;

namespace Sineva.VHL.IF.WebApi.Controllers
{
    public class ReturnCode
    {
        public bool succeed { get; set; } = false;
        public int status { get; set; } = 210;
        public string msg { get; set; } = string.Empty;
        public object data { get; set; }
    }
    public static class ClassConvertor
    {
        public static void Prop2Field<T>(object inp, ref T outp, Dictionary<string, string> m = null)
        {
            try
            {
                List<PropertyInfo> properties = inp.GetType().GetProperties().ToList();
                List<FieldInfo> fields = outp.GetType().GetFields().ToList();
                if (m != null)
                {
                    foreach (var k in m.Keys)
                    {
                        var props = properties.Where((o) => { if (string.Equals(o.Name, k, StringComparison.InvariantCultureIgnoreCase)) return true; else { return false; } });
                        if (props.Count() > 0)
                        {
                            var flds = fields.Where((o) => { if (string.Equals(o.Name, m[k], StringComparison.InvariantCultureIgnoreCase)) return true; else { return false; } });
                            if (flds.Count() > 0)
                            {
                                if (props.First().PropertyType == flds.First().FieldType)
                                {
                                    flds.First().SetValue(outp, props.First().GetValue(inp));
                                    fields.RemoveAll((o) => { return string.Equals(o.Name, m[k], StringComparison.InvariantCultureIgnoreCase); });
                                }
                                else if (props.First().PropertyType.IsEnum)
                                {
                                    if (flds.First().FieldType == typeof(int))
                                    {
                                        flds.First().SetValue(outp, (int)props.First().GetValue(inp));
                                    }
                                    else if (flds.First().FieldType == typeof(string))
                                    {
                                        flds.First().SetValue(outp, ((int)props.First().GetValue(inp)).ToString());
                                    }
                                    fields.RemoveAll((o) => { return string.Equals(o.Name, m[k], StringComparison.InvariantCultureIgnoreCase); });
                                }
                                else if (props.First().PropertyType == typeof(double))
                                {
                                    if (flds.First().FieldType == typeof(int))
                                    {
                                        var ttt = (double)props.First().GetValue(inp);
                                        int ttl = (int)ttt;
                                        flds.First().SetValue(outp, ttl);
                                        //flds.First().SetValue(outp, (int)props.First().GetValue(inp));
                                    }
                                    fields.RemoveAll((o) => { return string.Equals(o.Name, m[k], StringComparison.InvariantCultureIgnoreCase); });
                                }
                                //else if (flds.First().FieldType == typeof(string))
                                //{
                                //    try
                                //    {

                                //        flds.First().SetValue(outp, props.First().GetValue(inp).ToString());
                                //    }
                                //    catch
                                //    { }

                                //}
                                properties.RemoveAll((o) => { return string.Equals(o.Name, k, StringComparison.InvariantCultureIgnoreCase); });
                            }
                        }
                    }
                }
                foreach (var field in fields)
                {
                    var props = properties.Where((o) => { if (string.Equals(o.Name, field.Name, StringComparison.InvariantCultureIgnoreCase)) return true; else { return false; } });
                    if (props.Count() > 0)
                    {
                        if (props.First().PropertyType == field.FieldType)
                        {
                            field.SetValue(outp, props.First().GetValue(inp));
                        }
                        else if (props.First().PropertyType.IsEnum)
                        {
                            if (field.FieldType == typeof(int))
                            {
                                field.SetValue(outp, (int)props.First().GetValue(inp));
                            }
                            else if (field.FieldType == typeof(string))
                            {
                                field.SetValue(outp, ((int)props.First().GetValue(inp)).ToString());
                            }
                        }
                        else if (props.First().PropertyType == typeof(double))
                        {
                            if (field.FieldType == typeof(int))
                            {
                                var ttt = (double)props.First().GetValue(inp);
                                int ttl = (int)ttt;
                                field.SetValue(outp, ttl);
                                //field.SetValue(outp, (int)props.First().GetValue(inp));
                            }
                        }
                        else if (field.FieldType == typeof(string))
                        {
                            try
                            {
                                field.SetValue(outp, props.First().GetValue(inp).ToString());
                            }
                            catch
                            { }

                        }
                    }
                }
            }
            catch (Exception ex)
            { }
        }
        public static void Field2Prop<T>(object inp, ref T outp, Dictionary<string, string> m = null)
        {
            try
            {
                List<FieldInfo> fields = inp.GetType().GetFields().ToList();
                List<PropertyInfo> properties = outp.GetType().GetProperties().ToList();
                //if (m != null)
                //{
                //    foreach (var k in m.Keys)
                //    {
                //        var props = properties.Where((o) => { if (string.Equals(o.Name, k, StringComparison.InvariantCultureIgnoreCase)) return true; else { return false; } });
                //        if (props.Count() > 0)
                //        {
                //            var flds = fields.Where((o) => { if (string.Equals(o.Name, m[k], StringComparison.InvariantCultureIgnoreCase)) return true; else { return false; } });
                //            if (flds.Count() > 0)
                //            {
                //                if (props.First().PropertyType == flds.First().FieldType)
                //                {
                //                    flds.First().SetValue(outp, props.First().GetValue(inp));
                //                    fields.RemoveAll((o) => { return string.Equals(o.Name, m[k], StringComparison.InvariantCultureIgnoreCase); });
                //                }
                //                else if (props.First().PropertyType.IsEnum)
                //                {
                //                    if (flds.First().FieldType == typeof(int))
                //                    {
                //                        flds.First().SetValue(outp, (int)props.First().GetValue(inp));
                //                    }
                //                    else if (flds.First().FieldType == typeof(string))
                //                    {
                //                        flds.First().SetValue(outp, ((int)props.First().GetValue(inp)).ToString());
                //                    }
                //                    fields.RemoveAll((o) => { return string.Equals(o.Name, m[k], StringComparison.InvariantCultureIgnoreCase); });
                //                }
                //                else if (props.First().PropertyType == typeof(double))
                //                {
                //                    if (flds.First().FieldType == typeof(int))
                //                    {
                //                        var ttt = (double)props.First().GetValue(inp);
                //                        int ttl = (int)ttt;
                //                        flds.First().SetValue(outp, ttl);
                //                        //flds.First().SetValue(outp, (int)props.First().GetValue(inp));
                //                    }
                //                    fields.RemoveAll((o) => { return string.Equals(o.Name, m[k], StringComparison.InvariantCultureIgnoreCase); });
                //                }
                //                else if (flds.First().FieldType == typeof(string))
                //                {
                //                    try
                //                    {

                //                        flds.First().SetValue(outp, props.First().GetValue(inp).ToString());
                //                    }
                //                    catch
                //                    { }

                //                }
                //                properties.RemoveAll((o) => { return string.Equals(o.Name, k, StringComparison.InvariantCultureIgnoreCase); });
                //            }
                //        }
                //    }
                //}
                foreach (var field in fields)
                {
                    var props = properties.Where((o) => { if (string.Equals(o.Name, field.Name, StringComparison.InvariantCultureIgnoreCase)) return true; else { return false; } });
                    if (props.Count() > 0)
                    {
                        if (props.First().PropertyType == field.FieldType)
                        {
                            props.First().SetValue(outp, field.GetValue(inp));
                        }
                        else if (props.First().PropertyType.IsEnum)
                        {
                            if (field.FieldType == typeof(int))
                            {
                                var ttttt = field.GetValue(inp);
                                props.First().SetValue(outp, field.GetValue(inp));
                            }
                            else if (field.FieldType == typeof(string))
                            {
                                //field.SetValue(outp, ((int)props.First().GetValue(inp)).ToString());
                                props.First().SetValue(outp, Enum.Parse(props.First().PropertyType, field.GetValue(inp).ToString()));
                            }
                        }
                        else if (props.First().PropertyType == typeof(double))
                        {
                            if (field.FieldType == typeof(int))
                            {
                                //var ttt = (double)props.First().GetValue(inp);
                                //int ttl = (int)ttt;
                                //field.SetValue(outp, ttl);
                                ////field.SetValue(outp, (int)props.First().GetValue(inp));
                                props.First().SetValue(outp, field.GetValue(inp));
                            }
                        }
                        else if (field.FieldType == typeof(string))
                        {
                            try
                            {
                                //field.SetValue(outp, props.First().GetValue(inp).ToString());
                                props.First().SetValue(outp, field.GetValue(inp));
                            }
                            catch
                            { }

                        }
                    }
                }
            }
            catch (Exception ex)
            { }
        }
    }
}