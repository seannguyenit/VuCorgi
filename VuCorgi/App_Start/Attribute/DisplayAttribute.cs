using System;

namespace VuBongBongWeb
{
    public class DisplayAttribute : Attribute
    {
        public DisplayAttribute()
        {
            Controller = string.Empty;
            Action = string.Empty;
        }

        public string Controller { get; set; }
        public string Action { get; set; }
    }
}