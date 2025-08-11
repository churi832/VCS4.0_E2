using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sineva.VHL.Data.Setup
{
    [Serializable]
    public partial class SetupTeachingOffset
    {
        #region Fields
        private double m_LeftBCROffset = 0.0f;
        private double m_RightBCROffset = 0.0f;

        private double m_RotateOffset = 0.0f;
        private double m_RotateOffsetPort = 0.0f;
        private double m_LeftSlideOffset = 0.0f;
        private double m_RightSlideOffset = 0.0f;
        private double m_HoistLeftOffset = 0.0f;
        private double m_HoistRightOffset = 0.0f;
        private double m_HoistOffsetPort = 0.0f;
        private double m_HoistFoupStateOffset = 0.0f;
        private double m_BufferloadLeftSlideOffset = 0.0f;
        private double m_BufferloadRightSlideOffset = 0.0f;
        private double m_PortSlideOffset = 0.0f;
        private double m_IOPortSlideOffset = 0.0f;

        private double m_HoistOffsetUnderPort = 0.0f;
        #endregion

        #region Properties
        [Category("BCR"), DisplayName("LeftBCROffset")]
        public double  LeftBCROffset { get => m_LeftBCROffset; set => m_LeftBCROffset = value; }
        [Category("BCR"), DisplayName("RightBCROffset")]
        public double RightBCROffset { get => m_RightBCROffset; set => m_RightBCROffset = value; }
        [Category("Rotate"), DisplayName("RotateOffset")]
        public double RotateOffset { get => m_RotateOffset; set => m_RotateOffset = value; }
        [Category("Rotate"), DisplayName("RotateOffsetPort")]
        public double RotateOffsetPort { get => m_RotateOffsetPort; set => m_RotateOffsetPort = value; }
        [Category("Slide"), DisplayName("LeftSlideOffset")]
        public double LeftSlideOffset { get => m_LeftSlideOffset; set => m_LeftSlideOffset = value; }
        [Category("Slide"), DisplayName("RightSlideOffset")]
        public double RightSlideOffset { get => m_RightSlideOffset; set => m_RightSlideOffset = value; }
        [Category("Hoist"), DisplayName("HoistLeftOffset")]
        public double HoistLeftOffset { get => m_HoistLeftOffset; set => m_HoistLeftOffset = value; }
        [Category("Hoist"), DisplayName("HoistRightOffset")]
        public double HoistRightOffset { get => m_HoistRightOffset; set => m_HoistRightOffset = value; }
        [Category("Hoist"), DisplayName("HoistOffsetPort")]
        public double HoistOffsetPort { get => m_HoistOffsetPort; set => m_HoistOffsetPort = value; }
        [Category("Hoist"), DisplayName("HoistFoupStateOffset")]
        public double HoistFoupStateOffset { get => m_HoistFoupStateOffset; set => m_HoistFoupStateOffset = value; }
        [Category("Slide"), DisplayName("BufferloadLeftSlideOffset")]
        public double BufferloadLeftSlideOffset { get => m_BufferloadLeftSlideOffset; set => m_BufferloadLeftSlideOffset = value; }
        [Category("Slide"), DisplayName("BufferloadRightSlideOffset")]
        public double BufferloadRightSlideOffset { get => m_BufferloadRightSlideOffset; set => m_BufferloadRightSlideOffset = value; }
        [Category("Slide"), DisplayName("PortSlideOffset")]
        public double PortSlideOffset { get => m_PortSlideOffset; set => m_PortSlideOffset = value; }
        [Category("Slide"), DisplayName("IOPortSlideOffset")]
        public double IOPortSlideOffset { get => m_IOPortSlideOffset; set => m_IOPortSlideOffset = value; }
        [Category("Hoist"), DisplayName("HoistOffsetUnderPort")]
        public double HoistOffsetUnderPort { get => m_HoistOffsetUnderPort; set => m_HoistOffsetUnderPort = value; }
        #endregion

        #region Constructor
        public SetupTeachingOffset()
        {

        }
        #endregion

        #region Methods
        #endregion

    }
}
