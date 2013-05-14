using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ZTn.BNet.D3.Items;

namespace ZTnDroid.D3Calculator.Adapters
{
    public class AttributePercentListItem : AttributeListItem
    {
        public AttributePercentListItem(int id, long value) :
            this(D3Calc.Context.Resources.GetString(id), value)
        {
        }

        public AttributePercentListItem(String name, long value)
            : base(name, String.Format("{0} %", value))
        {
        }

        public AttributePercentListItem(int id, ItemValueRange value) :
            this(D3Calc.Context.Resources.GetString(id), value)
        {
        }

        public AttributePercentListItem(String name, ItemValueRange value) :
            base(name, String.Format("{0:0.00} %", 100 * value.min))
        {
        }

        public AttributePercentListItem(int id, double value) :
            this(D3Calc.Context.Resources.GetString(id), value)
        {
        }

        public AttributePercentListItem(String name, double value)
            : base(name, String.Format("{0:0.00} %", 100 * value))
        {
        }
    }
}