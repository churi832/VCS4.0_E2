/****************************************************************
 * Copyright	: www.sineva.com.cn
 * Version		: V1.0
 * Programmer	: Software Group
 * Issue Date	: 23.02.20
 * Description	: 
 * 
 ****************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;

// 비 정형 움직임이 있을 경우 사용
// Circle or Arc 동작 구현
namespace Sineva.VHL.Library.Servo
{
    public enum enSegmentType
    {
        NONE,
        ARC,
        CIRCLE,
        LINE,
    }

    [Editor(typeof(UIEditorPropertyEdit), typeof(UITypeEditor))]
    [Serializable]
    public class SegmentCommand
    {
        public enSegmentType SegmentType { get; set; }
        public List<_Axis> SubAxes { get; set; }
        public XyPosition StartPos { get; set; }
        public XyPosition EndPos { get; set; }
        public XyPosition MoveVel { get; set; }
        public XyPosition CircleCenterPos { get; set; }
        public double radian { get; set; } // 각도 설정
        public enAxisDir MoveDirection { get; set; } // 이동 방향, clock_wise, counter_clock_wise
        public SegmentCommand()
        {

        }
    }
}
