using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using System.Web.UI;


using Discuz.Common;
using Discuz.Forum;
using Discuz.Config;
using Discuz.Data;

namespace Discuz.Web.Admin
{
    /// <summary>
    /// ��������
    /// </summary>
    
    public partial class scoreset : AdminPage
    {
        public DataSet dsSrc = new DataSet();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

                #region ���ػ������ù�ʽ�����������Ϣ

                dsSrc.ReadXml(Server.MapPath("../../config/scoreset.config"));
                formula.Text = dsSrc.Tables["formula"].Rows[0]["formulacontext"].ToString();
                if (dsSrc.Tables["formula"].Rows[0]["creditstrans"].ToString() == "")
                {
                    creditstrans.SelectedIndex = 0;
                }
                else
                {
                    creditstrans.SelectedValue = dsSrc.Tables["formula"].Rows[0]["creditstrans"].ToString();
                }
                creditstax.Text = dsSrc.Tables["formula"].Rows[0]["creditstax"].ToString();
                transfermincredits.Text = dsSrc.Tables["formula"].Rows[0]["transfermincredits"].ToString();
                exchangemincredits.Text = dsSrc.Tables["formula"].Rows[0]["exchangemincredits"].ToString();
                maxincperthread.Text = dsSrc.Tables["formula"].Rows[0]["maxincperthread"].ToString();
                maxchargespan.Text = dsSrc.Tables["formula"].Rows[0]["maxchargespan"].ToString();
                #endregion

