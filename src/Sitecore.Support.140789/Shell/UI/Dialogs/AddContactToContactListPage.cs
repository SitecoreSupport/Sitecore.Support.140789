
using ComponentArt.Web.UI;
using Sitecore.Configuration;
using Sitecore.Diagnostics;
using Sitecore.Form.Core.Utility;
using Sitecore.Forms.Shell.UI.Controls;
using Sitecore.Forms.Shell.UI.Dialogs;
using Sitecore.ListManagement;
using Sitecore.ListManagement.ContentSearch.Model;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.WFFM.Abstractions.Dependencies;
using Sitecore.WFFM.Abstractions.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.UI.WebControls;
using Sitecore.Form.Web.UI.Controls;
using System.Reflection;

namespace Sitecore.Support.Forms.Shell.UI.Dialogs
{
    public class AddContactToContactListPage : EditorBase
    {
        // Fields
        private IQueryable<ContactList> _contactList;
        protected ComboBox ConditionCombobox;
        protected ControlledChecklist ConditionList;
        private Sitecore.Web.UI.HtmlControls.Literal ConditionText;
        private Sitecore.Web.UI.HtmlControls.Literal ContactListHeader;
        protected CheckBoxList ContactListsBox;
        private const string ContactsListKey = "ContactsLists";
        private readonly IResourceManager resourceManager;

        // Methods
        public AddContactToContactListPage() : this(DependenciesManager.ResourceManager)
        {
        }

        public AddContactToContactListPage(IResourceManager resourceManager)
        {
            Assert.IsNotNull(resourceManager, "resourceManager");
            this.resourceManager = resourceManager;
        }

        protected override void Localize()
        {
            base.Header = this.resourceManager.Localize("ADD_CONTACT_TO_CONTACT_LIST");
            base.Text = this.resourceManager.Localize("SELECT_THE_CONTACT_LIST");
            this.ContactListHeader.Text = this.resourceManager.Localize("CONTACT_LISTS");
            this.ConditionText.Text = this.resourceManager.Localize("CONDITION");
        }

        protected override void OnInit(EventArgs e)
        {
            this.ConditionList.AddRange(ConditionalStatementUtil.GetConditionalItems(base.CurrentForm));
            this.ConditionList.SelectRange(base.GetValueByKey("ExecuteWhen", "Always"));
            this.ConditionCombobox.Text = this.ConditionList.SelectedTitle;
            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!Sitecore.Context.ClientPage.IsEvent)
            {
                string valueByKey = base.GetValueByKey("ContactsLists");
                List<string> list = (valueByKey == null) ? new List<string>() : valueByKey.Split(new char[] { ',' }).ToList<string>();
                foreach (ContactList list2 in this.ContactsLists)
                {
                    System.Web.UI.WebControls.ListItem item = new System.Web.UI.WebControls.ListItem
                    {
                        Value = list2.Id,
                        Text = list2.Name,
                        Selected = list.Contains(list2.Id)
                    };
                    this.ContactListsBox.Items.Add(item);
                }
            }
        }

        protected override void SaveValues()
        {
            base.SaveValues();
            base.SetValue("ContactsLists", string.Join(",", (from x in this.ContactListsBox.Items.Where(x => x.Selected) select x.Value).ToList<string>()));
            base.SetValue("ExecuteWhen", string.Join("|", this.ConditionList.GetManagedSelectedValues().ToArray<string>()));
        }

        // Properties
        public IQueryable<ContactList> ContactsLists
        {
            get
            {
                ListManager<ContactList, ContactData> manager = Factory.CreateObject("contactListManager", false) as ListManager<ContactList, ContactData>;
                if (manager == null)
                {
                    return null;
                }
                if (this._contactList == null)
                {
                    var res = new List<ContactList>();
                    var lists = manager.GetAll(null, true);
                    foreach (var list in lists)
                    {
                        if (res.Find(x => x.Id == list.Id) == null)
                        {
                            res.Add(list);
                        }
                    }
                    this._contactList = res.AsQueryable();
                }
                return this._contactList;
            }
        }


        public string RestrictedFieldTypes =>
            Sitecore.Web.WebUtil.GetQueryString("RestrictedFieldTypes", "{1F09D460-200C-4C94-9673-488667FF75D1}|{1AD5CA6E-8A92-49F0-889C-D082F2849FBD}|{7FB270BE-FEFC-49C3-8CB4-947878C099E5}");


    }


}
