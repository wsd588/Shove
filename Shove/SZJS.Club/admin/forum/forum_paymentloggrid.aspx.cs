using System;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI;

using Discuz.Control;
using Discuz.Forum;
using Discuz.Common;
using Button = Discuz.Control.Button;
using DataGrid = Discuz.Control.DataGrid;
using TextBox = Discuz.Control.TextBox;
using Discuz.Config;
using Discuz.Data;


namespace Discuz.Web.Admin
{
    /// <summary>
    /// ֧����־�б�
    /// </summary>

#if NET1
    public class paymentloggrid : AdminPage
#else
    public partial class paymentloggrid : AdminPage
#endif
    {

#if NET1
        #region �ؼ�����
        protected Discuz.Control.Calendar postdatetimeStart;
        protected Discuz.Control.Calendar postdatetimeEnd;
        protected Discuz.Control.TextBox Username;
        protected Discuz.Control.Button SearchLog;
        protected Discuz.Control.Hint Hint1;
        protected Discuz.Control.DataGrid DataGrid1;
        protected Discuz.Control.TextBox deleteNum;
        protected Discuz.Control.Calendar deleteFrom;
        protected Discuz.Control.Button DelRec;
        #endregion
#endif
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                postdatetimeStart.SelectedDate = DateTime.Now.AddDays(-30);
                postdatetimeEnd.SelectedDate = DateTime.Now;
                BindData();
            }
        }

        public void BindData()
        {
            #region ���ݰ�

            DataGrid1.AllowCustomPaging = true;
            DataGrid1.VirtualItemCount = GetRecordCount();

            if (ViewState["condition"] == null)
            {
                DataGrid1.DataSource = AdminPaymentLogs.LogList(DataGrid1.PageSize, DataGrid1.CurrentPageIndex + 1);
            }
            else
            {
                DataGrid1.DataSource = AdminPaymentLogs.LogList(DataGrid1.PageSize, DataGrid1.CurrentPageIndex + 1, ViewState["condition"].ToString());
            }
            DataGrid1.DataBind();

            #endregion
        }

        private void DelRec_Click(object sender, EventArgs e)
        {
            #region ɾ��ָ����������־��Ϣ

            if (this.CheckCookie())
            {
                string condition = "";
                //switch (Request.Form["deleteMode"])
                //{
                //    case "chkall":
                //        if (DNTRequest.GetString("id") != "")
                //            condition = " [id] IN(" + DNTRequest.GetString("id") + ")";
                //        break;
                //    case "deleteNum":
                //        if (deleteNum.Text != "" && Utils.IsNumeric(deleteNum.Text))
                //            condition = " [id] not in (select top " + deleteNum.Text + " [id] from [" + BaseConfigs.GetTablePrefix + "paymentlog] order by [id] desc)";
                //        break;
                //    case "deleteFrom":
                //        if (deleteFrom.SelectedDate.ToString() != "")
                //            condition = " [buydate]<'" + deleteFrom.SelectedDate.ToString() + "'";
                //        break;
                //}
                condition = DatabaseProvider.GetInstance().DelModeratorManageCondition(Request.Form["deleteMode"].ToString(), DNTRequest.GetString("id").ToString(), deleteNum.Text.ToString(), deleteFrom.SelectedDate.ToString());

                if (condition != "")
                {
                    AdminPaymentLogs.DeleteLog(condition);
                    Response.Redirect("forum_paymentloggrid.aspx");
                }
                else
                {
                    base.RegisterStartupScript( "", "<script>alert('��δѡ���κ�ѡ��������������');window.location.href='forum_paymentloggrid.aspx';</script>");
                }
            }
            #endregion
        }

        private void SearchLog_Click(object sender, EventArgs e)
        {
            #region ��ָ����ѯ����������־��Ϣ

            if (this.CheckCookie())
            {
                string sqlstring = DatabaseProvider.GetInstance().SearchPaymentLog(postdatetimeStart.SelectedDate, postdatetimeEnd.SelectedDate, Username.Text);

                ViewState["condition"] = sqlstring;
                DataGrid1.CurrentPageIndex = 0;
                BindData();
            }

            #endregion
        }

        private int GetRecordCount()
        {
            #region �õ���־��¼��

            if (ViewState["condition"] == null)
            {
                return AdminPaymentLogs.RecordCount();
            }
            else
            {
                return AdminPaymentLogs.RecordCount(ViewState["condition"].ToString());
            }

            #endregion
        }

        protected void DataGrid_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
        {
            DataGrid1.CurrentPageIndex = e.NewPageIndex;
            BindData();
        }

        public void GoToPagerButton_Click(object sender, EventArgs e)
        {
            BindData();
        }

        private void DataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.Cells[8].Text.ToString().Length > 8)
            {
                e.Item.Cells[8].Text = Utils.HtmlEncode(e.Item.Cells[8].Text.Substring(0, 8)) + "��";
            }
        }

        #region Web Form Designer generated code

        protected override void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        private void InitializeComponent()
        {
            this.SearchLog.Click += new EventHandler(this.SearchLog_Click);
            this.DelRec.Click += new EventHandler(this.DelRec_Click);
            this.DataGrid1.GoToPagerButton.Click += new EventHandler(GoToPagerButton_Click);
            this.DataGrid1.ItemDataBound += new DataGridItemEventHandler(this.DataGrid_ItemDataBound);
            this.Load += new EventHandler(this.Page_Load);

            DataGrid1.TableHeaderName = "��ҽ��׼�¼";
            DataGrid1.AllowSorting = false;
            DataGrid1.ColumnSpan = 8;
        }

        #endregion
    }
}