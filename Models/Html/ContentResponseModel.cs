using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace puppeteer_sharp.Models.Html
{
    /// <summary>
    ///  body margin 8px 适当调整margin的w/h值
    /// </summary>
    public class ContentResponseModel
    {
        private decimal _cWidth;
        private decimal _cHeight;

        /// <summary>
        /// content clientWidth
        /// </summary>
        public decimal CWidth  {
            get {

                return _cWidth + 16;
            }
            set {
                _cWidth = value;
            }
        }

        /// <summary>
        /// content clientHeight
        /// </summary>
        public decimal CHeight {

            get
            {
                return _cHeight + 12;
            }
            set
            {
                _cHeight = value;
            }
        }
    }
}