                BindData();
            }
        }


        public void BindData()
        {
            #region ���������б����ݼ���

            DataGrid1.AllowCustomPaging = false;
            DataGrid1.TableHeaderName = "<img src='../images/icons/icon31.jpg'>��������";
            DataGrid1.DataSource = AbsScoreSet(dsSrc.Tables[0]);
            DataGrid1.DataBind();

            #endregion
        }

        private void DataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            #region �������ݰ󶨵ĳ���

            switch (e.Item.ItemType)
            {
                case ListItemType.Item:
                    break;
                case ListItemType.AlternatingItem:
                    break;
                case ListItemType.Header:
                    e.Item.Cells[0].ColumnSpan = 1; //�ϲ���Ԫ�� 
                    e.Item.Cells[1].Visible = false;
                    break;
                case ListItemType.EditItem:
                    {
                        for (int i = 0; i < DataGrid1.Columns.Count; i++) //ֻ�������༭���� 
                        {
                            if (e.Item.ItemType == ListItemType.EditItem)
                            {
                                if (i >= 3)
                                {
                                    System.Web.UI.WebControls.TextBox txt = (System.Web.UI.WebControls.TextBox)e.Item.Cells[i].Controls[0];
                                    txt.Width = 60;
                                }
                            }
                        }
                        break;
                    }
                default:
                    break;
            }

            #endregion
        }

        public DataTable AbsScoreSet(DataTable dt)
        {
            #region �Ա������ݽ��о���ֵת��

            foreach (DataRow dr in dt.Rows)
            {
                if (Convert.ToInt16(dr["id"].ToString()) > 2)
                {
                    //���Ǽ����ֲ���ʱ��ϵͳ�Զ�ȡ������������Ҫ��Ϊ�˴�config�ļ���ȡ����ֵ�����������ɲ���ǰ̨��������
                    if ((dr["id"].ToString() == "11") || (dr["id"].ToString() == "12") || (dr["id"].ToString() == "13"))
                    {
                        dr["extcredits1"] = (-1) * Double.Parse(dr["extcredits1"].ToString());
                        dr["extcredits2"] = (-1) * Double.Parse(dr["extcredits2"].ToString());
                        dr["extcredits3"] = (-1) * Double.Parse(dr["extcredits3"].ToString());
                        dr["extcredits4"] = (-1) * Double.Parse(dr["extcredits4"].ToString());
                        dr["extcredits5"] = (-1) * Double.Parse(dr["extcredits5"].ToString());
                        dr["extcredits6"] = (-1) * Double.Parse(dr["extcredits6"].ToString());
                        dr["extcredits7"] = (-1) * Double.Parse(dr["extcredits7"].ToString());
                        dr["extcredits8"] = (-1) * Double.Parse(dr["extcredits8"].ToString());
                    }
                }
            }
            return dt;

            #endregion
        }

        public void SetUserGroupRaterange(int scoreid)
        {
            #region �����û�����ַ�Χ

            bool isupdate = true;

            foreach (DataRow dr in Scoresets.GetScoreSet().Rows)
            {
                if ((dr["id"].ToString() != "1") && (dr["id"].ToString() != "2"))
                {
                    if (dr[scoreid + 1].ToString().Trim() != "0")
                    {
                        isupdate = false;
                        break;
                    }
                }
            }

            if (isupdate)
            {
                foreach (DataRow dr in DatabaseProvider.GetInstance().GetRateRange(scoreid))
                {
                    DatabaseProvider.GetInstance().UpdateRateRange(dr["raterange"].ToString().Replace(scoreid + ",True,", scoreid + ",False,"), Utils.StrToInt(dr["groupid"], 0));
                }
            }

            #endregion
        }

        public void DataGrid_Update(Object sender, DataGridCommandEventArgs E)
        {
            #region ������Ӧ�Ļ���ѡ��

            string id = DataGrid1.DataKeys[(int)E.Item.ItemIndex].ToString();
            string item3 = ((System.Web.UI.WebControls.TextBox)E.Item.Cells[3].Controls[0]).Text.Trim();
            string item4 = ((System.Web.UI.WebControls.TextBox)E.Item.Cells[4].Controls[0]).Text.Trim();
            string item5 = ((System.Web.UI.WebControls.TextBox)E.Item.Cells[5].Controls[0]).Text.Trim();
            string item6 = ((System.Web.UI.WebControls.TextBox)E.Item.Cells[6].Controls[0]).Text.Trim();
            string item7 = ((System.Web.UI.WebControls.TextBox)E.Item.Cells[7].Controls[0]).Text.Trim();
            string item8 = ((System.Web.UI.WebControls.TextBox)E.Item.Cells[8].Controls[0]).Text.Trim();
            string item9 = ((System.Web.UI.WebControls.TextBox)E.Item.Cells[9].Controls[0]).Text.Trim();
            string item10 = ((System.Web.UI.WebControls.TextBox)E.Item.Cells[10].Controls[0]).Text.Trim();

            int rowindex = Convert.ToInt16(id);
            bool flag = true;

            //��������ֻ��Ϊ���������
            if (rowindex <= 2)
            {
                if (rowindex == 1)
                {
                    if (item3 == "") SetUserGroupRaterange(1);
                    if (item4 == "") SetUserGroupRaterange(2);
                    if (item5 == "") SetUserGroupRaterange(3);
                    if (item6 == "") SetUserGroupRaterange(4);
                    if (item7 == "") SetUserGroupRaterange(5);
                    if (item8 == "") SetUserGroupRaterange(6);
                    if (item9 == "") SetUserGroupRaterange(7);
                    if (item10 == "") SetUserGroupRaterange(8);
                }

                //�Ƿ�Ϊ�ջ���������
                if (item3 != "" && (Utils.IsNumeric(item3.Replace("-", "")))) flag = false;
                if (item4 != "" && (Utils.IsNumeric(item4.Replace("-", "")))) flag = false;
                if (item5 != "" && (Utils.IsNumeric(item5.Replace("-", "")))) flag = false;
                if (item6 != "" && (Utils.IsNumeric(item6.Replace("-", "")))) flag = false;
                if (item7 != "" && (Utils.IsNumeric(item7.Replace("-", "")))) flag = false;
                if (item8 != "" && (Utils.IsNumeric(item8.Replace("-", "")))) flag = false;
                if (item9 != "" && (Utils.IsNumeric(item9.Replace("-", "")))) flag = false;
                if (item10 != "" && (Utils.IsNumeric(item10.Replace("-", "")))) flag = false;

                if (!flag)
                {
                    base.RegisterStartupScript( "DataGrid1",
                                               "<script>alert('��ǰ�������ݲ���Ϊ����');window.location.href='global_scoreset.aspx';</script>");
                    return;
                }
            }
            else
            {
                if (item3 == "") flag = false;
                if (item4 == "") flag = false;
                if (item5 == "") flag = false;
                if (item6 == "") flag = false;
                if (item7 == "") flag = false;
                if (item8 == "") flag = false;
                if (item9 == "") flag = false;
                if (item10 == "") flag = false;

                if (!flag)
                {
                    base.RegisterStartupScript( "DataGrid1",
                                               "<script>alert('��ǰ�������ݲ���Ϊ��.');window.location.href='global_scoreset.aspx';</script>");
                    return;
                }

                flag = true;

                //�Ƿ�Ϊ�ջ��������
                if (item3 != "" && (!Utils.IsNumeric(item3.Replace("-", "")))) flag = false;
                if (item4 != "" && (!Utils.IsNumeric(item4.Replace("-", "")))) flag = false;
                if (item5 != "" && (!Utils.IsNumeric(item5.Replace("-", "")))) flag = false;
                if (item6 != "" && (!Utils.IsNumeric(item6.Replace("-", "")))) flag = false;
                if (item7 != "" && (!Utils.IsNumeric(item7.Replace("-", "")))) flag = false;
                if (item8 != "" && (!Utils.IsNumeric(item8.Replace("-", "")))) flag = false;
                if (item9 != "" && (!Utils.IsNumeric(item9.Replace("-", "")))) flag = false;
                if (item10 != "" && (!Utils.IsNumeric(item10.Replace("-", "")))) flag = false;

                if (!flag)
                {
                    base.RegisterStartupScript( "DataGrid1",
                                               "<script>alert('��ǰ��������ֻ��Ϊ����.');window.location.href='global_scoreset.aspx';</script>");
                    return;
                }

                flag = true;

                //���ַ�Χ�Ƿ���-999 �� 999֮��
                if ((Convert.ToDouble(item3) > 999) || (Convert.ToDouble(item3) < -999)) flag = false;
                if ((Convert.ToDouble(item4) > 999) || (Convert.ToDouble(item4) < -999)) flag = false;
                if ((Convert.ToDouble(item5) > 999) || (Convert.ToDouble(item5) < -999)) flag = false;
                if ((Convert.ToDouble(item6) > 999) || (Convert.ToDouble(item6) < -999)) flag = false;
                if ((Convert.ToDouble(item7) > 999) || (Convert.ToDouble(item7) < -999)) flag = false;
                if ((Convert.ToDouble(item8) > 999) || (Convert.ToDouble(item8) < -999)) flag = false;
                if ((Convert.ToDouble(item9) > 999) || (Convert.ToDouble(item9) < -999)) flag = false;
                if ((Convert.ToDouble(item10) > 999) || (Convert.ToDouble(item10) < -999)) flag = false;

                if (!flag)
                {
                    base.RegisterStartupScript( "DataGrid1",
                                               "<script>alert('����������������ķ�ΧΪ-999��+999.');window.location.href='global_scoreset.aspx';</script>");
                    return;
                }
            }

            dsSrc = new DataSet();

            dsSrc.ReadXml(Server.MapPath("../../config/scoreset.config"));

            foreach (DataRow dr in dsSrc.Tables["record"].Rows)
            {
                int currentid = Convert.ToInt16(dr["id"].ToString());
                if (id == currentid.ToString())
                {
                    if (rowindex <= 2)
                    {
                        dr["extcredits1"] = item3;
                        dr["extcredits2"] = item4;
                        dr["extcredits3"] = item5;
                        dr["extcredits4"] = item6;
                        dr["extcredits5"] = item7;
                        dr["extcredits6"] = item8;
                        dr["extcredits7"] = item9;
                        dr["extcredits8"] = item10;
                    }
                    else
                    {
                        dr["extcredits1"] = Math.Round(Double.Parse(item3), 2);
                        dr["extcredits2"] = Math.Round(Double.Parse(item4), 2);
                        dr["extcredits3"] = Math.Round(Double.Parse(item5), 2);
                        dr["extcredits4"] = Math.Round(Double.Parse(item6), 2);
                        dr["extcredits5"] = Math.Round(Double.Parse(item7), 2);
                        dr["extcredits6"] = Math.Round(Double.Parse(item8), 2);
                        dr["extcredits7"] = Math.Round(Double.Parse(item9), 2);
                        dr["extcredits8"] = Math.Round(Double.Parse(item10), 2);
                    }
                }

                if (currentid > 2)
                {
                    dr["extcredits1"] = Math.Round(Double.Parse(dr["extcredits1"].ToString()), 2);
                    dr["extcredits2"] = Math.Round(Double.Parse(dr["extcredits2"].ToString()), 2);
                    dr["extcredits3"] = Math.Round(Double.Parse(dr["extcredits3"].ToString()), 2);
                    dr["extcredits4"] = Math.Round(Double.Parse(dr["extcredits4"].ToString()), 2);
                    dr["extcredits5"] = Math.Round(Double.Parse(dr["extcredits5"].ToString()), 2);
                    dr["extcredits6"] = Math.Round(Double.Parse(dr["extcredits6"].ToString()), 2);
                    dr["extcredits7"] = Math.Round(Double.Parse(dr["extcredits7"].ToString()), 2);
                    dr["extcredits8"] = Math.Round(Double.Parse(dr["extcredits8"].ToString()), 2);

                    //���Ǽ����ֲ���ʱ��ϵͳ�Զ�ȡ������������Ҫ��Ϊ�˴�config�ļ���ȡ����ֵ�����������ɲ���ǰ̨��������
                    //if ((currentid == 11) || (currentid == 12) || (currentid == 13))
                    //{
                    //    dr["extcredits1"] = (-1) * Double.Parse(dr["extcredits1"].ToString());
                    //    dr["extcredits2"] = (-1) * Double.Parse(dr["extcredits2"].ToString());
                    //    dr["extcredits3"] = (-1) * Double.Parse(dr["extcredits3"].ToString());
                    //    dr["extcredits4"] = (-1) * Double.Parse(dr["extcredits4"].ToString());
                    //    dr["extcredits5"] = (-1) * Double.Parse(dr["extcredits5"].ToString());
                    //    dr["extcredits6"] = (-1) * Double.Parse(dr["extcredits6"].ToString());
                    //    dr["extcredits7"] = (-1) * Double.Parse(dr["extcredits7"].ToString());
                    //    dr["extcredits8"] = (-1) * Double.Parse(dr["extcredits8"].ToString());
                    //}
                }
            }


            try
            {

                dsSrc.WriteXml(Server.MapPath("../../config/scoreset.config"));
                dsSrc.Reset();
                dsSrc.Dispose();

                Discuz.Cache.DNTCache cache = Discuz.Cache.DNTCache.GetCacheService();
                cache.RemoveObject("/Forum/ScorePaySet");
                cache.RemoveObject("/Forum/ScoreSet");
                cache.RemoveObject("/Forum/ValidScoreName");
                cache.RemoveObject("/Forum/ValidScoreUnit");
                cache.RemoveObject("/Forum/RateScoreSet");

                AdminVistLogs.InsertLog(this.userid, this.username, this.usergroupid, this.grouptitle, this.ip,
                                              "��������", "");

                DataGrid1.EditItemIndex = -1;

                dsSrc = new DataSet();
                dsSrc.ReadXml(Server.MapPath("../../config/scoreset.config"));
                DataGrid1.DataSource = AbsScoreSet(dsSrc.Tables[0]);
                DataGrid1.DataBind();
            }
            catch
            {
                base.RegisterStartupScript( "DataGrid1",
                                           "<script>alert('�޷��������ݿ�.');window.location.href='global_scoreset.aspx';</script>");
                return;
            }

            if (rowindex > 2)
            {
                Regex r = new Regex(@"^\d+(\.\d{1,2})?$");
                if (!r.IsMatch(item3.Replace("-", "")) || !r.IsMatch(item4.Replace("-", "")) ||
                    !r.IsMatch(item5.Replace("-", "")) || !r.IsMatch(item6.Replace("-", "")) ||
                    !r.IsMatch(item7.Replace("-", "")) || !r.IsMatch(item8.Replace("-", "")) ||
                    !r.IsMatch(item9.Replace("-", "")) || !r.IsMatch(item10.Replace("-", "")))
                {
                    base.RegisterStartupScript( "DataGrid1",
                                               "<script>alert('��ǰ���������ֻ��ΪСλ�����λ,ϵͳ���������������.');window.location.href='global_scoreset.aspx';</script>");
                    return;
                }
            }
            #endregion
        }

        private void Save_Click(object sender, EventArgs e)
        {
            #region �������������Ϣ

            if (this.CheckCookie())
            {
                if ((Convert.ToDouble(creditstax.Text.Trim()) > 1) || (Convert.ToDouble(creditstax.Text.Trim()) < 0))
                {
                    base.RegisterStartupScript( "", "<script>alert('���ֽ���˰������0--1֮���С��');window.location.href='global_scoreset.aspx';</script>");
                    return;
                }

                if (Convert.ToDouble(transfermincredits.Text.Trim()) < 0)
                {
                    base.RegisterStartupScript( "", "<script>alert('ת������������Ǵ��ڻ����0');window.location.href='global_scoreset.aspx';</script>");
                    return;
                }

                if (Convert.ToDouble(exchangemincredits.Text.Trim()) < 0)
                {
                    base.RegisterStartupScript( "", "<script>alert('�һ�����������Ǵ��ڻ����0');window.location.href='global_scoreset.aspx';</script>");
                    return;
                }

                if (Convert.ToDouble(maxincperthread.Text.Trim()) < 0)
                {
                    base.RegisterStartupScript( "", "<script>alert('�����������������Ǵ��ڻ����0');window.location.href='global_scoreset.aspx';</script>");
                    return;
                }

                if (Convert.ToDouble(maxchargespan.Text.Trim()) < 0)
                {
                    base.RegisterStartupScript( "", "<script>alert('��������߳���ʱ�ޱ����Ǵ��ڻ����0');window.location.href='global_scoreset.aspx';</script>");
                    return;
                }

                dsSrc.ReadXml(Server.MapPath("../../config/scoreset.config"));
                dsSrc.Tables["formula"].Rows[0]["formulacontext"] = formula.Text;
                dsSrc.Tables["formula"].Rows[0]["creditstrans"] = creditstrans.SelectedValue;
                dsSrc.Tables["formula"].Rows[0]["creditstax"] = Convert.ToDouble(creditstax.Text);
                dsSrc.Tables["formula"].Rows[0]["transfermincredits"] = Convert.ToDouble(transfermincredits.Text);
                dsSrc.Tables["formula"].Rows[0]["exchangemincredits"] = Convert.ToDouble(exchangemincredits.Text);
                dsSrc.Tables["formula"].Rows[0]["maxincperthread"] = Convert.ToDouble(maxincperthread.Text);
                dsSrc.Tables["formula"].Rows[0]["maxchargespan"] = Convert.ToDouble(maxchargespan.Text);
                dsSrc.WriteXml(Server.MapPath("../../config/scoreset.config"));


                Discuz.Cache.DNTCache cache = Discuz.Cache.DNTCache.GetCacheService();
                cache.RemoveObject("/Forum/Scoreset");
                cache.RemoveObject("/Forum/Scoreset/CreditsTrans");
                cache.RemoveObject("/Forum/Scoreset/CreditsTax");
                cache.RemoveObject("/Forum/Scoreset/TransferMinCredits");
                cache.RemoveObject("/Forum/Scoreset/ExchangeMinCredits");
                cache.RemoveObject("/Forum/Scoreset/MaxIncPerThread");
                cache.RemoveObject("/Forum/Scoreset/MaxChargeSpan");
                cache.RemoveObject("/Forum/ValidScoreUnit");
                cache.RemoveObject("/Forum/RateScoreSet");

                AdminVistLogs.InsertLog(this.userid, this.username, this.usergroupid, this.grouptitle, this.ip, "�޸Ļ�������", "�޸Ļ�������");

                if (RefreshUserScore.SelectedValue.IndexOf("1") == 0)
                {
                    //DbHelper.ExecuteNonQuery("UPDATE [" + BaseConfigs.GetTablePrefix + "users] SET [credits]=" + formula.Text);
                    DatabaseProvider.GetInstance().UpdateUserCredits(formula.Text);
                }

                base.RegisterStartupScript( "PAGE",  "window.location.href='global_scoreset.aspx';");
            }

            #endregion
        }

        protected void DataGrid_Delete(Object sender, DataGridCommandEventArgs E)
        {
            DataGrid1.DataKeys[E.Item.ItemIndex].ToString();
        }

        protected void DataGrid_Edit(Object sender, DataGridCommandEventArgs E)
        {
            #region �༭����
            DataGrid1.EditItemIndex = E.Item.ItemIndex;
            dsSrc.Clear();
            dsSrc.ReadXml(Server.MapPath("../../config/scoreset.config"));
            DataGrid1.DataSource = AbsScoreSet(dsSrc.Tables[0]);
            DataGrid1.DataBind();
            #endregion
        }

        protected void DataGrid_Cancel(Object sender, DataGridCommandEventArgs E)
        {
            #region ȡ���༭
            DataGrid1.EditItemIndex = -1;
            DataGrid1.DataSource = AbsScoreSet(Scoresets.GetScoreSet());
            DataGrid1.DataBind();
            #endregion
        }

        public string GetImgLink(string img)
        {
            if (img != "")
            {
                return "<img src=../../images/groupicons/" + img + ">";
            }
            return "";
        }

        #region Web Form Designer generated code

        protected override void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        private void InitializeComponent()
        {
            this.DataGrid1.EditCommand += new DataGridCommandEventHandler(this.DataGrid_Edit);
            this.DataGrid1.CancelCommand += new DataGridCommandEventHandler(DataGrid_Cancel);
            //this.DataGrid1.UpdateCommand += new DataGridCommandEventHandler(this.DataGrid_Update);
            this.DataGrid1.ItemDataBound += new DataGridItemEventHandler(this.DataGrid_ItemDataBound);
            //this.Load += new EventHandler(this.Page_Load);
            this.Save.Click += new EventHandler(this.Save_Click);

            formula.IsReplaceInvertedComma = false;

            DataGrid1.LoadEditColumn();
            DataGrid1.AllowSorting = false;
            DataGrid1.AllowPaging = false;
            DataGrid1.ShowFooter = false;
        }

        #endregion
    }
}