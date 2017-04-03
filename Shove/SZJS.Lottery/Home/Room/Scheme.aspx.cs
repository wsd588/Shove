﻿using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Shove.Database;
using System.Text;
using System.Web.UI;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public partial class Home_Room_Scheme : SitePageBase
{
    protected long SchemeID = -1;
    private bool Opt_FullSchemeCanQuash = false;

    public string LotteryID = "5";
    public string LotteryName;
    public int PlayTypeID;
    private string dingZhi = "";
    public static Dictionary<int, string> Lotteries = new Dictionary<int, string>();

    static Home_Room_Scheme()
    {
        Lotteries[59] = "15X5";
        Lotteries[9] = "22X5";
        Lotteries[65] = "31X7";
        Lotteries[6] = "3D";
        Lotteries[39] = "CJDLT";
        Lotteries[58] = "DF6J1";
        Lotteries[2] = "JQC";
        Lotteries[15] = "LCBQC";
        Lotteries[63] = "PL3";
        Lotteries[64] = "PL5";
        Lotteries[13] = "QLC";
        Lotteries[3] = "QXC";
        Lotteries[1] = "SFC";
        Lotteries[61] = "SSC";
        Lotteries[29] = "SSL";
        Lotteries[5] = "SSQ";
        Lotteries[62] = "SYYDJ";
        Lotteries[72] = "JCZQ";
        Lotteries[73] = "JCLQ";
        Lotteries[74] = "SFC";
        Lotteries[75] = "SFC_9_D";
    }

    protected void Page_Load(object sender, System.EventArgs e)
    {
        AjaxPro.Utility.RegisterTypeForAjax(typeof(Home_Room_Scheme), this.Page);

        SchemeID = Shove._Convert.StrToLong(Shove._Web.Utility.GetRequest("id"), -1);

        if (SchemeID < 1)
        {
            PF.GoError(ErrorNumber.Unknow, "参数错误(-26)", this.GetType().FullName);

            return;
        }

        Opt_FullSchemeCanQuash = _Site.SiteOptions["Opt_FullSchemeCanQuash"].ToBoolean(false);

        if (!IsPostBack)
        {
            tbSchemeID.Text = SchemeID.ToString();
            if (_User != null)
            {
                labBalance.Text = _User.Balance.ToString("N");
            }

            BindData();

            bool isBuyValidPasswordAdv = _Site.SiteOptions["Opt_isBuyValidPasswordAdv"].ToBoolean(false);

            if (isBuyValidPasswordAdv)  //如果普通用户需要输入投注密码，支付宝会员不需要
            {
                isBuyValidPasswordAdv = false;
            }

            panelInvestPassword.Visible = isBuyValidPasswordAdv;

        }

    }

    #region Web 窗体设计器生成的代码

    override protected void OnInit(EventArgs e)
    {
        RequestLoginPage = this.Request.Url.AbsoluteUri;
        isRequestLogin = false;
        base.OnInit(e);
    }

    #endregion

    private void BindData()
    {
        DataTable dt = new DAL.Views.V_SchemeSchedulesWithQuashed().Open("", "SiteID = " + _Site.ID.ToString() + " and [id] = " + Shove._Web.Utility.FilteSqlInfusion(tbSchemeID.Text), "");

        if ((dt == null) || (dt.Rows.Count < 1))
        {
            PF.GoError(ErrorNumber.DataReadWrite, "数据库繁忙，请重试(-141)", this.GetType().BaseType.FullName);

            return;
        }

        DataRow dr = dt.Rows[0];

        long InitiateUserID = Shove._Convert.StrToLong(dr["InitiateUserID"].ToString(), 0);
        hfID.Value = InitiateUserID.ToString();
        LotteryName = dr["LotteryName"].ToString();
        if (LotteryName == "江西时时彩")
        {
            LotteryName = LotteryName.Replace("江西", "");
        }
        Label3.Text = LotteryName + "<font class='red14'>" + dr["IsuseName"].ToString() + "</font>期" + dr["PlayTypeName"].ToString() + "认购方案";
        labTitle.Text = dr["IsuseName"].ToString();
        labStartTime.Text = dr["StartTime"].ToString();
        tbIsuseID.Text = dr["IsuseID"].ToString();
        tbLotteryID.Text = dr["LotteryID"].ToString();
        LotteryID = tbLotteryID.Text;
        PlayTypeID = Shove._Convert.StrToInt(dr["PlayTypeID"].ToString(), 0);

        labEndTime.Text = dr["SystemEndTime"].ToString();

        labInitiateUser.Text = dr["InitiateName"].ToString() + "&nbsp;&nbsp;【<A class=li3 href='../Web/Score.aspx?id=" + dr["InitiateUserID"].ToString() + "&LotteryID=" + tbLotteryID.Text + "' target='_blank'>发起人历史战绩</A>】";

        short QuashStatus = Shove._Convert.StrToShort(dr["QuashStatus"].ToString(), 0);
        Shove._Web.Cache.SetCache("All_QuashStatus" + SchemeID, QuashStatus, 600);
        bool Buyed = Shove._Convert.StrToBool(dr["Buyed"].ToString(), false);
        int Share = Shove._Convert.StrToInt(dr["Share"].ToString(), 0);
        int BuyedShare = Shove._Convert.StrToInt(dr["BuyedShare"].ToString(), 0);
        double Money = Shove._Convert.StrToDouble(dr["Money"].ToString(), 0);
        double AssureMoney = Shove._Convert.StrToDouble(dr["AssureMoney"].ToString(), 0);
        double WinMoney = Shove._Convert.StrToDouble(dr["WinMoney"].ToString(), 0);
        short SecrecyLevel = Shove._Convert.StrToShort(dr["SecrecyLevel"].ToString(), 0);
        bool IsuseOpenedWined = false;
        bool isCanChat = Shove._Convert.StrToBool(dr["isCanChat"].ToString(), false);
        if (Share > 1)
        {
            trBonusScale.Visible = true;
            lbSchemeBonus.Text = (Shove._Convert.StrToDouble(dr["SchemeBonusScale"].ToString(), 0.04) * 100).ToString() + "%";
        }

        HidSchedule.Value = dr["Schedule"].ToString();
        DataTable dtIsuse = dtIsuse = new DAL.Views.V_Isuses().Open("IsOpened, WinLotteryNumber,Code", "[id] = " + dr["IsuseID"].ToString(), "");

        if (dtIsuse == null)
        {
            PF.GoError(ErrorNumber.DataReadWrite, "数据库繁忙，请重试(-213)", this.GetType().FullName);

            return;
        }

        if (dtIsuse.Rows.Count < 1)
        {
            PF.GoError(ErrorNumber.Unknow, "系统错误(-220)", this.GetType().FullName);

            return;
        }

        IsuseOpenedWined = Shove._Convert.StrToBool(dtIsuse.Rows[0]["IsOpened"].ToString(), true);

        lbWinNumber.Text = dtIsuse.Rows[0]["WinLotteryNumber"].ToString();
        ImageLogo.ImageUrl = "images/lottery/" + dtIsuse.Rows[0]["Code"].ToString().ToLower() + ".jpg";

        //能撤消整个方案
        //Opt_FullSchemeCanQuash 是否允许撤消满员方案
        bool isSchemeCanQuash = _Site.SiteOptions["Opt_FullSchemeCanQuash"].ToBoolean(false);

        if (!isSchemeCanQuash)
        {
            btnQuashScheme.Visible = ((QuashStatus == 0) && (!Buyed) && (Share > BuyedShare) && _User != null && (InitiateUserID == _User.ID));
        }
        else
        {
            btnQuashScheme.Visible = ((QuashStatus == 0) && (!Buyed) && _User != null && (InitiateUserID == _User.ID));
        }

        short AtTopStatus = Shove._Convert.StrToShort(dr["AtTopStatus"].ToString(), 0);
        bool AtTopApplication = (AtTopStatus != 0);

        if (AtTopStatus == 0)
        {
            cbAtTopApplication.Visible = ((QuashStatus == 0) && (!Buyed) && (Share > BuyedShare) && _User != null && (InitiateUserID == _User.ID));
            cbAtTopApplication.Checked = AtTopApplication;
        }
        else
        {
            labAtTop.Visible = true;
        }

        bool CanBuy = false;
        bool Stop = false;
        System.DateTime EndTime = Shove._Convert.StrToDateTime(labEndTime.Text, DateTime.Now.ToString());

        if (DateTime.Now >= EndTime)
        {
            Stop = true;
            tbStop.Text = Stop.ToString();
        }

        if (QuashStatus > 0)
        {
            if (QuashStatus == 2)
            {
                labState.Text = "已撤单(系统撤单)";
            }
            else
            {
                labState.Text = "已撤单";
            }
        }
        else
        {
            if (Stop)
            {
                labState.Text = "已截止";
            }
            else
            {
                if (Buyed)
                {
                    labState.Text = "<FONT color='red'>等待出票</font>";
                }
                else
                {
                    if (Share <= BuyedShare)
                    {
                        labState.Text = "<FONT color='red'>已满员</font>";
                    }
                    else
                    {
                        labState.Text = "<font color='red'>抢购中...</font>";
                        CanBuy = true;
                    }
                }
            }
        }

        labMultiple.Text = dr["Multiple"].ToString();

        //  SecrecyLevel 0 不保密 1 到截止 2 到开奖 3 永远
        if ((SecrecyLevel == SchemeSecrecyLevels.ToDeadline) && !Stop && ((_User == null) || ((_User != null) && (InitiateUserID != _User.ID) && (!_User.isOwnedViewSchemeCompetence()))))
        {
            labLotteryNumber.Text = "投注内容已经被保密，将在本期投注截止后公开。";
        }
        else if ((SecrecyLevel == SchemeSecrecyLevels.ToOpen) && !IsuseOpenedWined && ((_User == null) || ((_User != null) && (InitiateUserID != _User.ID) && (!_User.isOwnedViewSchemeCompetence()))))
        {
            labLotteryNumber.Text = "投注内容已经被保密，将在本期开奖后公开。";
        }
        else if ((SecrecyLevel == SchemeSecrecyLevels.Always) && ((_User == null) || ((_User != null) && (InitiateUserID != _User.ID) && (!_User.isOwnedViewSchemeCompetence()))))
        {
            labLotteryNumber.Text = "投注内容已经被保密。";
        }
        else
        {
            int MaxShowLotteryNumberRows = _Site.SiteOptions["Opt_MaxShowLotteryNumberRows"].ToShort(0);
            string t_str = "";

            try
            {
                t_str = dr["LotteryNumber"].ToString();
            }
            catch { }

            if (Shove._String.StringAt(t_str, '\n') < MaxShowLotteryNumberRows)
            {
                labLotteryNumber.Text = Shove._Convert.ToHtmlCode(t_str) + "&nbsp;";

                if (IsuseOpenedWined)
                {
                    NumberDuiBi(labLotteryNumber.Text.Replace("<br/>", "\n"), lbWinNumber.Text, PlayTypeID);
                }

                #region 对阵信息

                //74胜负彩
                StringBuilder sb = new StringBuilder();

                if (dr["LotteryID"].ToString() == "74")
                {
                    string CacheKey = "dtZCDZ_" + dr["LotteryID"].ToString();
                    DataTable dtZCDZ = Shove._Web.Cache.GetCacheAsDataTable(CacheKey);

                    if (dtZCDZ == null)
                    {
                        dtZCDZ = new DAL.Tables.T_IsuseForSFC().Open("", "IsuseID=" + dr["IsuseID"].ToString(), "[NO]");

                        Shove._Web.Cache.SetCache(CacheKey, dtZCDZ);
                    }

                    //格式话号码
                    t_str = t_str.Replace("0", "4");
                    string[] xhfzshu = t_str.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries); //选号分组
                    string playTypeName = dr["PlayTypeName"].ToString(); //玩法名称
                    int N = xhfzshu.Length; //分组数
                    int[, ,] fzHao = new int[N, 14, 3]; //分组号码 N*14*3 矩阵

                    for (int i = 0; i < N; i++)
                    {
                        if (playTypeName == "单式")
                        {
                            for (int j = 0; j < 14; j++)
                            {
                                int number = int.Parse(xhfzshu[i].Substring(j, 1)); //取一个号
                                //记录选号，未选中为-1
                                if (number == 3) { fzHao[i, j, 0] = 3; } else { fzHao[i, j, 0] = -1; }
                                if (number == 1) { fzHao[i, j, 1] = 1; } else { fzHao[i, j, 1] = -1; }
                                if (number == 4) { fzHao[i, j, 2] = 4; } else { fzHao[i, j, 2] = -1; }
                            }
                        }

                        if (playTypeName == "复式")
                        {
                            string zxh = xhfzshu[i].Replace("-", "(2)").Replace("(", "|(").Replace(")", ")|").Replace("||", "|").Trim();

                            if (zxh.StartsWith("|"))
                            {
                                zxh = zxh.Remove(0, 1);
                            }

                            if (zxh.EndsWith("|"))
                            {
                                zxh = zxh.Substring(0, zxh.Length - 1);
                            }

                            string[] arrZxh = zxh.Split('|');

                            zxh = "";

                            foreach (string s in arrZxh)
                            {
                                string ss = "";

                                if (s.StartsWith("("))
                                {
                                    ss = s;
                                }
                                else
                                {
                                    for (int l = 0; l < s.Length; l++)
                                    {
                                        ss += "(" + s.Substring(l, 1) + ")";
                                    }
                                }

                                zxh += ss;
                            }

                            zxh = zxh.Replace(")", "|").Replace("(", "|").Replace("||", "|");

                            if (zxh.StartsWith("|"))
                            {
                                zxh = zxh.Remove(0, 1);
                            }

                            if (zxh.EndsWith("|"))
                            {
                                zxh = zxh.Substring(0, zxh.Length - 1);
                            }

                            arrZxh = zxh.Split('|');

                            for (int j = 0; j < 14; j++)
                            {
                                int[] Q = new int[] { 3, 1, 4 };

                                //记录选号，未选中为-1
                                for (int y = 0; y < arrZxh[j].Length; y++)
                                {
                                    int number = int.Parse(arrZxh[j].Substring(y, 1));

                                    for (int x = 0; x < 3; x++)
                                    {
                                        if (number == Q[x])
                                        {
                                            fzHao[i, j, x] = Q[x];
                                        }
                                        else
                                        {
                                            if (fzHao[i, j, x] == 0)
                                            {
                                                fzHao[i, j, x] = -1;
                                            }
                                        }
                                    }
                                }
                            }

                        }
                    }

                    StringBuilder sbXhfz = new StringBuilder();

                    for (int i = 0; i < N; i++) //分组数
                    {
                        sbXhfz.Append("<div class='blue'><a href='javascript:;' onmouseover='displayBall(").Append((i + 1).ToString()).Append(",").Append(N.ToString()).Append(",14);' onmouseout=\"Div_Zc.style.display='none'\">").Append(xhfzshu[i].Replace("4", "0")).Append("</a>").Append(i == N - 1 && i > 0 ? "<span style='margin-left:40px;color:red;'>【注】</span>鼠标移动到投注内容查看选号区" : "").Append("</div>");
                    }

                    labLotteryNumber.Text = sbXhfz.ToString();

                    sb.Append("<table style='width:100%; background-color:#ccc; text-align:center; line-height:25px;' cellspacing='1' cellpadding='0'>")
                        .Append("<tr style='color:red;'><td style='width:50px;'>场次</td><td>对阵</td><td style='width:140px;'>比赛时间</td><td>选号区</td></tr>")
                        .Append("<tbody style='background-color:White;'>");

                    for (int i = 0; i < 14; i++)
                    {
                        sb.Append("<tr><td>").Append(dtZCDZ.Rows[i]["NO"].ToString())
                            .Append("</td><td>").Append(dtZCDZ.Rows[i]["HostTeam"].ToString()).Append("<span style='color:red;'> VS </span>").Append(dtZCDZ.Rows[i]["QuestTeam"].ToString())
                            .Append("</td><td>").Append(dtZCDZ.Rows[i]["DateTime"].ToString())
                            .Append("</td><td>");

                        for (int k = 0; k < N; k++)
                        {
                            sb.Append("<table id='Xhfz_").Append((100 * (k + 1) + i + 1).ToString()).Append("' style='color:White; font-weight:bold;").Append(k == 0 ? "" : "display:none;").Append("'><tr>");

                            int[] Q = new int[] { 3, 1, 4 };
                            for (int j = 0; j < 3; j++)
                            {
                                sb.Append("<td style='background-image:url(Images/");
                                sb.Append(fzHao[k, i, j] == Q[j] ? "zfb_redball" : "zfb_huiball").Append(".gif);background-repeat:no-repeat; height:25px;width:25px;'>");
                                sb.Append((Q[j] == 4 ? 0 : Q[j]).ToString());
                                sb.Append("</td>");
                            }

                            sb.Append("</tr></table>");
                        }

                        sb.Append("</td></tr>");
                    }

                    sb.Append("</tbody></table>");
                }

                //75任九场
                if (dr["LotteryID"].ToString() == "75")
                {
                    string CacheKey = "dtZCDZ_" + dr["LotteryID"].ToString();
                    DataTable dtZCDZ = Shove._Web.Cache.GetCacheAsDataTable(CacheKey);

                    if (dtZCDZ == null)
                    {
                        dtZCDZ = new DAL.Tables.T_IsuseForSFC().Open("", "IsuseID=" + dr["IsuseID"].ToString(), "[NO]");

                        Shove._Web.Cache.SetCache(CacheKey, dtZCDZ);
                    }

                    //格式话号码
                    t_str = t_str.Replace("0", "4");
                    string[] xhfzshu = t_str.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries); //选号分组
                    string playTypeName = dr["PlayTypeName"].ToString(); //玩法名称
                    int N = xhfzshu.Length; //分组数
                    int[, ,] fzHao = new int[N, 14, 3]; //分组号码 N*14*3 矩阵

                    for (int i = 0; i < N; i++)
                    {

                        if (playTypeName == "单式")
                        {
                            for (int j = 0; j < 14; j++)
                            {
                                int number = int.Parse(xhfzshu[i].Substring(j, 1) == "-" ? "-1" : xhfzshu[i].Substring(j, 1)); //取一个号
                                int[] Q = new int[] { 3, 1, 4 };

                                for (int k = 0; k < 3; k++)
                                {
                                    //记录选号，未选中为-1
                                    if (number == Q[k])
                                    {
                                        fzHao[i, j, k] = Q[k];
                                    }
                                    else
                                    {
                                        fzHao[i, j, k] = -1;
                                    }
                                }
                            }
                        }

                        if (playTypeName == "复式")
                        {
                            string zxh = xhfzshu[i].Replace("-", "(2)").Replace("(", "|(").Replace(")", ")|").Replace("||", "|").Trim();

                            if (zxh.StartsWith("|"))
                            {
                                zxh = zxh.Remove(0, 1);
                            }

                            if (zxh.EndsWith("|"))
                            {
                                zxh = zxh.Substring(0, zxh.Length - 1);
                            }

                            string[] arrZxh = zxh.Split('|');

                            zxh = "";

                            foreach (string s in arrZxh)
                            {
                                string ss = "";

                                if (s.StartsWith("("))
                                {
                                    ss = s;
                                }
                                else
                                {
                                    for (int l = 0; l < s.Length; l++)
                                    {
                                        ss += "(" + s.Substring(l, 1) + ")";
                                    }
                                }

                                zxh += ss;
                            }

                            zxh = zxh.Replace(")", "|").Replace("(", "|").Replace("||", "|");

                            if (zxh.StartsWith("|"))
                            {
                                zxh = zxh.Remove(0, 1);
                            }

                            if (zxh.EndsWith("|"))
                            {
                                zxh = zxh.Substring(0, zxh.Length - 1);
                            }

                            arrZxh = zxh.Split('|');

                            for (int j = 0; j < 14; j++)
                            {
                                int[] Q = new int[] { 3, 1, 4 };

                                //记录选号，未选中为-1
                                for (int y = 0; y < arrZxh[j].Length; y++)
                                {
                                    int number = int.Parse(arrZxh[j].Substring(y, 1));

                                    for (int x = 0; x < 3; x++)
                                    {
                                        if (number == Q[x])
                                        {
                                            fzHao[i, j, x] = Q[x];
                                        }
                                        else
                                        {
                                            if (fzHao[i, j, x] == 0)
                                            {
                                                fzHao[i, j, x] = -1;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    StringBuilder sbXhfz = new StringBuilder();

                    for (int i = 0; i < N; i++) //分组数
                    {
                        sbXhfz.Append("<div class='blue'><a href='javascript:;' onmouseover='displayBall(").Append((i + 1).ToString()).Append(",").Append(N.ToString()).Append(",14);' onmouseout=\"Div_Zc.style.display='none'\">").Append(xhfzshu[i].Replace("4", "0")).Append("</a>").Append(i == N - 1 && i > 0 ? "<span style='margin-left:40px;color:red;'>【注】</span>鼠标移动到投注内容查看选号区" : "").Append("</div>");
                    }

                    labLotteryNumber.Text = sbXhfz.ToString();

                    sb.Append("<table style='width:100%; background-color:#ccc; text-align:center; line-height:25px;' cellspacing='1' cellpadding='0'>")
                        .Append("<tr style='color:red;'><td style='width:50px;'>场次</td><td>对阵</td><td style='width:140px;'>比赛时间</td><td>选号区</td></tr>")
                        .Append("<tbody style='background-color:White;'>");

                    for (int i = 0; i < 14; i++)
                    {
                        sb.Append("<tr><td>").Append(dtZCDZ.Rows[i]["NO"].ToString())
                            .Append("</td><td>").Append(dtZCDZ.Rows[i]["HostTeam"].ToString()).Append("<span style='color:red;'> VS </span>").Append(dtZCDZ.Rows[i]["QuestTeam"].ToString())
                            .Append("</td><td>").Append(dtZCDZ.Rows[i]["DateTime"].ToString())
                            .Append("</td><td>");

                        for (int k = 0; k < N; k++)
                        {
                            sb.Append("<table id='Xhfz_").Append((100 * (k + 1) + i + 1).ToString()).Append("' style='color:White; font-weight:bold;").Append(k == 0 ? "" : "display:none;").Append("'><tr>");

                            int[] Q = new int[] { 3, 1, 4 };
                            for (int j = 0; j < 3; j++)
                            {
                                sb.Append("<td style='background-image:url(Images/");
                                sb.Append(fzHao[k, i, j] == Q[j] ? "zfb_redball" : "zfb_huiball").Append(".gif);background-repeat:no-repeat; height:25px;width:25px;'>");
                                sb.Append((Q[j] == 4 ? 0 : Q[j]).ToString());
                                sb.Append("</td>");
                            }

                            sb.Append("</tr></table>");
                        }

                        sb.Append("</td></tr>");
                    }

                    sb.Append("</tbody></table>");

                }

                //2四场进球彩
                if (dr["LotteryID"].ToString() == "2")
                {
                    string CacheKey = "dtZCDZ" + dr["LotteryID"].ToString();
                    DataTable dtZCDZ = Shove._Web.Cache.GetCacheAsDataTable(CacheKey);

                    if (dtZCDZ == null)
                    {
                        dtZCDZ = new DAL.Tables.T_IsuseForJQC().Open("", "IsuseID=" + dr["IsuseID"].ToString(), "[NO]");

                        Shove._Web.Cache.SetCache(CacheKey, dtZCDZ);
                    }

                    string[] xhfzshu = t_str.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries); //选号分组
                    string playTypeName = dr["PlayTypeName"].ToString(); //玩法名称
                    int N = xhfzshu.Length; //分组数
                    int[, ,] fzHao = new int[N, 8, 4]; //分组号码 N*14*3 矩阵

                    for (int i = 0; i < N; i++)
                    {
                        if (playTypeName == "单式")
                        {
                            for (int j = 0; j < 8; j++)
                            {
                                int number = int.Parse(xhfzshu[i].Substring(j, 1)); //取一个号
                                //记录选号，未选中为-1
                                for (int a = 0; a < 4; a++)
                                {
                                    if (number == a)
                                    {
                                        fzHao[i, j, a] = a;
                                    }
                                    else
                                    {
                                        fzHao[i, j, a] = -1;
                                    }
                                }
                            }
                        }

                        if (playTypeName == "复式")
                        {
                            string zxh = xhfzshu[i].Replace("-", "(2)").Replace("(", "|(").Replace(")", ")|").Replace("||", "|").Trim();

                            if (zxh.StartsWith("|"))
                            {
                                zxh = zxh.Remove(0, 1);
                            }

                            if (zxh.EndsWith("|"))
                            {
                                zxh = zxh.Substring(0, zxh.Length - 1);
                            }

                            string[] arrZxh = zxh.Split('|');

                            zxh = "";

                            foreach (string s in arrZxh)
                            {
                                string ss = "";

                                if (s.StartsWith("("))
                                {
                                    ss = s;
                                }
                                else
                                {
                                    for (int l = 0; l < s.Length; l++)
                                    {
                                        ss += "(" + s.Substring(l, 1) + ")";
                                    }
                                }

                                zxh += ss;
                            }

                            zxh = zxh.Replace(")", "|").Replace("(", "|").Replace("||", "|");

                            if (zxh.StartsWith("|"))
                            {
                                zxh = zxh.Remove(0, 1);
                            }

                            if (zxh.EndsWith("|"))
                            {
                                zxh = zxh.Substring(0, zxh.Length - 1);
                            }

                            arrZxh = zxh.Split('|');

                            for (int j = 0; j < 8; j++)
                            {
                                //记录选号，未选中为-1
                                for (int x = 0; x < 4; x++)
                                {
                                    fzHao[i, j, x] = -1;
                                }

                                for (int y = 0; y < arrZxh[j].Length; y++)
                                {
                                    int number = int.Parse(arrZxh[j].Substring(y, 1));

                                    for (int x = 0; x < 4; x++)
                                    {
                                        if (number == x)
                                        {
                                            fzHao[i, j, x] = x;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    StringBuilder sbXhfz = new StringBuilder();

                    for (int i = 0; i < N; i++) //分组数
                    {
                        sbXhfz.Append("<div class='blue'><a href='javascript:;' onmouseover='displayBall(").Append((i + 1).ToString()).Append(",").Append(N.ToString()).Append(",8);' onmouseout=\"Div_Zc.style.display='none'\">").Append(xhfzshu[i]).Append("</a>").Append(i == N - 1 && i > 0 ? "<span style='margin-left:40px;color:red;'>【注】</span>点击投注内容查看选号区" : "").Append("<div/>");
                    }

                    labLotteryNumber.Text = sbXhfz.ToString();

                    sb.Append("<table style='width:100%; background-color:#ccc; text-align:center; line-height:25px;' cellspacing='1' cellpadding='0'>")
                        .Append("<tbody style='color:red;'><tr><td>场次</td><td>主客队</td><td>比赛时间</td><td>选号区</td></tr></tbody>")
                        .Append("<tbody style='background-color:White;'>");

                    int c = dtZCDZ.Rows.Count;
                    for (int i = 0; i < c; i++)
                    {
                        sb.Append("<tr><td>").Append(dtZCDZ.Rows[i]["NO"].ToString())
                            .Append("</td><td>").Append(dtZCDZ.Rows[i]["Team"].ToString())
                            .Append("</td><td>").Append(dtZCDZ.Rows[i]["DateTime"].ToString())
                            .Append("</td><td>");

                        for (int k = 0; k < N; k++)
                        {
                            sb.Append("<table id='Xhfz_").Append((100 * (k + 1) + i + 1).ToString()).Append("' style='color:White; font-weight:bold;").Append(k == 0 ? "" : "display:none;").Append("'><tr>");

                            for (int j = 0; j < 4; j++)
                            {
                                sb.Append("<td style='background-image:url(Images/");
                                sb.Append(fzHao[k, i, j] == j ? "zfb_redball" : "zfb_huiball").Append(".gif);background-repeat:no-repeat; height:25px;width:25px;'>");
                                sb.Append(j == 3 ? "3+" : j.ToString());
                                sb.Append("</td>");
                            }

                            sb.Append("</tr></table>");
                        }

                        sb.Append("</td></tr>");
                    }

                    sb.Append("</tbody></table>");
                }

                //15六场半全场
                if (dr["LotteryID"].ToString() == "15")
                {
                    string CacheKey = "dtZCDZ" + dr["LotteryID"].ToString();
                    DataTable dtZCDZ = Shove._Web.Cache.GetCacheAsDataTable(CacheKey);

                    if (dtZCDZ == null)
                    {
                        dtZCDZ = new DAL.Tables.T_IsuseForLCBQC().Open("", "IsuseID=" + dr["IsuseID"].ToString(), "[NO]");

                        Shove._Web.Cache.SetCache(CacheKey, dtZCDZ);
                    }

                    t_str = t_str.Replace("0", "4");
                    string[] xhfzshu = t_str.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries); //选号分组
                    string playTypeName = dr["PlayTypeName"].ToString(); //玩法名称
                    int N = xhfzshu.Length; //分组数
                    int[, ,] fzHao = new int[N, 12, 3]; //分组号码 N*12*3 矩阵

                    for (int i = 0; i < N; i++)
                    {
                        if (playTypeName == "单式")
                        {
                            for (int j = 0; j < 12; j++)
                            {
                                int number = int.Parse(xhfzshu[i].Substring(j, 1)); //取一个号
                                //记录选号，未选中为-1
                                if (number == 3) { fzHao[i, j, 0] = 3; } else { fzHao[i, j, 0] = -1; }
                                if (number == 1) { fzHao[i, j, 1] = 1; } else { fzHao[i, j, 1] = -1; }
                                if (number == 4) { fzHao[i, j, 2] = 4; } else { fzHao[i, j, 2] = -1; }
                            }
                        }

                        if (playTypeName == "复式")
                        {
                            string zxh = xhfzshu[i].Replace("-", "(2)").Replace("(", "|(").Replace(")", ")|").Replace("||", "|").Trim();

                            if (zxh.StartsWith("|"))
                            {
                                zxh = zxh.Remove(0, 1);
                            }

                            if (zxh.EndsWith("|"))
                            {
                                zxh = zxh.Substring(0, zxh.Length - 1);
                            }

                            string[] arrZxh = zxh.Split('|');

                            zxh = "";

                            foreach (string s in arrZxh)
                            {
                                string ss = "";

                                if (s.StartsWith("("))
                                {
                                    ss = s;
                                }
                                else
                                {
                                    for (int l = 0; l < s.Length; l++)
                                    {
                                        ss += "(" + s.Substring(l, 1) + ")";
                                    }
                                }

                                zxh += ss;
                            }

                            zxh = zxh.Replace(")", "|").Replace("(", "|").Replace("||", "|");

                            if (zxh.StartsWith("|"))
                            {
                                zxh = zxh.Remove(0, 1);
                            }

                            if (zxh.EndsWith("|"))
                            {
                                zxh = zxh.Substring(0, zxh.Length - 1);
                            }

                            arrZxh = zxh.Split('|');

                            for (int j = 0; j < 12; j++)
                            {
                                int[] Q = new int[] { 3, 1, 4 };

                                //记录选号，未选中为-1
                                for (int y = 0; y < arrZxh[j].Length; y++)
                                {
                                    int number = int.Parse(arrZxh[j].Substring(y, 1));

                                    for (int x = 0; x < 3; x++)
                                    {
                                        if (number == Q[x])
                                        {
                                            fzHao[i, j, x] = Q[x];
                                        }
                                        else
                                        {
                                            if (fzHao[i, j, x] == 0)
                                            {
                                                fzHao[i, j, x] = -1;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    StringBuilder sbXhfz = new StringBuilder();

                    for (int i = 0; i < N; i++) //分组数
                    {
                        sbXhfz.Append("<div class='blue'><a href='javascript:;' onmouseover='displayBall(").Append((i + 1).ToString()).Append(",").Append(N.ToString()).Append(",12);' onmouseout=\"Div_Zc.style.display='none'\">").Append(xhfzshu[i].Replace("4", "0")).Append("</a>").Append(i == N - 1 && i > 0 ? "<span style='margin-left:40px;color:red;'>【注】</span>鼠标移动到投注内容查看选号区" : "").Append("<div/>");
                    }

                    labLotteryNumber.Text = sbXhfz.ToString();

                    sb.Append("<table style='width:100%; background-color:#ccc; text-align:center; line-height:25px;' cellspacing='1' cellpadding='0'>")
                        .Append("<tbody style='color:red;'><tr><td>场次</td><td>对阵双方</td><td>比赛时间</td><td>半全场</td><td>选号区</td></tr></tbody>")
                        .Append("<tbody style='background-color:White;'>");

                    int c = dtZCDZ.Rows.Count * 2;

                    for (int i = 0; i < c; i++)
                    {
                        if (i % 2 == 0)
                        {
                            sb.Append("<tr><td rowspan='2'>").Append(dtZCDZ.Rows[i / 2]["NO"].ToString())
                                .Append("</td><td rowspan='2'>").Append(dtZCDZ.Rows[i / 2]["HostTeam"].ToString()).Append("<span style='color:red;'> VS </span>").Append(dtZCDZ.Rows[i / 2]["QuestTeam"].ToString())
                                .Append("</td><td rowspan='2'>").Append(dtZCDZ.Rows[i / 2]["DateTime"].ToString())
                                .Append("</td>");
                        }

                        sb.Append("<td>").Append(i % 2 == 0 ? "半" : "全").Append("场</td><td>");

                        for (int k = 0; k < N; k++)
                        {
                            sb.Append("<table id='Xhfz_").Append((100 * (k + 1) + i + 1).ToString()).Append("' style='color:White; font-weight:bold;").Append(k == 0 ? "" : "display:none;").Append("'><tr>");

                            int[] Q = new int[] { 3, 1, 4 };
                            for (int j = 0; j < 3; j++)
                            {
                                sb.Append("<td style='background-image:url(Images/");
                                sb.Append(fzHao[k, i, j] == Q[j] ? "zfb_redball" : "zfb_huiball").Append(".gif);background-repeat:no-repeat; height:25px;width:25px;'>");
                                sb.Append((Q[j] == 4 ? 0 : Q[j]).ToString());
                                sb.Append("</td>");
                            }

                            sb.Append("</tr></table>");
                        }

                        sb.Append("</td></tr>");
                    }

                    sb.Append("</table></tbody>");
                }

                tbZCDZ.Text = sb.ToString();

                #endregion
            }
        }

        // 填充
        labSchemeNumber.Text = dr["SchemeNumber"].ToString();
        labSchemeDateTime.Text = dr["DateTime"].ToString();
        labSchemeMoney.Text = Shove._Convert.StrToDouble(dr["Money"].ToString(), 0).ToString("N");
        labSchemeTitle.Text = dr["Title"].ToString() + "&nbsp;";
        labSchemeDescription.Text = dr["Description"].ToString() + "&nbsp;";

        labSchemeADUrl.Text = Shove._Web.Utility.GetUrl() + "/Home/Room/Scheme.aspx?id=" + tbSchemeID.Text;  //this.Page.Request.Url.AbsoluteUri;
        labSchemeDetail.Text = string.Format("此方案总金额 <FONT color='red'>{0}</font> 元，共 <FONT color='red'>{1}</font> 份，每份 <FONT color='red'>{2}</font> 元。<br />已认购 <FONT color='red'>{3}</font> 份（金额 <FONT color='red'>{4}</font> 元）", labSchemeMoney.Text, Share, (Money / Share).ToString("N"), BuyedShare, ((Money / Share) * BuyedShare).ToString("N")) +
                                (CanBuy ? string.Format("，还有 <FONT color='red'>{0}</font> 份（金额 <FONT color='red'>{1}</font> 元）可以认购！", Share - BuyedShare, ((Money / Share) * (Share - BuyedShare)).ToString("N")) : "");

        labAssureMoney.Text = (AssureMoney > 0) ? string.Format("发起人保底 <FONT color='red'>{0}</font> 份，<FONT color='red'>{1}</font> 元", Math.Round(AssureMoney / (Money / Share), 0).ToString(), AssureMoney.ToString("N")) : "未保底";

        if (QuashStatus > 0)
        {
            if (QuashStatus == 2)
            {
                labWin.Text = "已撤单(系统撤单)";
            }
            else
            {
                labWin.Text = "已撤单";
            }
        }
        else
        {
            if (Stop)
            {
                labWin.Text = string.Format("<FONT color='red'>{0}</font> 元", WinMoney.ToString("N"));
                string WinDescription = dr["WinDescription"].ToString();

                if (WinDescription != "")
                {
                    labWin.Text += "<br />" + WinDescription;
                }
                else
                {
                    if (IsuseOpenedWined)
                    {
                        labWin.Text += "  未中奖";
                    }
                    else
                    {
                        labWin.Text += "  <font color='red'>【注】</font>中奖结果在开奖后需要一段时间才能显示。";
                    }
                }
            }
            else
            {
                labWin.Text = "尚未截止";
            }
        }

        if (IsuseOpenedWined)
        {
            if (LotteryID == "1" || LotteryID == "2" || LotteryID == "15")
            {
                labWin.Text += "(命中<font color='red'>" + CompareLotteryNumberToWinNumber(dr["LotteryNumber"].ToString(), dr["WinLotteryNumber"].ToString()).ToString() + "</font>场)";
            }
        }

        if (Stop)
        {
            labCannotBuyTip.Text = "方案已截止，不能认购";
            labCannotBuyTip.Visible = true;
            pBuy.Visible = false;
            btnOK.Enabled = false;
        }
        else
        {
            if (QuashStatus > 0)
            {
                labCannotBuyTip.Text = "方案已撤单，不能认购";
                labCannotBuyTip.Visible = true;
                pBuy.Visible = false;
                btnOK.Enabled = false;
            }
            else
            {
                if (BuyedShare >= Share)
                {
                    labCannotBuyTip.Text = "方案已满员，不能认购";
                    labCannotBuyTip.Visible = true;
                    pBuy.Visible = false;
                    btnOK.Enabled = false;
                }
                else
                {
                    labCannotBuyTip.Visible = false;
                    pBuy.Visible = true;
                    btnOK.Enabled = true;
                }
            }
        }

        labShare.Text = (Share - BuyedShare).ToString();
        labShareMoney.Text = (Money / Share).ToString("N");

        // 绑定参与用户列表
        BindDataForUserList();

        if (_User != null)
        {
            DataTable dtMyBuy = new DAL.Views.V_BuyDetailsWithQuashedAll().Open("[id],[DateTime],[Money],Share,SchemeShare,BuyedShare,QuashStatus,Buyed,IsuseID,Code,Schedule,DetailMoney,isWhenInitiate, WinMoneyNoWIthTax", "SiteID = " + _Site.ID.ToString() + " and SchemeID = " + Shove._Web.Utility.FilteSqlInfusion(tbSchemeID.Text) + " and [UserID] = " + _User.ID.ToString(), "[id]");

            if (dtMyBuy == null)
            {
                PF.GoError(ErrorNumber.DataReadWrite, "数据库繁忙，请重试(-518)", this.GetType().FullName);

                return;
            }

            if (dtMyBuy.Rows.Count == 0)
            {
                labMyBuy.Text = "此方案还没有我的认购记录。";
                labMyBuy.Visible = true;

                g.Visible = false;
            }
            else
            {
                labMyBuy.Visible = false;

                g.Visible = true;

                PF.DataGridBindData(g, dtMyBuy);

                if (IsuseOpenedWined)
                {
                    double DetailMoney = 0;

                    for (int i = 0; i < dtMyBuy.Rows.Count; i++)
                    {
                        DetailMoney += double.Parse(dtMyBuy.Rows[i]["WinMoneyNoWIthTax"].ToString());
                    }

                    lbReward.Text = DetailMoney.ToString("N");
                }

            }
        }

    }

    private void NumberDuiBi(string LotteryNumber, string WinLotteryNumber, int PlayTypeID)
    {
        #region 重庆时时彩
        if (PlayTypeID == 2803 || PlayTypeID == 2806)
        {
            string[] Lotterys = LotteryNumber.Split('\n');
            string[] Locate = new string[5];

            Regex regex = new Regex(@"(?<L0>([\d-])|([(][\d-]+?[)]))(?<L1>([\d-])|([(][\d-]+?[)]))(?<L2>([\d-])|([(][\d-]+?[)]))(?<L3>([\d-])|([(][\d-]+?[)]))(?<L4>(\d-)|([(][\d-]+?[)]))", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            labLotteryNumber.Text = "";

            for (int ii = 0; ii < Lotterys.Length; ii++)
            {
                Match m = regex.Match(Lotterys[ii]);

                for (int j = 0; j < 5; j++)
                {
                    Locate[j] = m.Groups["L" + j.ToString()].ToString().Trim();

                    if (Locate[j] == "")
                    {
                        continue;
                    }

                    if (Locate[j].Length > 1)
                    {
                        Locate[j] = Locate[j].Substring(1, Locate[j].Length - 2);

                        if (Locate[j] == "")
                        {
                            continue;
                        }

                        Locate[j] = "(" + Locate[j] + ")";
                    }

                    if (Locate[j].IndexOf(WinLotteryNumber.Substring(j, 1)) >= 0)
                    {
                        Locate[j] = Locate[j].Replace(WinLotteryNumber.Substring(j, 1), "<font color='Red'>" + WinLotteryNumber.Substring(j, 1) + "</font>");
                    }

                    labLotteryNumber.Text += Locate[j];
                }

                labLotteryNumber.Text += "\n";
            }
        }


        #endregion

        #region 江西时时彩
        if (PlayTypeID == 6106 || PlayTypeID == 6106 || PlayTypeID == 6117 || PlayTypeID == 6118)
        {
            string[] Lotterys = LotteryNumber.Split('\n');
            string[] Locate = new string[5];

            Regex regex = new Regex(@"(?<L0>([\d-])|([(][\d]+?[)]))(?<L1>([\d-])|([(][\d]+?[)]))(?<L2>([\d-])|([(][\d]+?[)]))(?<L3>([\d-])|([(][\d]+?[)]))(?<L4>(\d)|([(][\d]+?[)]))", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            labLotteryNumber.Text = "";

            for (int ii = 0; ii < Lotterys.Length; ii++)
            {
                Match m = regex.Match(Lotterys[ii]);

                for (int j = 0; j < 5; j++)
                {
                    Locate[j] = m.Groups["L" + j.ToString()].ToString().Trim();

                    if (Locate[j] == "")
                    {
                        continue;
                    }

                    if (Locate[j].Length > 1)
                    {
                        Locate[j] = Locate[j].Substring(1, Locate[j].Length - 2);

                        if (Locate[j] == "")
                        {
                            continue;
                        }

                        Locate[j] = "(" + Locate[j] + ")";
                    }

                    if (Locate[j].IndexOf(WinLotteryNumber.Substring(j, 1)) >= 0)
                    {
                        Locate[j] = Locate[j].Replace(WinLotteryNumber.Substring(j, 1), "<font color='Red'>" + WinLotteryNumber.Substring(j, 1) + "</font>");
                    }

                    labLotteryNumber.Text += Locate[j];
                }

                labLotteryNumber.Text += "\n";
            }
        }
        #endregion

        #region 十一运夺金
        if (LotteryID == "62")
        {
            string Number = LotteryNumber.Replace("&nbsp;", " ").Replace("<br/>", " ").Trim();
            string[] lotteryNumber = null;
            string[] winLotteryNumber = WinLotteryNumber.Split(' ');

            if (PlayTypeID == 6201 || PlayTypeID == 6209 || PlayTypeID == 6210)
            {
                Number = Number.Replace(" ", "|");
                lotteryNumber = Number.Split('|');
                if (PlayTypeID == 6201)
                {
                    for (int i = 0; i < lotteryNumber.Length; i++)
                    {
                        if (lotteryNumber[i] == winLotteryNumber[0])
                        {
                            LotteryNumber = LotteryNumber.Replace(lotteryNumber[i], "<font color='Red'>" + lotteryNumber[i] + "</font>");
                        }
                    }
                }
                else if (PlayTypeID == 6209)
                {
                    LotteryNumber = "";
                    for (int i = 0; i < lotteryNumber.Length; i++)
                    {
                        if (i % 2 == 0 && lotteryNumber[i] == winLotteryNumber[0])
                        {
                            LotteryNumber += "<font color='Red'>" + lotteryNumber[i] + "</font>|";
                        }
                        else if (i % 2 == 1 && lotteryNumber[i] == winLotteryNumber[1])
                        {
                            LotteryNumber += "<font color='Red'>" + lotteryNumber[i] + "</font>|";
                        }
                        else
                        {
                            LotteryNumber += lotteryNumber[i] + "|";
                        }

                        if (i % 2 == 1)
                        {
                            LotteryNumber += "<br>";
                        }
                    }
                }
                else
                {
                    LotteryNumber = "";
                    for (int i = 0; i < lotteryNumber.Length; i++)
                    {
                        if (i % 3 == 0 && lotteryNumber[i] == winLotteryNumber[0])
                        {
                            LotteryNumber += "<font color='Red'>" + lotteryNumber[i] + "</font>|";
                        }
                        else if (i % 3 == 1 && lotteryNumber[i] == winLotteryNumber[1])
                        {
                            LotteryNumber += "<font color='Red'>" + lotteryNumber[i] + "</font>|";
                        }
                        else if (i % 3 == 2 && lotteryNumber[i] == winLotteryNumber[2])
                        {
                            LotteryNumber += "<font color='Red'>" + lotteryNumber[i] + "</font>|";
                        }
                        else
                        {
                            LotteryNumber += lotteryNumber[i] + "|";
                        }

                        if (i % 3 == 2)
                        {
                            LotteryNumber += "<br>";
                        }
                    }
                }
                labLotteryNumber.Text = LotteryNumber;
            }
            else
            {
                if (PlayTypeID == 6211 || PlayTypeID == 6212)
                {
                    string[] Numbers = LotteryNumber.Replace("<br/>", " ").Split(' ');

                    string[] EachNumber = null;

                    LotteryNumber = "";

                    if (PlayTypeID == 6212)
                    {
                        for (int i = 0; i < Numbers.Length; i++)
                        {
                            EachNumber = Numbers[i].Replace("&nbsp;", " ").Split(' ');
                            for (int j = 0; j < EachNumber.Length; j++)
                            {
                                if (EachNumber[j] == winLotteryNumber[0] || EachNumber[j] == winLotteryNumber[1] || EachNumber[j] == winLotteryNumber[2])
                                {
                                    LotteryNumber += "<font color='Red'>" + EachNumber[j] + "</font>&nbsp;";
                                }
                                else
                                {
                                    LotteryNumber += EachNumber[j] + "&nbsp;";
                                }

                            }
                            LotteryNumber += "<br>";
                        }
                    }
                    else
                    {
                        for (int i = 0; i < Numbers.Length; i++)
                        {
                            EachNumber = Numbers[i].Replace("&nbsp;", " ").Split(' ');
                            for (int j = 0; j < EachNumber.Length; j++)
                            {
                                if (EachNumber[j] == winLotteryNumber[0] || EachNumber[j] == winLotteryNumber[1])
                                {
                                    LotteryNumber += "<font color='Red'>" + EachNumber[j] + "</font>&nbsp;";
                                }
                                else
                                {
                                    LotteryNumber += EachNumber[j] + "&nbsp;";
                                }

                            }
                            LotteryNumber += "<br>";
                        }
                    }

                    labLotteryNumber.Text = LotteryNumber;
                }
                else
                {
                    lotteryNumber = Number.Replace("|", " ").Split(' ');

                    foreach (string s in lotteryNumber)
                    {
                        if (WinLotteryNumber.IndexOf(s) > -1)
                        {
                            LotteryNumber = LotteryNumber.Replace(s, "<font color='Red'>" + s + "</font>");
                        }
                    }

                    labLotteryNumber.Text = LotteryNumber;
                }
            }
        }
        #endregion

        #region 胜负彩
        if (LotteryID == "1")
        {
            labLotteryNumber.Text = "";

            string[] Number = LotteryNumber.Trim().Split('\n');

            Regex regex = new Regex(@"(?<L0>([013-])|([(][013-]{1,3}[)]))(?<L1>([013-])|([(][013-]{1,3}[)]))(?<L2>([013-])|([(][013-]{1,3}[)]))(?<L3>([013-])|([(][013-]{1,3}[)]))(?<L4>([013-])|([(][013-]{1,3}[)]))(?<L5>([013-])|([(][013-]{1,3}[)]))(?<L6>([013-])|([(][013-]{1,3}[)]))(?<L7>([013-])|([(][013-]{1,3}[)]))(?<L8>([013-])|([(][013-]{1,3}[)]))(?<L9>([013-])|([(][013-]{1,3}[)]))(?<L10>([013-])|([(][013-]{1,3}[)]))(?<L11>([013-])|([(][013-]{1,3}[)]))(?<L12>([013-])|([(][013-]{1,3}[)]))(?<L13>([013-])|([(][013-]{1,3}[)]))", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            string[] Locate = new string[14];

            foreach (string s in Number)
            {
                Match m = regex.Match(s);

                if (!m.Success)
                {
                    continue;
                }

                for (int j = 0; j < 14; j++)
                {
                    Locate[j] = m.Groups["L" + j.ToString()].ToString().Trim();

                    if (Locate[j] == "")
                    {
                        continue;
                    }

                    if (Locate[j].IndexOf(WinLotteryNumber.Substring(j, 1)) >= 0)
                    {
                        labLotteryNumber.Text += Locate[j].Replace(WinLotteryNumber.Substring(j, 1), "<font color='Red'>" + WinLotteryNumber.Substring(j, 1) + "</font>");
                    }
                }

                labLotteryNumber.Text += "\n\r";
            }
        }
        #endregion

        #region 六场半全场
        if (LotteryID == "15")
        {
            labLotteryNumber.Text = "";

            string[] Number = LotteryNumber.Trim().Split('\n');

            Regex regex = new Regex(@"(?<L0>([013-])|([(][013-]{1,3}[)]))(?<L1>([013-])|([(][013-]{1,3}[)]))(?<L2>([013-])|([(][013-]{1,3}[)]))(?<L3>([013-])|([(][013-]{1,3}[)]))(?<L4>([013-])|([(][013-]{1,3}[)]))(?<L5>([013-])|([(][013-]{1,3}[)]))(?<L6>([013-])|([(][013-]{1,3}[)]))(?<L7>([013-])|([(][013-]{1,3}[)]))(?<L8>([013-])|([(][013-]{1,3}[)]))(?<L9>([013-])|([(][013-]{1,3}[)]))(?<L10>([013-])|([(][013-]{1,3}[)]))(?<L11>([013-])|([(][013-]{1,3}[)]))", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            string[] Locate = new string[12];

            foreach (string s in Number)
            {
                Match m = regex.Match(s);

                if (!m.Success)
                {
                    continue;
                }

                for (int j = 0; j < 12; j++)
                {
                    Locate[j] = m.Groups["L" + j.ToString()].ToString().Trim();

                    if (Locate[j] == "")
                    {
                        continue;
                    }

                    if (Locate[j].IndexOf(WinLotteryNumber.Substring(j, 1)) >= 0)
                    {
                        labLotteryNumber.Text += Locate[j].Replace(WinLotteryNumber.Substring(j, 1), "<font color='Red'>" + WinLotteryNumber.Substring(j, 1) + "</font>");
                    }
                }

                labLotteryNumber.Text += "\n\r";
            }
        }
        #endregion

        #region 四场进球彩
        if (LotteryID == "2")
        {
            labLotteryNumber.Text = "";

            string[] Number = LotteryNumber.Trim().Split('\n');

            Regex regex = new Regex(@"(?<L0>([0123])|([(][0123]{1,3}[)]))(?<L1>([0123])|([(][0123]{1,3}[)]))(?<L2>([0123])|([(][0123]{1,3}[)]))(?<L3>([0123])|([(][0123]{1,3}[)]))(?<L4>([0123])|([(][0123]{1,3}[)]))(?<L5>([0123])|([(][0123]{1,3}[)]))(?<L6>([0123])|([(][0123]{1,3}[)]))(?<L7>([0123])|([(][0123]{1,3}[)]))", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            string[] Locate = new string[8];

            foreach (string s in Number)
            {
                Match m = regex.Match(s);

                if (!m.Success)
                {
                    continue;
                }

                for (int j = 0; j < 8; j++)
                {
                    Locate[j] = m.Groups["L" + j.ToString()].ToString().Trim();

                    if (Locate[j] == "")
                    {
                        continue;
                    }

                    if (Locate[j].IndexOf(WinLotteryNumber.Substring(j, 1)) >= 0)
                    {
                        labLotteryNumber.Text += Locate[j].Replace(WinLotteryNumber.Substring(j, 1), "<font color='Red'>" + WinLotteryNumber.Substring(j, 1) + "</font>");
                    }
                }

                labLotteryNumber.Text += "\n\r";
            }
        }
        #endregion

        #region 时时乐
        if (PlayTypeID == 2902)
        {
            labLotteryNumber.Text = "";

            string[] Number = LotteryNumber.Trim().Split('\n');

            Regex regex = new Regex(@"(?<L0>([\d])|([(][\d]+?[)]))(?<L1>([\d])|([(][\d]+?[)]))(?<L2>([\d])|([(][\d]+?[)]))", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            string[] Locate = new string[3];

            foreach (string s in Number)
            {
                Match m = regex.Match(s);

                if (!m.Success)
                {
                    continue;
                }

                for (int j = 0; j < 3; j++)
                {
                    Locate[j] = m.Groups["L" + j.ToString()].ToString().Trim();

                    if (Locate[j] == "")
                    {
                        continue;
                    }

                    if (Locate[j].IndexOf(WinLotteryNumber.Substring(j, 1)) >= 0)
                    {
                        labLotteryNumber.Text += Locate[j].Replace(WinLotteryNumber.Substring(j, 1), "<font color='Red'>" + WinLotteryNumber.Substring(j, 1) + "</font>");
                    }
                }

                labLotteryNumber.Text += "\n\r";
            }
        }
        #endregion

        #region 双色球
        if (PlayTypeID == 602)
        {
            labLotteryNumber.Text = "";

            string[] Number = LotteryNumber.Trim().Split('\n');
            string[] WinLotteryNumbers = WinLotteryNumber.Split(' ');

            foreach (string s in Number)
            {
                for (int i = 0; i < WinLotteryNumbers.Length; i++)
                {
                    if (s.IndexOf(WinLotteryNumbers[i]) >= 0)
                    {
                        labLotteryNumber.Text += s.Replace(WinLotteryNumbers[i], "<font color='Red'>" + WinLotteryNumbers[i] + "</font>");
                    }
                }

                labLotteryNumber.Text += "\n\r";
            }
        }
        #endregion

        #region 福彩3D
        if (PlayTypeID == 602)
        {
            string[] Lotterys = LotteryNumber.Split('\n');
            string[] Locate = new string[3];

            Regex regex = new Regex(@"(?<L0>([\d])|([(][\d]+?[)]))(?<L1>([\d])|([(][\d]+?[)]))(?<L2>([\d])|([(][\d]+?[)]))", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            labLotteryNumber.Text = "";

            for (int ii = 0; ii < Lotterys.Length; ii++)
            {
                Match m = regex.Match(Lotterys[ii]);

                for (int j = 0; j < 3; j++)
                {
                    Locate[j] = m.Groups["L" + j.ToString()].ToString().Trim();

                    if (Locate[j] == "")
                    {
                        continue;
                    }

                    if (Locate[j].Length > 1)
                    {
                        Locate[j] = Locate[j].Substring(1, Locate[j].Length - 2);

                        if (Locate[j] == "")
                        {
                            continue;
                        }

                        Locate[j] = "(" + Locate[j] + ")";
                    }

                    if (Locate[j].IndexOf(WinLotteryNumber.Substring(j, 1)) >= 0)
                    {
                        Locate[j] = Locate[j].Replace(WinLotteryNumber.Substring(j, 1), "<font color='Red'>" + WinLotteryNumber.Substring(j, 1) + "</font>");
                    }

                    labLotteryNumber.Text += Locate[j];
                }

                labLotteryNumber.Text += "\n";
            }
        }
        #endregion

        #region 七星彩
        if (PlayTypeID == 302)
        {
            string[] Lotterys = LotteryNumber.Split('\n');
            string[] Locate = new string[7];

            Regex regex = new Regex(@"(?<L0>([\d])|([(][\d]+?[)]))(?<L1>([\d])|([(][\d]+?[)]))(?<L2>([\d])|([(][\d]+?[)]))(?<L3>([\d])|([(][\d]+?[)]))(?<L4>([\d])|([(][\d]+?[)]))(?<L5>([\d])|([(][\d]+?[)]))(?<L6>([\d])|([(][\d]+?[)]))", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            labLotteryNumber.Text = "";

            for (int ii = 0; ii < Lotterys.Length; ii++)
            {
                Match m = regex.Match(Lotterys[ii]);

                for (int j = 0; j < 7; j++)
                {
                    Locate[j] = m.Groups["L" + j.ToString()].ToString().Trim();

                    if (Locate[j] == "")
                    {
                        continue;
                    }

                    if (Locate[j].Length > 1)
                    {
                        Locate[j] = Locate[j].Substring(1, Locate[j].Length - 2);

                        if (Locate[j] == "")
                        {
                            continue;
                        }

                        Locate[j] = "(" + Locate[j] + ")";
                    }

                    if (Locate[j].IndexOf(WinLotteryNumber.Substring(j, 1)) >= 0)
                    {
                        Locate[j] = Locate[j].Replace(WinLotteryNumber.Substring(j, 1), "<font color='Red'>" + WinLotteryNumber.Substring(j, 1) + "</font>");
                    }

                    labLotteryNumber.Text += Locate[j];
                }

                labLotteryNumber.Text += "\n";
            }
        }
        #endregion

        #region 排列三
        if (PlayTypeID == 6302)
        {
            string[] Lotterys = LotteryNumber.Split('\n');
            string[] Locate = new string[3];

            Regex regex = new Regex(@"(?<L0>([\d])|([(][\d]+?[)]))(?<L1>([\d])|([(][\d]+?[)]))(?<L2>([\d])|([(][\d]+?[)]))", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            labLotteryNumber.Text = "";

            for (int ii = 0; ii < Lotterys.Length; ii++)
            {
                Match m = regex.Match(Lotterys[ii]);

                for (int j = 0; j < 3; j++)
                {
                    Locate[j] = m.Groups["L" + j.ToString()].ToString().Trim();

                    if (Locate[j] == "")
                    {
                        continue;
                    }

                    if (Locate[j].Length > 1)
                    {
                        Locate[j] = Locate[j].Substring(1, Locate[j].Length - 2);

                        if (Locate[j] == "")
                        {
                            continue;
                        }

                        Locate[j] = "(" + Locate[j] + ")";
                    }

                    if (Locate[j].IndexOf(WinLotteryNumber.Substring(j, 1)) >= 0)
                    {
                        Locate[j] = Locate[j].Replace(WinLotteryNumber.Substring(j, 1), "<font color='Red'>" + WinLotteryNumber.Substring(j, 1) + "</font>");
                    }

                    labLotteryNumber.Text += Locate[j];
                }

                labLotteryNumber.Text += "\n";
            }
        }
        #endregion\

        #region 排列五
        if (PlayTypeID == 6402)
        {
            string[] Lotterys = LotteryNumber.Split('\n');
            string[] Locate = new string[5];

            Regex regex = new Regex(@"(?<L0>([\d])|([(][\d]+?[)]))(?<L1>([\d])|([(][\d]+?[)]))(?<L2>([\d])|([(][\d]+?[)]))(?<L3>([\d])|([(][\d]+?[)]))(?<L4>([\d])|([(][\d]+?[)]))", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            labLotteryNumber.Text = "";

            for (int ii = 0; ii < Lotterys.Length; ii++)
            {
                Match m = regex.Match(Lotterys[ii]);

                for (int j = 0; j < 5; j++)
                {
                    Locate[j] = m.Groups["L" + j.ToString()].ToString().Trim();

                    if (Locate[j] == "")
                    {
                        continue;
                    }

                    if (Locate[j].Length > 1)
                    {
                        Locate[j] = Locate[j].Substring(1, Locate[j].Length - 2);

                        if (Locate[j] == "")
                        {
                            continue;
                        }

                        Locate[j] = "(" + Locate[j] + ")";
                    }

                    if (Locate[j].IndexOf(WinLotteryNumber.Substring(j, 1)) >= 0)
                    {
                        Locate[j] = Locate[j].Replace(WinLotteryNumber.Substring(j, 1), "<font color='Red'>" + WinLotteryNumber.Substring(j, 1) + "</font>");
                    }

                    labLotteryNumber.Text += Locate[j];
                }

                labLotteryNumber.Text += "\n";
            }
        }
        #endregion
    }

    private void BindDataForUserList()
    {
        string sql = "select * from V_BuyDetailsWithQuashed with (nolock) where SiteID = @SiteID and SchemeID = @SchemeID and QuashStatus = 0 order by [id]";
        int All_QuashStatus = Shove._Convert.StrToInt(Shove._Web.Cache.GetCache("All_QuashStatus" + SchemeID).ToString(), 0);
        if (All_QuashStatus == 2)   //当为系统撤单时，查询合买方案的所有参与用户
        {
            sql = "SELECT dbo.T_BuyDetails.DateTime, dbo.T_BuyDetails.Share,dbo.T_BuyDetails.Share * (dbo.T_Schemes.Money / dbo.T_Schemes.Share) AS DetailMoney, dbo.T_Users.Name" +
                  " FROM dbo.T_BuyDetails INNER JOIN " +
                  "  dbo.T_Users ON dbo.T_BuyDetails.UserID = dbo.T_Users.ID INNER JOIN" +
                  " dbo.T_Schemes ON dbo.T_BuyDetails.SchemeID = dbo.T_Schemes.ID AND dbo.T_BuyDetails.QuashStatus <> 1" +
                  " where T_Schemes.SiteID = @SiteID and SchemeID = @SchemeID order by T_Schemes.[id]";
        }
        DataTable dtUserList = MSSQL.Select(sql,
                            new MSSQL.Parameter("SiteID", SqlDbType.BigInt, 0, ParameterDirection.Input, _Site.ID),
                            new MSSQL.Parameter("SchemeID", SqlDbType.VarChar, 0, ParameterDirection.Input, Shove._Web.Utility.FilteSqlInfusion(tbSchemeID.Text)));

        if (dtUserList == null)
        {
            PF.GoError(ErrorNumber.DataReadWrite, "数据库繁忙，请重试(-503)", this.GetType().FullName);

            return;
        }

        labUserList.Text = String.Format("总共有 <font color='red'>{0}</font> 个用户参与。", dtUserList.Rows.Count) +
            ((dtUserList.Rows.Count > 0) ? "&nbsp;&nbsp;【<A class=li3 href='javascript:onUserListClick();'>打开/隐藏明细</A>】" : "");

        gUserList.DataSource = dtUserList.DefaultView;
        gUserList.DataBind();
    }

    protected void btnOK_Click(object sender, System.EventArgs e)
    {
        //if (_User.UserType == 1)
        //{
        //    Shove._Web.JavaScript.Alert(this.Page, "对不起，您还不是高级会员，请先免费升级为高级会员。谢谢！");

        //    return;
        //}

        if (panelInvestPassword.Visible)
        {
            if (PF.EncryptPassword(tbInvestPassword.Text) != _User.PasswordAdv)
            {
                Shove._Web.JavaScript.Alert(this.Page, "投注密码错误！");

                return;
            }
        }

        DateTime EndTime = DateTime.Parse(labEndTime.Text);

        if (DateTime.Now > EndTime)
        {
            Shove._Web.JavaScript.Alert(this.Page, "投注时间已经截止，不能认购。");

            return;
        }

        //既不是发起人，也不在招股对象之内
        if (!_User.isCanViewSchemeContent(SchemeID))
        {
            Shove._Web.JavaScript.Alert(this.Page, "对不起，您不在此方案的招股对象之内。");

            return;
        }

        double ShareMoney = 0;
        int Share = 0;

        try
        {
            ShareMoney = double.Parse(labShareMoney.Text);
            Share = int.Parse(tbShare.Text);
        }
        catch
        {
            Shove._Web.JavaScript.Alert(this.Page, "输入有错误，请仔细检查。");

            return;
        }

        if ((ShareMoney <= 0) || (Share < 1) || (Share > Shove._Convert.StrToInt(labShare.Text, 0)))
        {
            Shove._Web.JavaScript.Alert(this.Page, "输入有错误，请仔细检查。");

            return;
        }

        if ((ShareMoney * Share) > _User.Balance)
        {
            Shove._Web.JavaScript.Alert(this.Page, "您的账户余额不足，请先充值，谢谢。");

            return;
        }

        string ReturnDescription = "";

        if (_User.JoinScheme(int.Parse(tbSchemeID.Text), Share, ref ReturnDescription) < 0 || ReturnDescription != "")
        {
            if (ReturnDescription.IndexOf("方案剩余份数已不足") > -1)
            {
                try
                {
                    string strShare = ReturnDescription.Split(new string[] { ",剩余 " }, StringSplitOptions.None)[1].Split(' ')[0].ToString();

                    ScriptManager.RegisterStartupScript(Page, this.GetType(), "", "alert('" + ReturnDescription + "');document.getElementById('tbShare').value='" + strShare + "';document.getElementById('labShare').innerText='" + strShare + "';", true);
                }
                catch
                {
                    ScriptManager.RegisterStartupScript(Page, this.GetType(), "", "alert('方案剩余份数已不足 " + Share.ToString() + " 份');document.getElementById('tbShare').value='" + (Share - 1).ToString() + "';", true);
                }
            }
            else
            {
                Shove._Web.JavaScript.Alert(this.Page, ReturnDescription);
            }

            return;
        }
        else
        {
            tbShare.Text = "";

            Shove._Web.Cache.ClearCache("Home_Room_CoBuy_BindDataForType" + tbIsuseID.Text);
            Shove._Web.Cache.ClearCache("Home_Room_SchemeAll_BindData" + tbIsuseID.Text);
            Shove._Web.Cache.ClearCache("Home_Room_JoinAllBuy_BindData");

            Response.Write("<script>try{window.opener.parent.ReloadSchedule();} catch(ex) {};window.location.href='UserBuySuccess.aspx?LotteryID=" + LotteryID.ToString() + "&Type=3&Money=" + (ShareMoney * Share).ToString() + "&SchemeID=" + tbSchemeID.Text + "'</script>");
        }
    }

    protected void btnQuashScheme_Click(object sender, System.EventArgs e)
    {
        //if (_User.UserType == 1)
        //{
        //    Shove._Web.JavaScript.Alert(this.Page, "对不起，您还不是高级会员，请先免费升级为高级会员。谢谢！");

        //    return;
        //}

        if (panelInvestPassword.Visible)
        {
            if (PF.EncryptPassword(tbInvestPassword.Text) != _User.PasswordAdv)
            {
                Shove._Web.JavaScript.Alert(this.Page, "投注密码错误！");

                return;
            }
        }

        DateTime EndTime = DateTime.Parse(labEndTime.Text);

        if (DateTime.Now > EndTime)
        {
            Shove._Web.JavaScript.Alert(this.Page, "投注时间已经截止，不能撤消方案。");

            return;
        }

        double Opt_Betting_ForbidenCancel_Percent = Shove._Convert.StrToDouble(new SystemOptions()["Betting_ForbidenCancel_Percent"].Value.ToString(), 0);

        if (Opt_Betting_ForbidenCancel_Percent > 0 && Shove._Convert.StrToDouble(HidSchedule.Value, -1) >= Opt_Betting_ForbidenCancel_Percent)
        {
            Shove._Web.JavaScript.Alert(this.Page, "对不起，由于本方案进度已经达到 " + Opt_Betting_ForbidenCancel_Percent.ToString("N") + "%，即将满员，不允许撤单。");

            return;
        }

        string ReturnDescription = "";

        if (_User.QuashScheme(int.Parse(tbSchemeID.Text), false, ref ReturnDescription) < 0)
        {
            PF.GoError(ErrorNumber.Unknow, ReturnDescription, this.GetType().FullName);

            return;
        }
        Shove._Web.Cache.ClearCache("Home_Room_CoBuy_BindDataForType" + tbIsuseID.Text);
        Shove._Web.Cache.ClearCache("Home_Room_SchemeAll_BindData" + tbIsuseID.Text);

        BindData();

        return;
    }

    protected void cbAtTopApplication_CheckedChanged(object sender, System.EventArgs e)
    {
        int AtTopApplication = cbAtTopApplication.Checked ? 1 : 0;

        if (MSSQL.ExecuteNonQuery("update T_Schemes set AtTopStatus = " + AtTopApplication.ToString() + " where SiteID = " + _Site.ID.ToString() + " and [id] = " + Shove._Web.Utility.FilteSqlInfusion(tbSchemeID.Text)) < 0)
        {
            PF.GoError(ErrorNumber.DataReadWrite, "数据库繁忙，请重试(-678)", this.GetType().FullName);

            return;
        }

        BindData();
    }

    protected void g_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
    {
        if (e.CommandName == "QuashBuy")
        {
            double Opt_Betting_ForbidenCancel_Percent = Shove._Convert.StrToDouble(new SystemOptions()["Betting_ForbidenCancel_Percent"].Value.ToString(), 0);

            if (Opt_Betting_ForbidenCancel_Percent > 0 && Shove._Convert.StrToDouble(HidSchedule.Value, -1) >= Opt_Betting_ForbidenCancel_Percent)
            {
                Shove._Web.JavaScript.Alert(this.Page, "对不起，由于本方案进度已经达到 " + Opt_Betting_ForbidenCancel_Percent.ToString("N") + "%，即将满员，不允许撤单。");

                return;
            }

            long BuyDetailID = Shove._Convert.StrToLong(e.Item.Cells[12].Text, 0);

            if (BuyDetailID < 1)
            {
                PF.GoError(ErrorNumber.Unknow, "参数错误(-694)", this.GetType().FullName);

                return;
            }

            string ReturnDescription = "";

            if (_User.Quash(BuyDetailID, ref ReturnDescription) < 0)
            {
                PF.GoError(ErrorNumber.Unknow, ReturnDescription + "(-703)", this.GetType().FullName);

                return;
            }

            BindData();

            Shove._Web.Cache.ClearCache("Home_Room_CoBuy_BindDataForType" + tbIsuseID.Text);
            Shove._Web.Cache.ClearCache("Home_Room_SchemeAll_BindData" + tbIsuseID.Text);

            return;
        }
    }

    protected void g_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.EditItem)
        {
            e.Item.Cells[0].Text = "<font color='red'>" + e.Item.Cells[0].Text + "</font> 份";

            double money;
            money = Shove._Convert.StrToDouble(e.Item.Cells[1].Text, 0);
            e.Item.Cells[1].Text = "<font color='red'>" + ((money == 0) ? "0.00" : money.ToString("N")) + "</font> 元";

            e.Item.Cells[2].Text = e.Item.Cells[8].Text + e.Item.Cells[9].Text + e.Item.Cells[12].Text;

            short QuashStatus = Shove._Convert.StrToShort(e.Item.Cells[6].Text, 0);
            bool Buyed = Shove._Convert.StrToBool(e.Item.Cells[7].Text, false);
            bool Stop = Shove._Convert.StrToBool(tbStop.Text, false);
            double Schedule = Shove._Convert.StrToDouble(e.Item.Cells[11].Text, 0);
            int SchemeShare = Shove._Convert.StrToInt(e.Item.Cells[14].Text, 0);
            int BuyedShare = Shove._Convert.StrToInt(e.Item.Cells[10].Text, 0);

            Button btnQuashBuy = ((Button)e.Item.Cells[5].FindControl("btnQuashBuy"));

            if (QuashStatus > 0)
            {
                btnQuashBuy.Enabled = false;
                e.Item.Cells[4].Text = "已撤单";
            }
            else
            {
                if (Stop)
                {
                    if (Schedule >= 100)
                    {
                        e.Item.Cells[4].Text = "<Font color=\'Red\'>等待出票</font>";
                    }
                    else
                    {
                        e.Item.Cells[4].Text = "未成功";
                    }

                    btnQuashBuy.Enabled = false;
                }
                else
                {
                    if (Buyed)
                    {
                        e.Item.Cells[4].Text = "<Font color=\'Red\'>已出票</font>";

                        btnQuashBuy.Enabled = false;
                    }
                    else
                    {
                        e.Item.Cells[4].Text = "<Font color=\'Red\'>进行中</font>";
                    }

                    bool isWhenInitiate = Shove._Convert.StrToBool(e.Item.Cells[13].Text, true);
                    bool isFull = (SchemeShare <= BuyedShare);

                    if (isWhenInitiate)
                    {
                        btnQuashBuy.Enabled = false;
                    }
                    else
                    {
                        if (isFull)
                        {
                            btnQuashBuy.Enabled = (_User != null && Opt_FullSchemeCanQuash && (_User.UserType > 1));
                        }
                        else
                        {
                            btnQuashBuy.Enabled = (_User != null && _User.UserType > 1);
                        }
                    }
                }
            }
        }
    }


    [AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)]
    public string GetUserBalance()
    {
        Users u = Users.GetCurrentUser(1);

        return u.Balance.ToString("N");
    }

    [AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)]
    public string AnalyseScheme(string Content, string LotteryID, int PlayTypeID)
    {
        string Result = new SZJS.Lottery()[Shove._Convert.StrToInt(LotteryID, 5)].AnalyseScheme(Content, PlayTypeID);

        return Result.Trim();
    }

    private int CompareLotteryNumberToWinNumber(string LotteryNumber, string WinNumber)
    {
        string[] Number = LotteryNumber.Trim().Split('\n');

        string number;
        bool isEnd;
        int row;
        int Max = 0;
        int Num;
        foreach (string s in Number)
        {
            isEnd = false;
            row = -1;
            Num = 0;

            for (int i = 0; i < s.Trim().Length; i++)
            {
                if (!isEnd)
                {
                    row++;
                }

                number = s.Trim().Substring(i, 1);

                if (number == "(")
                {
                    isEnd = true;
                    continue;
                }
                else
                {
                    if (number == ")")
                    {
                        isEnd = false;
                        continue;
                    }
                    else
                    {
                        if (number == WinNumber.Substring(row, 1))
                        {
                            Num++;
                        }
                    }
                }
            }

            if (Num > Max)
            {
                Max = Num;
            }
        }

        return Max;
    }
}