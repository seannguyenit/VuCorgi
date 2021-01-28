using System;

namespace VuBongBongWeb
{
    public class MenuSyncAttribute : Attribute
    {
        public MenuSyncAttribute()
        {
            this.IsBindingWithParent = false;
            this.MenuTaget = "_parent";
            this.IsBlankPage = false;
            this.IsShownOnMenuBar = true;
            this.IsMainPage = false;
            this.IsAllowGuest = false;
        }

        public string CssClass { get; set; }
        public int SyncOrder { get; set; }
        public string Description { get; set; }
        public bool IsBindingWithParent { get; set; }
        public int Level { get; set; }
        public string MenuTaget { get; set; }
        public string ParentAction { get; set; }
        public bool IsBlankPage { get; set; }
        public bool IsShownOnMenuBar { get; set; }
        public bool IsMainPage { get; set; }
        public bool IsAllowGuest { get; set; }
    }
}